using OpenIM.Proto;

namespace OpenIM.IMSDK.Listener
{
    public interface IGroupListener
    {
        void OnJoinedGroupAdded(IMGroup groupInfo);
        void OnJoinedGroupDeleted(IMGroup groupInfo);
        void OnGroupMemberAdded(IMGroupMember groupMemberInfo);
        void OnGroupMemberDeleted(IMGroupMember groupMemberInfo);
        void OnGroupApplicationAdded(IMGroupApplication groupApplication);
        void OnGroupApplicationDeleted(IMGroupApplication groupApplication);
        void OnGroupInfoChanged(IMGroup groupInfo);
        void OnGroupDismissed(IMGroup groupInfo);
        void OnGroupMemberInfoChanged(IMGroupMember groupMemberInfo);
        void OnGroupApplicationAccepted(IMGroupApplication groupApplication);
        void OnGroupApplicationRejected(IMGroupApplication groupApplication);
    }
}