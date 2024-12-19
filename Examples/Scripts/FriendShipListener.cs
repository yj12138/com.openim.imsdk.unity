using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OpenIM.Proto;
using OpenIM.IMSDK;
using OpenIM.IMSDK.Listener;

public class FriendShipListener : IFriendShipListener
{
    public void OnBlackAdded(IMBlack black)
    {
        Debug.Log("OnBlackAdded:" + black.ToString());
    }

    public void OnBlackDeleted(IMBlack black)
    {
        Debug.Log("OnBlackDeleted:" + black.ToString());
    }

    public void OnFriendAdded(IMFriend friend)
    {
        Debug.Log("OnFriendAdded:" + friend.ToString());
    }
    public void OnFriendDeleted(IMFriend friend)
    {
        Debug.Log("OnFriendDeleted:" + friend.ToString());
    }

    public void OnFriendInfoChanged(IMFriend friend)
    {
        Debug.Log("OnFriendInfoChanged:" + friend.ToString());
    }
    public void OnFriendApplicationAccepted(IMFriendApplication friendApplication)
    {
        Debug.Log("OnFriendApplicationAccepted:" + friendApplication.ToString());
    }

    public void OnFriendApplicationAdded(IMFriendApplication friendApplication)
    {
        Debug.Log("OnFriendApplicationAdded:" + friendApplication.ToString());
    }

    public void OnFriendApplicationDeleted(IMFriendApplication friendApplication)
    {
        Debug.Log("OnFriendApplicationDeleted:" + friendApplication.ToString());
    }

    public void OnFriendApplicationRejected(IMFriendApplication friendApplication)
    {
        Debug.Log("OnFriendApplicationRejected:" + friendApplication.ToString());
    }


}
