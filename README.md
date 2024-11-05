# Unity Client SDK for OpenIM 👨‍💻💬

Use this SDK to add instant messaging capabilities to your Unity Game. By connecting to a self-hosted [OpenIM](https://www.openim.online/) server, you can quickly integrate instant messaging capabilities into your app with just a few lines of code.

The underlying SDK core is implemented in [openim-sdk-cpp](https://github.com/openimsdk/openim-sdk-cpp/tree/unity).

## Documentation 📚

Visit [https://doc.rentsoft.cn/](https://doc.rentsoft.cn/) for detailed documentation and guides.

For the SDK reference, see [https://doc.rentsoft.cn/sdks/quickstart/unity](https://doc.rentsoft.cn/sdks/quickstart/unity).

## Installation 💻

### Adding Unity Package

1. Add SDK package via git url

```
https://github.com/openimsdk/open-im-sdk-unity.git
```

## Usage 🚀

The following examples demonstrate how to use the SDK.

### Importing the SDK and initialize

```C#
using OpenIM.IMSDK.Unity;
using OpenIM.IMSDK.Unity.Listener;
```

### Initialize

```C#
var config = new IMConfig()
{
    PlatformID = (int)PlatformID.WindowsPlatformID,
    ApiAddr = apiAddr,
    WsAddr = wsAddr,
    DataDir = Path.Combine(Application.persistentDataPath, "OpenIM"),
    LogLevel = (int)LogLevel.Debug,
    IsLogStandardOutput = true,
    LogFilePath = Path.Combine(Application.persistentDataPath, "OpenIM/Logs"),
    IsExternalExtensions = false,
};
var suc = IMSDK.InitSDK(config, new ConnListener());
```

### Set Listener

> Note1: You need to set up the listeners first and then log in.

```C#
IMSDK.SetConversationListener(IConversationListener l);
IMSDK.SetGroupListener(IGroupListener l);
IMSDK.SetFriendShipListener(IFriendShipListener l);
IMSDK.SetFriendShipListener(IFriendShipListener l);
IMSDK.SetAdvancedMsgListener(IAdvancedMsgListener l);
IMSDK.SetUserListener(IUserListener l);
IMSDK.SetBatchMsgListener(IBatchMsgListener l)
```

### Login

```C#
var status = IMSDK.GetLoginStatus();
if (status == LoginStatus.Empty || status == LoginStatus.Logout)
{
    IMSDK.Login(userId, token, (suc, err, errMsg) =>
    {
        if (suc)
        {
            Debug.Log("Login UserId:" + IMSDK.GetLoginUserId());
            GetData();
        }
        else
        {
            {
                Debug.Log("Login Failed :" + errMsg);
            }
        }
    });
}
else if (status == LoginStatus.Logged)
{
    Debug.Log("Login UserId:" + IMSDK.GetLoginUserId());
    GetData();
}

```

### Logout

```C#
// Log out the currently logged in user
IMSDK.Logout((suc,err,errMsg)=>{

})
// exit sdk
IMSDK.UnInitSDK();
```

To log into the IM server, you need to create an account and obtain a user ID and token. Refer to the [access token documentation](https://doc.rentsoft.cn/restapi/userManagement/userRegister) for details.

### Receiving and Sending Messages 💬

OpenIM makes it easy to send and receive messages. By default, there is no restriction on having a friend relationship to send messages (although you can configure other policies on the server). If you know the user ID of the recipient, you can conveniently send a message to them.

```java
//Send
Message  msg = OpenIMClient.getInstance().messageManager.createTextMessage("hello openim");
OpenIMClient.getInstance().messageManager.sendMessage(new OnMsgSendCallback() {
            @Override
            public void onError(int code, String error) {
             // Failed to send message ❌
            }

            @Override
            public void onProgress(long progress) {
             //Sending...
            }

            @Override
            public void onSuccess(Message message) {
             // Message sent successfully ✉️

            }
        }, msg, userID, groupID, null);


//Receive
OpenIMClient.getInstance().messageManager.setAdvancedMsgListener(new OnAdvanceMsgListener(){
  @Override
            public void onRecvNewMessage(Message msg) {
                // Received new message 📨
            }
}）
```

## Examples 🌟

You can find a demo that uses the SDK in the [open-im-unity-demo](https://github.com/openimsdk/open-im-unity-demo.git) repository.

## Community :busts_in_silhouette:

- 📚 [OpenIM Community](https://github.com/OpenIMSDK/community)
- 💕 [OpenIM Interest Group](https://github.com/Openim-sigs)
- 🚀 [Join our Slack community](https://join.slack.com/t/openimsdk/shared_invite/zt-22720d66b-o_FvKxMTGXtcnnnHiMqe9Q)
- :eyes: [Join our wechat (微信群)](https://openim-1253691595.cos.ap-nanjing.myqcloud.com/WechatIMG20.jpeg)

## Community Meetings :calendar:

We want anyone to get involved in our community and contributing code, we offer gifts and rewards, and we welcome you to join us every Thursday night.

Our conference is in the [OpenIM Slack](https://join.slack.com/t/openimsdk/shared_invite/zt-22720d66b-o_FvKxMTGXtcnnnHiMqe9Q) 🎯, then you can search the Open-IM-Server pipeline to join

We take notes of each [biweekly meeting](https://github.com/orgs/OpenIMSDK/discussions/categories/meeting) in [GitHub discussions](https://github.com/openimsdk/open-im-server/discussions/categories/meeting), Our historical meeting notes, as well as replays of the meetings are available at [Google Docs :bookmark_tabs:](https://docs.google.com/document/d/1nx8MDpuG74NASx081JcCpxPgDITNTpIIos0DS6Vr9GU/edit?usp=sharing).

## Who are using OpenIM :eyes:

Check out our [user case studies](https://github.com/OpenIMSDK/community/blob/main/ADOPTERS.md) page for a list of the project users. Don't hesitate to leave a [📝comment](https://github.com/openimsdk/open-im-server/issues/379) and share your use case.

## License :page_facing_up:

OpenIM is licensed under the Apache 2.0 license. See [LICENSE](https://github.com/openimsdk/open-im-server/tree/main/LICENSE) for the full license text.
