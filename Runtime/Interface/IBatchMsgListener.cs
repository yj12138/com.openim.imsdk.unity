using System.Collections.Generic;

namespace OpenIM.IMSDK.Unity.Listener
{
    public interface IBatchMsgListener
    {
        void OnRecvNewMessages(List<Message> messageList);
        void OnRecvOfflineNewMessages(List<Message> messageList);
    }
}