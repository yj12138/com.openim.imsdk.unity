using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OpenIM.Proto;
using OpenIM.IMSDK;
using OpenIM.IMSDK.Listener;
public class SendMsgCallBack : ISendMsg
{
    public void OnError(int code, string errMsg)
    {
        Debug.LogError("SendMsg Error:" + errMsg);
    }

    public void OnProgress(long progress)
    {
        Debug.Log("SendMsg Progress:" + progress);
    }

    public void OnSuccess(IMMessage msg)
    {
        Debug.Log("SendMsg Success:" + msg.ToString());
    }
}
