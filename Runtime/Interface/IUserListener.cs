namespace OpenIM.IMSDK.Unity.Listener
{
    public interface IUserListener
    {
        void OnSelfInfoUpdated(LocalUser userInfo);
        void OnUserStatusChanged(OnlineStatus userOnlineStatus);
        void OnUserCommandAdd(string userCommand);
        void OnUserCommandDelete(string userCommand);
        void OnUserCommandUpdate(string userCommand);
    }
}