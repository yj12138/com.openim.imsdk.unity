using OpenIM.Proto;

namespace OpenIM.IMSDK.Listener
{
    public interface IFriendShipListener
    {
        void OnFriendApplicationAdded(IMFriendApplication friendApplication);
        void OnFriendApplicationDeleted(IMFriendApplication friendApplication);
        void OnFriendApplicationAccepted(IMFriendApplication friendApplication);
        void OnFriendApplicationRejected(IMFriendApplication friendApplication);
        void OnFriendAdded(IMFriend friend);
        void OnFriendDeleted(IMFriend friend);
        void OnFriendInfoChanged(IMFriend friend);
        void OnBlackAdded(IMBlack black);
        void OnBlackDeleted(IMBlack black);
    }
}