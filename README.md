# CrystalConnector / 水晶互联
猫猫之间的三次握爪，四次摇尾巴！



## 传输协议

版本：**V2.0（2024.1.9 更新）**

水晶互联采用 WebSocket 进行消息传输，使用 Protobuf 进行消息编码。

Proto 文件见 `CrystalConnector.Protocol/proto` 目录。



### 协议约定

传输的所有的字符串都是 UTF-8 编码。

**服务端不会主动断开客户端的链接。**



#### 服务端响应

```protobuf
message Result {
  oneof result {
    Successful successful = 1;
    Error error = 2;
  }
}
```



#### 操作失败原因列表

目前已定义的原因列表如下：

| 原因                | 说明                                                         |
| ------------------- | ------------------------------------------------------------ |
| Unknown             | 服务端无法理解客户端的需求，产生此错误。                     |
| Authenticated       | 当已认证客户端再次申请认证时，产生此错误。                   |
| NameExists          | 当某个客户端尝试以一个已认证的客户端身份进行认证时，产生此错误。 |
| Unauthenticated     | 当客户端认证失败或者未经认证的客户端执行除认证外的其他操作时，产生此错误。 |
| UnregisteredChannel | 当客户端未注册外发某个频道的信息，却想要发布对应频道信息时，产生此错误。 |



#### 数据结构

##### 消息方向（MessageDirection）

```protobuf
enum MessageDirection {
  NONE = 0;
  INCOMING = 1;
  OUTGOING = 2;
  ALL = 3;
}
```

`Incoming` 代表服务端向客户端转发的信息，`Outgoing` 代表客户端向服务端发布的信息，`All` 代表双向通信。



##### ID 字符串（IdString）

一个字符串，满足正则表达式 `/^[a-z0-9_]+$/` 。

例如，以下每一行都是合法的 ID 字符串：

```
crystalconnector
cat_messenger
qyl27
```

而以下每一行都是非法的的 ID 字符串：

```
CristalConnector
@qyl28
Hello, world!
```



##### 命名空间名 （NamespacedName）

```protobuf
message NamespacedName {
  string namespace = 1;
  string name = 2;
}
```

Namespace 可以是注册这一频道的项目名称，尽量避免冲突。

ChannelName 是自定义的频道名。

**目前版本的服务端对于频道ID没有严格规定，理论上是任意字符串都支持，但是推荐客户端遵守上述格式，以免被日后的更新破坏。**



### 协议操作

#### 认证客户端并注册频道

为了防止未授权的传输，水晶互联需要一个 Secret Key 来信任客户端，它由服务端配置文件指定。（这个设计没有任何安全性的保证，请保证不在公网中使用它。）

为了降低处理压力，客户端需要在启动时向服务端注册频道及所需的消息方向。否则将无法发送或接收消息。

```protobuf
message Authenticate {
  NamespacedName client_id = 1;
  string client_name = 2;
  string secret_key = 3;
  
  repeated Channel channels = 4;
}

message Channel {
  NamespacedName id = 1;
  MessageDirection direction = 2;
}
```



#### 发布消息

Payload 是一个字符串，会被原样发给所有注册了该频道 `Incoming` 方向消息的客户端。

```protobuf
message Publish {
  NamespacedName channel = 1;
  string payload = 2;
}
```



#### 接收消息

当服务端收到发布消息的请求时，所有注册了该频道的客户端都会收到转发的信息。

```protobuf
message Forward {
  string publisher = 1;
  NamespacedName channel = 2;
  string payload = 3;
}
```

`publisher` 是注册客户端时的 `client_name` 。
