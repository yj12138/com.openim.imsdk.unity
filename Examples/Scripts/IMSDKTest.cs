using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OpenIM.IMSDK.Unity;

public class IMSDKTest : MonoBehaviour, IConnCallBack
{
    public string wsAddr = "ws://192.168.101.9:10001";
    public string apiAddr = "http://192.168.101.9:10002";
    public string userId = "test";
    public string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySUQiOiJ0ZXN0IiwiUGxhdGZvcm1JRCI6MywiZXhwIjoxNzM2MDgyMzk4LCJuYmYiOjE3MjgzMDYwOTgsImlhdCI6MTcyODMwNjM5OH0.1nxduLgJe5YKvlDwJr-bmV2JWWVkGegfvbBw5Do5oGM";
    void Start()
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
        var suc = IMSDK.InitSDK(config, this);
        Debug.Log("InitSDK :" + suc);

        if (suc)
        {
            IMSDK.Login(userId, token, (suc, err, errMsg) =>
            {
                if (suc)
                {
                    Debug.Log("Login Suc");
                }
                else
                {
                    Debug.Log("Login Failed:" + errMsg);
                }
            });
        }
    }

    void Update()
    {
        IMSDK.Polling();
    }

    void OnDestroy()
    {
        IMSDK.UnInitSDK();
    }

    public void OnConnecting()
    {
        Debug.Log("OnConnecting");
    }

    public void OnConnectSuccess()
    {
        Debug.Log("OnConnectSuccess");
    }

    public void OnConnectFailed(int errCode, string errMsg)
    {
        Debug.Log("OnConnectFailed:" + errCode + ":" + errMsg);
    }

    public void OnKickedOffline()
    {
        Debug.Log("OnKickedOffline");
    }

    public void OnUserTokenExpired()
    {
        Debug.Log("OnUserTokenExpired");
    }

    public void OnUserTokenInvalid(string errMsg)
    {
        Debug.Log("OnUserTokenInvalid:" + errMsg);
    }
}
