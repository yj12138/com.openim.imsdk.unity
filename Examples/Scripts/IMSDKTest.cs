using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenIM.Proto;
using OpenIM.IMSDK;
using System.IO;

public class IMSDKTest : MonoBehaviour
{
    public string wsAddr = "ws://192.168.101.9:10001";
    public string apiAddr = "http://192.168.101.9:10002";
    public string userId = "test";
    public string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySUQiOiJ0ZXN0IiwiUGxhdGZvcm1JRCI6MywiZXhwIjoxNzM2MDgyMzk4LCJuYmYiOjE3MjgzMDYwOTgsImlhdCI6MTcyODMwNjM5OH0.1nxduLgJe5YKvlDwJr-bmV2JWWVkGegfvbBw5Do5oGM";
    void Awake()
    {
        IMSDK.SetErrorHandler((errCode, errMsg) =>
        {
            Debug.LogError(errMsg);
        });

        IMSDK.SetConnListener(new ConnListener());
        IMSDK.SetUserListener(new UserListener());
        IMSDK.SetFriendShipListener(new FriendShipListener());
        IMSDK.SetGroupListener(new GroupListener());
        IMSDK.SetConversationListener(new ConversatioListener());
        IMSDK.SetMessageListener(new MessageListener());

        var config = new IMConfig()
        {
            Platform = Platform.Windows,
            ApiAddr = apiAddr,
            WsAddr = wsAddr,
            DataDir = Path.Combine(Application.persistentDataPath, "OpenIM"),
            LogLevel = LogLevel.LevelDebug,
            IsLogStandardOutput = true,
            LogFilePath = Path.Combine(Application.persistentDataPath, "OpenIM/Logs"),
            DbPath = Path.Combine(Application.persistentDataPath, "OpenIM"),
        };

        IMSDK.InitSDK((suc) =>
        {
            Debug.Log("InitSDK:" + suc);
        }, config);
    }

    void Update()
    {
        IMSDK.Polling();
    }
    void OnGUI()
    {
        if (GUILayout.Button("Login"))
        {
            IMSDK.GetLoginStatus((status) =>
            {
                if (status == LoginStatus.Logged)
                {
                    Debug.Log("Logged");
                }
                else if (status == LoginStatus.Logging)
                {
                    Debug.Log("Logging");
                }
                else
                {
                    IMSDK.Login((suc) =>
                    {
                        if (suc)
                        {
                            Debug.Log("Login success");
                        }
                        else
                        {
                            {
                                Debug.Log("Login Failed");
                            }
                        }
                    }, userId, token);
                }
            });

        }
        if (GUILayout.Button("GetAllConversationList"))
        {
            IMSDK.GetAllConversationList((list) =>
            {
                if (list != null)
                {
                    Debug.Log("Conversation Count:" + list.Length);
                }
            });
        }
        if (GUILayout.Button("GetFriends"))
        {
            IMSDK.GetFriends((friendList) =>
            {
                if (friendList != null)
                {
                    Debug.Log("Friend Count:" + friendList.Length);
                }
            }, true);
        }
        if (GUILayout.Button("GetJoinedGroups"))
        {
            IMSDK.GetJoinedGroups((groupList) =>
            {
                if (groupList != null)
                {
                    Debug.Log("Group Count:" + groupList.Length);
                }
            });
        }
        if (GUILayout.Button("SendMessage"))
        {
            IMSDK.CreateTextMessage((msg) =>
            {
                IMSDK.SendMessage(new SendMsgCallBack(), msg, "test2", "", false);
            }, "Hello From Unity");
        }
        if (GUILayout.Button("Logout"))
        {
            IMSDK.Logout((suc) =>
            {
                Debug.Log("Logout:" + suc);
            });
        }
    }
}
