namespace OpenIM.IMSDK.Unity
{
    public interface IConnCallBack
    {
        void OnConnecting();
        void OnConnectSuccess();
        void OnConnectFailed(int errCode, string errMsg);
        void OnKickedOffline();
        void OnUserTokenExpired();
    }
}