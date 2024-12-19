using OpenIM.Proto;

namespace OpenIM.IMSDK.Listener
{
    public interface IConversationListener
    {
        void OnSyncServerStart(bool reinstalled);
        void OnSyncServerFinish(bool reinstalled);
        void OnSyncServerProgress(int progress);
        void OnSyncServerFailed(bool reinstalled);
        void OnNewConversation(IMConversation[] conversations);
        void OnConversationChanged(IMConversation[] conversations);
        void OnTotalUnreadMessageCountChanged(int totalUnreadCount);
        void OnConversationUserInputStatusChanged(string conversationId, string userId, Platform[] platforms);
    }
}