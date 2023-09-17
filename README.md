# CrystalConnector / 水晶互联
猫猫之间的三次握爪，四次摇尾巴！



## 传输协议

版本：**V1.0（2023.9.17）**

水晶互联采用 WebSocket 进行消息传输，使用 CBOR 进行消息编码。

为了贴近 Guava 中 `ByteArrayDataInput` / `ByteArrayDataOutput` 的使用习惯，规定以单层基本类型的数组形式使用 CBOR 。（Todo：支持更复杂的结构。）

参考代码：

```c#
var writer = new CborWriter();
writer.WriteStartArray(null);    // 不定长数组
// 消息序列化
writer.WriteEndArray();

var reader = new CborReader(buffer);
reader.ReadStartArray();
// 消息反序列化
reader.ReadEndArray();
```



### 文档约定

文档中的代码块，以行首的 `>` 代表客户端向服务端传输的消息；以行首的 `<` 代表服务端向客户端传输的消息。其后的消息以纯文本形式发送。如需某一端进行操作，在尖括号后以 `!` 标志体现。

服务端是指水晶互联的服务端，一个 C# 程序用来完成消息转发操作。

客户端是指所有通过 WebSocket 连接服务端的消息提供或消费者。

文档中使用尖括号对 `<type:Name>` 代表必填参数，方括号对 `[type:Name]` 代表可选参数。 `type` 是参数类型，如 `string` 、 `int` 等。 `Name` 是参数名，不在具体传输过程中体现，是方便开发人员记忆的名称。



### 协议约定

**服务端不会主动断开客户端的链接。**

当客户端操作成功时，服务端必定响应：

```
< Successful
```

当操作失败时，服务端需要指出（供机器处理的）原因：

```
< <string:Reason>
```



#### 操作失败原因列表

目前已定义的原因列表如下：

| 原因                  | 说明                                                         |
| --------------------- | ------------------------------------------------------------ |
| Unknown               | 服务端无法处理客户端所发送的数据包，产生此错误。             |
| Malformed             | 客户端发给服务端的信息是 CBOR 数据包，但无法按协议规则解析时，产生此错误。 |
| Authenticated         | 当已认证客户端再次申请认证时，产生此错误。                   |
| NameAuthenticated     | 当某个客户端尝试以一个已认证的客户端身份进行认证时，产生此错误。 |
| Unauthenticated       | 当未认证客户端执行除认证外的其他操作时，产生此错误。         |
| UndefinedDirection    | 当客户端尝试注册的频道信息方向超过了信息方向枚举允许的值时，产生此错误。 |
| UnregisteredDirection | 当客户端未注册外发某个频道的信息，却想要发布对应频道信息时，产生此错误。 |



#### 数据结构

##### 消息方向（MessageDirection）

```
enum MessageDirection {
	None = 0,
	Incoming = 1,
	Outgoing = 2,
	All = Incoming | Outgoing	// 3
}
```



##### ID 字符串（IdString）

一个字符串，满足正则表达式 `/a-z0-9_/` 。

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



##### 频道 ID （ChannelId）

```
<IdString:Namespace>:<IdString:ChannelName>
```

Namespace 可以是注册这一频道的项目名称，尽量避免冲突。

ChannelName 是自定义的频道名。

**目前版本的服务端对于频道ID没有严格规定，理论上是任意字符串都支持，但是推荐客户端遵守上述格式，以免被日后的更新破坏。**



### 协议操作

#### 认证客户端

为了防止未授权的传输，水晶互联需要一个 SecretKey 来信任客户端，它由服务端配置文件指定。（这个设计没有任何安全性的保证，请保证不在公网中使用它。）

请求：

```
> Authenticate
> <string:SecretKey>
> <string:ClientName>
```



#### 注册频道

为了降低处理压力，客户端需要在启动时向服务端注册频道及消息方向。否则将无法发送或接收消息。

请求：

```
> Register
> <ChannelId:ChannelId>
> <int32:EnumDirection>
```



#### 发布消息

请求：

```
> Publish
> <ChannelId:ChannelId>
> <byte[]:Payload>
```

Payload 是一个字节型数组，会被原样发给其他的客户端（不会发回给自身）。

当服务端收到发布消息的请求时，所有注册了该频道的客户端都会收到如下信息：

```
< Forward
< <string:PublisherName>
< <ChannelId:ChannelId>
< <byte[]:Payload>
```

PublisherName 是先前注册这一频道的客户端。

