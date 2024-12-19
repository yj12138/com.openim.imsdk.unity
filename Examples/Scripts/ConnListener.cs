using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenIM.Proto;
using OpenIM.IMSDK;
using OpenIM.IMSDK.Listener;
public class ConnListener : IConnListener
{
    public void OnConnecting()
    {
        Debug.Log("Connecting");
    }

    public void OnConnectSuccess()
    {
        Debug.Log("ConnectSuccess");
    }

    public void OnConnectFailed(int errCode, string errMsg)
    {
        Debug.Log(errCode + errMsg);
    }

    public void OnKickedOffline()
    {
        Debug.Log("KickedOffline");
    }

    public void OnUserTokenExpired()
    {
        Debug.Log("UserTokenExpired");
    }

    public void OnUserTokenInvalid(string errMsg)
    {
        Debug.Log("UserTokenInvalid:" + errMsg);
    }



}
