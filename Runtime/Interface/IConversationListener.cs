using System.Collections.Generic;

namespace OpenIM.IMSDK.Unity.Listener
{
    public interface IConversationListener
    {
        void OnSyncServerStart();
        void OnSyncServerFinish();
        void OnSyncServerProgress(int progress);
        void OnSyncServerFailed();
        void OnNewConversation(List<LocalConversation> conversationList);
        void OnConversationChanged(List<LocalConversation> conversationList);
        void OnTotalUnreadMessageCountChanged(int totalUnreadCount);
        void OnConversationUserInputStatusChanged(InputStatesChangedData data);
    }
}