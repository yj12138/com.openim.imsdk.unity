using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OpenIM.Proto;
using OpenIM.IMSDK;
using OpenIM.IMSDK.Listener;
public class UserListener : IUserListener
{
    public void OnSelfInfoUpdated(IMUser userInfo)
    {
        Debug.Log("OnSelfInfoUpdated" + userInfo.ToString());
    }

    public void OnUserCommandAdd(CommandInfo commandInfo)
    {
        Debug.Log("OnUserCommandAdd" + commandInfo.ToString());
    }

    public void OnUserCommandDelete(CommandInfo commandInfo)
    {
        Debug.Log("OnUserCommandDelete" + commandInfo.ToString());
    }

    public void OnUserCommandUpdate(CommandInfo commandInfo)
    {
        Debug.Log("OnUserCommandUpdate" + commandInfo.ToString());
    }

    public void OnUserOnlineStatusChanged(string userId, Platform[] platforms)
    {
        Debug.Log("OnUserOnlineStatusChanged" + userId);
    }
}
