namespace OpenIM.IMSDK.Unity.Listener
{
    public interface IUserListener
    {
        void OnSelfInfoUpdated(LocalUser userInfo);
        void OnUserStatusChanged(OnlineStatus userOnlineStatus);
    }
}