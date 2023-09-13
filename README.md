# CrystalConnector / 水晶互联
猫猫之间的三次握爪，四次摇尾巴！



## 传输协议

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

### 客户端 to 服务端

#### 认证客户端

为了防止未授权的传输，水晶互联需要一个 SecretKey 来信任客户端，它由服务端配置文件指定。（这个设计没有任何安全性的保证，请保证不在公网中使用它。）

认证请求：

```
> Authenticate
> <string:SecretKey>
```

认证成功：

```
< Authenticated
```

认证失败：

```
<! Close
```

#### 注册接收器

