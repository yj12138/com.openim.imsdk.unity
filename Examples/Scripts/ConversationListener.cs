using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OpenIM.Proto;
using OpenIM.IMSDK;
using OpenIM.IMSDK.Listener;
public class ConversatioListener : IConversationListener
{
    public void OnConversationChanged(IMConversation[] conversations)
    {
        Debug.Log("ConversationChanged:" + conversations.Length);
    }

    public void OnConversationUserInputStatusChanged(string conversationId, string userId, Platform[] platforms)
    {
        Debug.Log("OnConversationUserInputStatusChanged:" + conversationId + " " + userId);
    }

    public void OnNewConversation(IMConversation[] conversations)
    {
        Debug.Log("OnNewConversation:" + conversations.Length);
    }

    public void OnSyncServerFailed(bool reinstalled)
    {
        Debug.Log("OnSyncServerFailed");
    }

    public void OnSyncServerFinish(bool reinstalled)
    {
        Debug.Log("OnSyncServerFinish");
    }

    public void OnSyncServerProgress(int progress)
    {
        Debug.Log("SyncServerProgress:" + progress);
    }

    public void OnSyncServerStart(bool reinstalled)
    {
        Debug.Log("OnSyncServerStart");
    }

    public void OnTotalUnreadMessageCountChanged(int totalUnreadCount)
    {
        Debug.Log("OnTotalUnreadMessageCountChanged:" + totalUnreadCount);
    }
}
