namespace OpenIM.IMSDK.Unity.Listener
{
    public interface IConnListener
    {
        void OnConnecting();
        void OnConnectSuccess();
        void OnConnectFailed(int errCode, string errMsg);
        void OnKickedOffline();
        void OnUserTokenExpired();
        void OnUserTokenInvalid(string errMsg);
    }
}