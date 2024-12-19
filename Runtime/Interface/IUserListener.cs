using OpenIM.Proto;

namespace OpenIM.IMSDK.Listener
{
    public interface IUserListener
    {
        void OnSelfInfoUpdated(IMUser userInfo);
        void OnUserOnlineStatusChanged(string userId, Platform[] platforms);
        void OnUserCommandAdd(CommandInfo commandInfo);
        void OnUserCommandDelete(CommandInfo commandInfo);
        void OnUserCommandUpdate(CommandInfo commandInfo);
    }
}