using System.Collections.Generic;

namespace OpenIM.IMSDK.Unity.Listener
{
    public interface IBatchMsgListener
    {
        void OnRecvNewMessages(List<MsgStruct> messageList);
        void OnRecvOfflineNewMessages(List<MsgStruct> messageList);
    }
}