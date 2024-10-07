namespace OpenIM.IMSDK.Unity.Listener
{
    public interface ICustomBusinessListener
    {
        void OnRecvCustomBusinessMessage(string businessMessage);
    }
}