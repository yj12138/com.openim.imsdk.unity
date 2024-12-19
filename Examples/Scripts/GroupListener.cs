using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OpenIM.Proto;
using OpenIM.IMSDK;
using OpenIM.IMSDK.Listener;
public class GroupListener : IGroupListener
{
    public void OnGroupApplicationAccepted(IMGroupApplication groupApplication)
    {
        Debug.Log("OnGroupApplicationAccepted:" + groupApplication.ToString());
    }

    public void OnGroupApplicationAdded(IMGroupApplication groupApplication)
    {
        Debug.Log("OnGroupApplicationAdded:" + groupApplication.ToString());
    }

    public void OnGroupApplicationDeleted(IMGroupApplication groupApplication)
    {
        Debug.Log("OnGroupApplicationDeleted:" + groupApplication.ToString());
    }

    public void OnGroupApplicationRejected(IMGroupApplication groupApplication)
    {
        Debug.Log("OnGroupApplicationRejected:" + groupApplication.ToString());
    }

    public void OnGroupDismissed(IMGroup groupInfo)
    {
        Debug.Log("OnGroupDismissed:" + groupInfo.ToString());
    }

    public void OnGroupInfoChanged(IMGroup groupInfo)
    {
        Debug.Log("OnGroupInfoChanged:" + groupInfo.ToString());
    }
    public void OnJoinedGroupAdded(IMGroup groupInfo)
    {
        Debug.Log("OnJoinedGroupAdded:" + groupInfo.ToString());
    }

    public void OnJoinedGroupDeleted(IMGroup groupInfo)
    {
        Debug.Log("OnJoinedGroupDeleted:" + groupInfo.ToString());
    }
    public void OnGroupMemberAdded(IMGroupMember groupMemberInfo)
    {
        Debug.Log("OnGroupMemberAdded:" + groupMemberInfo.ToString());
    }

    public void OnGroupMemberDeleted(IMGroupMember groupMemberInfo)
    {
        Debug.Log("OnGroupMemberDeleted:" + groupMemberInfo.ToString());
    }

    public void OnGroupMemberInfoChanged(IMGroupMember groupMemberInfo)
    {
        Debug.Log("OnGroupMemberInfoChanged:" + groupMemberInfo.ToString());
    }


}
