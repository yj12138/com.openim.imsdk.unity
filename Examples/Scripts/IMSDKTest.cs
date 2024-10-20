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
        initRes = IMSDK.InitSDK(config, new ConnListener());
        IMSDK.SetConversationListener(new ConversatioListener(this));
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
                        Debug.Log("Login UserId:" + IMSDK.GetLoginUserId());
                        GetData();
                    }
                });
            }
            else if (status == LoginStatus.Logged)
            {
                Debug.Log("Login UserId:" + IMSDK.GetLoginUserId());
                GetData();
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

    public void GetData()
    {
        IMSDK.GetAllConversationList((list, errCode, errMsg) =>
        {
            if (list != null)
            {
                Debug.Log("Conversation Count:" + list.Count);
            }
        });

        IMSDK.GetFriendList((friendList, errCode, errMsg) =>
        {
            if (friendList != null)
            {
                Debug.Log("Friend Count:" + friendList.Count);
            }
        }, true);
        IMSDK.GetJoinedGroupList((groupList, errCode, errMsg) =>
        {
            if (groupList != null)
            {
                Debug.Log("Group Count:" + groupList.Count);
            }
        });
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
    public class ConversatioListener : IConversationListener
    {
        IMSDKTest test;
        public ConversatioListener(IMSDKTest test)
        {
            this.test = test;
        }

        public void OnConversationChanged(List<Conversation> conversationList)
        {
        }

        public void OnConversationUserInputStatusChanged(InputStatesChangedData data)
        {
        }

        public void OnNewConversation(List<Conversation> conversationList)
        {
        }

        public void OnSyncServerFailed()
        {
        }

        public void OnSyncServerFinish()
        {
            this.test.GetData();
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
}
