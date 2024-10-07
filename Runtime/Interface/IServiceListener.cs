namespace OpenIM.IMSDK.Unity.Listener
{
    public interface IServiceListener
    {
        void OnGroupApplicationAdded(LocalGroupRequest groupApplication);
        void OnGroupApplicationAccepted(LocalAdminGroupRequest groupApplication);
        void OnFriendApplicationAdded(LocalFriendRequest friendApplication);
        void OnFriendApplicationAccepted(LocalFriendRequest groupApplication);
        void OnRecvNewMessage(MsgStruct message);
    }
}