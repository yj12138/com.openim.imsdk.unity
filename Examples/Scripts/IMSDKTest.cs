using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OpenIM.IMSDK.Unity;
using OpenIM.IMSDK.Unity.Listener;

public class IMSDKTest : MonoBehaviour
{
    public string wsAddr = "ws://192.168.101.9:10001";
    public string apiAddr = "http://192.168.101.9:10002";
    public string userId = "test";
    public string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySUQiOiJ0ZXN0IiwiUGxhdGZvcm1JRCI6MywiZXhwIjoxNzM2MDgyMzk4LCJuYmYiOjE3MjgzMDYwOTgsImlhdCI6MTcyODMwNjM5OH0.1nxduLgJe5YKvlDwJr-bmV2JWWVkGegfvbBw5Do5oGM";

    bool initRes = false;
    void Awake()
    {
        var config = new IMConfig()
        {
            PlatformID = (int)PlatformID.WindowsPlatformID,
            ApiAddr = apiAddr,
            WsAddr = wsAddr,
            DataDir = Application.persistentDataPath,
            LogLevel = (int)LogLevel.Debug,
            IsLogStandardOutput = true,
            LogFilePath = Application.productName + "/logs/",
            IsExternalExtensions = false,
        };
        var listenGroup = new ListenGroup(
            new ConnListener(),
            new ConversationListener(),
            new GroupListener(),
            new FriendShipListener(),
            new AdvancedMsgListener(),
            new UserListener(),
            new CustomBusinessListener(),
            new BatchMsgListener()
        );
        initRes = IMSDK.InitSDK(config, listenGroup);
    }


    void Start()
    {
        if (initRes)
        {
            var status = IMSDK.GetLoginStatus();
            if (status == LoginStatus.Empty || status == LoginStatus.Logout)
            {
                IMSDK.Login(userId, token, (suc, err, errMsg) =>
                {
                    if (suc)
                    {
                        Debug.Log("Login UserId:" + IMSDK.GetLoginUser());
                    }
                    else
                    {
                    }
                });
            }
            else if (status == LoginStatus.Logged)
            {
                Debug.Log("Login UserId:" + IMSDK.GetLoginUser());
            }
        }
    }

    void Update()
    {
        IMSDK.Polling();
    }

    void OnDestroy()
    {
#if !UNITY_EDITOR
        IMSDK.UnInitSDK();
#endif
    }

    public class ConnListener : IConnListener
    {
        public void OnConnecting()
        {
        }

        public void OnConnectSuccess()
        {
        }

        public void OnConnectFailed(int errCode, string errMsg)
        {
        }

        public void OnKickedOffline()
        {
        }

        public void OnUserTokenExpired()
        {
        }

        public void OnUserTokenInvalid(string errMsg)
        {
        }
    }
    public class ConversationListener : IConversationListener
    {
        public void OnConversationChanged(List<LocalConversation> conversationList)
        {
        }

        public void OnConversationUserInputStatusChanged(InputStatesChangedData data)
        {
        }

        public void OnNewConversation(List<LocalConversation> conversationList)
        {
        }

        public void OnSyncServerFailed()
        {
        }

        public void OnSyncServerFinish()
        {
        }

        public void OnSyncServerProgress(int progress)
        {
        }

        public void OnSyncServerStart()
        {
        }

        public void OnTotalUnreadMessageCountChanged(int totalUnreadCount)
        {
        }
    }
    public class GroupListener : IGroupListener
    {
        public void OnGroupApplicationAccepted(LocalGroupRequest groupApplication)
        {
        }

        public void OnGroupApplicationAdded(LocalGroupRequest groupApplication)
        {
        }

        public void OnGroupApplicationDeleted(LocalGroupRequest groupApplication)
        {
        }

        public void OnGroupApplicationRejected(LocalGroupRequest groupApplication)
        {
        }

        public void OnGroupDismissed(LocalGroup groupInfo)
        {
        }

        public void OnGroupInfoChanged(LocalGroup groupInfo)
        {
        }

        public void OnGroupMemberAdded(LocalGroupMember groupMemberInfo)
        {
        }

        public void OnGroupMemberDeleted(LocalGroupMember groupMemberInfo)
        {
        }

        public void OnGroupMemberInfoChanged(LocalGroupMember groupMemberInfo)
        {
        }

        public void OnJoinedGroupAdded(LocalGroup groupInfo)
        {
        }

        public void OnJoinedGroupDeleted(LocalGroup groupInfo)
        {
        }
    }
    public class FriendShipListener : IFriendShipListener
    {
        public void OnBlackAdded(LocalBlack blackInfo)
        {
        }

        public void OnBlackDeleted(LocalBlack blackInfo)
        {
        }

        public void OnFriendAdded(LocalFriend friendInfo)
        {
        }

        public void OnFriendApplicationAccepted(LocalFriendRequest friendApplication)
        {
        }

        public void OnFriendApplicationAdded(LocalFriendRequest friendApplication)
        {
        }

        public void OnFriendApplicationDeleted(LocalFriendRequest friendApplication)
        {
        }

        public void OnFriendApplicationRejected(LocalFriendRequest friendApplication)
        {
        }

        public void OnFriendDeleted(LocalFriend friendInfo)
        {
        }

        public void OnFriendInfoChanged(LocalFriend friendInfo)
        {
        }
    }
    public class AdvancedMsgListener : IAdvancedMsgListener
    {
        public void OnMsgDeleted(MsgStruct message)
        {
        }

        public void OnNewRecvMessageRevoked(MessageRevoked messageRevoked)
        {
        }

        public void OnRecvC2CReadReceipt(List<MessageReceipt> msgReceiptList)
        {
        }

        public void OnRecvGroupReadReceipt(List<MessageReceipt> groupMsgReceiptList)
        {
        }

        public void OnRecvMessageExtensionsAdded(string msgID, string reactionExtensionList)
        {
        }

        public void OnRecvMessageExtensionsChanged(string msgID, string reactionExtensionList)
        {
        }

        public void OnRecvMessageExtensionsDeleted(string msgID, string reactionExtensionKeyList)
        {
        }

        public void OnRecvNewMessage(MsgStruct message)
        {
        }

        public void OnRecvOfflineNewMessage(MsgStruct message)
        {
        }

        public void OnRecvOnlineOnlyMessage(MsgStruct message)
        {
        }
    }
    public class UserListener : IUserListener
    {
        public void OnSelfInfoUpdated(LocalUser userInfo)
        {
        }

        public void OnUserCommandAdd(string userCommand)
        {
        }

        public void OnUserCommandDelete(string userCommand)
        {
        }

        public void OnUserCommandUpdate(string userCommand)
        {
        }

        public void OnUserStatusChanged(OnlineStatus userOnlineStatus)
        {
        }
    }
    public class CustomBusinessListener : ICustomBusinessListener
    {
        public void OnRecvCustomBusinessMessage(string businessMessage)
        {
        }
    }
    public class BatchMsgListener : IBatchMsgListener
    {
        public void OnRecvNewMessages(List<MsgStruct> messageList)
        {
        }

        public void OnRecvOfflineNewMessages(List<MsgStruct> messageList)
        {
        }
    }
}
