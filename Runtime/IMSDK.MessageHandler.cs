using OpenIM.IMSDK.Unity.Native;
using Newtonsoft.Json;
using OpenIM.IMSDK.Unity.Util;
using System.Collections.Generic;
using System;
using System.Collections.Concurrent;
using AOT;

namespace OpenIM.IMSDK.Unity
{
    internal class Error
    {
        [JsonProperty("errCode")]
        public int ErrCode;
        [JsonProperty("errMsg")]
        public string ErrMsg;
        [JsonProperty("operationId")]
        public string OperationId;
    }
    internal class Success
    {
        [JsonProperty("operationId")]
        public string OperationId;
        [JsonProperty("dataType")]
        public int DataType;
        [JsonProperty("data")]
        public string Data;
    }
    internal class ErrorOrSuccess
    {
        [JsonProperty("operationId")]
        public string OperationId;
        [JsonProperty("errCode")]
        public int ErrCode;
        [JsonProperty("errMsg")]
        public string ErrMsg;
        [JsonProperty("dataType")]
        public int DataType;
        [JsonProperty("data")]
        public string Data;
    }

    internal class Progress
    {
        [JsonProperty("operationId")]
        public string OperationId;
        [JsonProperty("progress")]
        public int _Progress;
    }

    internal class MsgIDAndList
    {
        [JsonProperty("msgId")]
        public string Id;
        [JsonProperty("list")]
        public string List;
    }
    internal struct IdMsg
    {
        public int Id;
        public string Data;
    }


    public partial class IMSDK
    {

        static ConcurrentQueue<IdMsg> msgCache = new ConcurrentQueue<IdMsg>();
        [MonoPInvokeCallback(typeof(MessageHandler))]
        private static void MessageHandler(int msgId, string msg)
        {
            var idmsg = new IdMsg
            {
                Id = msgId,
                Data = msg
            };
            msgCache.Enqueue(idmsg);
        }
        public static void DispatorMsg(MessageDef id, string msg)
        {
            switch (id)
            {
                case MessageDef.Msg_Connecting:
                    listenGroup.ConnListener.OnConnecting();
                    break;
                case MessageDef.Msg_ConnectSuccess:
                    listenGroup.ConnListener.OnConnectSuccess();
                    break;
                case MessageDef.Msg_ConnectFailed:
                    {
                        Error err = Utils.FromJson<Error>(msg);
                        listenGroup.ConnListener.OnConnectFailed(err.ErrCode, err.ErrMsg);
                        break;
                    }
                case MessageDef.Msg_KickedOffline:
                    listenGroup.ConnListener.OnKickedOffline();
                    break;
                case MessageDef.Msg_UserTokenExpired:
                    listenGroup.ConnListener.OnUserTokenExpired();
                    break;
                case MessageDef.Msg_UserTokenInvalid:
                    {
                        Error err = Utils.FromJson<Error>(msg);
                        listenGroup.ConnListener.OnUserTokenInvalid(err.ErrMsg);
                        break;
                    }
                case MessageDef.Msg_SyncServerStart:
                    listenGroup.ConversationListener.OnSyncServerStart();
                    break;
                case MessageDef.Msg_SyncServerFinish:
                    listenGroup.ConversationListener.OnSyncServerFinish();
                    break;
                case MessageDef.Msg_SyncServerProgress:
                    {
                        var progress = Utils.FromJson<Progress>(msg);
                        listenGroup.ConversationListener.OnSyncServerProgress(progress._Progress);
                        break;
                    }
                case MessageDef.Msg_SyncServerFailed:
                    listenGroup.ConversationListener.OnSyncServerFailed();
                    break;
                case MessageDef.Msg_NewConversation:
                    {
                        var data = Utils.FromJson<List<LocalConversation>>(msg);
                        listenGroup.ConversationListener.OnNewConversation(data);
                        break;
                    }
                case MessageDef.Msg_ConversationChanged:
                    {
                        var data = Utils.FromJson<List<LocalConversation>>(msg);
                        listenGroup.ConversationListener.OnConversationChanged(data);
                        break;
                    }
                case MessageDef.Msg_TotalUnreadMessageCountChanged:
                    if (int.TryParse(msg, out int count))
                    {
                        listenGroup.ConversationListener.OnTotalUnreadMessageCountChanged(count);
                    }
                    break;
                case MessageDef.Msg_ConversationUserInputStatusChanged:
                    {
                        var data = Utils.FromJson<InputStatesChangedData>(msg);
                        listenGroup.ConversationListener.OnConversationUserInputStatusChanged(data);
                    }
                    break;
                case MessageDef.Msg_Advanced_RecvNewMessage:
                    {
                        var data = Utils.FromJson<MsgStruct>(msg);
                        listenGroup.AdvancedMsgListener.OnRecvNewMessage(data);
                    }
                    break;
                case MessageDef.Msg_Advanced_RecvC2CReadReceipt:
                    {
                        var data = Utils.FromJson<List<MessageReceipt>>(msg);
                        listenGroup.AdvancedMsgListener.OnRecvC2CReadReceipt(data);
                    }
                    break;
                case MessageDef.Msg_Advanced_RecvGroupReadReceipt:
                    {
                        var data = Utils.FromJson<List<MessageReceipt>>(msg);
                        listenGroup.AdvancedMsgListener.OnRecvGroupReadReceipt(data);
                    }
                    break;
                case MessageDef.Msg_Advanced_NewRecvMessageRevoked:
                    {
                        var data = Utils.FromJson<MessageRevoked>(msg);
                        listenGroup.AdvancedMsgListener.OnNewRecvMessageRevoked(data);
                    }
                    break;
                case MessageDef.Msg_Advanced_RecvMessageExtensionsChanged:
                    {
                        var data = Utils.FromJson<MsgIDAndList>(msg);
                        listenGroup.AdvancedMsgListener.OnRecvMessageExtensionsChanged(data.Id, data.List);
                    }
                    break;
                case MessageDef.Msg_Advanced_RecvMessageExtensionsDeleted:
                    {
                        var data = Utils.FromJson<MsgIDAndList>(msg);
                        listenGroup.AdvancedMsgListener.OnRecvMessageExtensionsDeleted(data.Id, data.List);
                    }
                    break;
                case MessageDef.Msg_Advanced_RecvMessageExtensionsAdded:
                    {
                        var data = Utils.FromJson<MsgIDAndList>(msg);
                        listenGroup.AdvancedMsgListener.OnRecvMessageExtensionsAdded(data.Id, data.List);
                    }
                    break;
                case MessageDef.Msg_Advanced_RecvOfflineNewMessage:
                    {
                        var data = Utils.FromJson<MsgStruct>(msg);
                        listenGroup.AdvancedMsgListener.OnRecvOfflineNewMessage(data);
                    }
                    break;
                case MessageDef.Msg_Advanced_MsgDeleted:
                    {
                        var data = Utils.FromJson<MsgStruct>(msg);
                        listenGroup.AdvancedMsgListener.OnMsgDeleted(data);
                    }
                    break;
                case MessageDef.Msg_Advanced_RecvOnlineOnlyMessage:
                    {
                        var data = Utils.FromJson<MsgStruct>(msg);
                        listenGroup.AdvancedMsgListener.OnRecvOnlineOnlyMessage(data);
                    }
                    break;
                case MessageDef.Msg_Batch_RecvNewMessages:
                    {
                        var data = Utils.FromJson<List<MsgStruct>>(msg);
                        listenGroup.BatchMsgListener.OnRecvNewMessages(data);
                    }
                    break;
                case MessageDef.Msg_Batch_RecvOfflineNewMessages:
                    {
                        var data = Utils.FromJson<List<MsgStruct>>(msg);
                        listenGroup.BatchMsgListener.OnRecvOfflineNewMessages(data);
                    }
                    break;
                case MessageDef.Msg_FriendApplicationAdded:
                    {
                        var data = Utils.FromJson<LocalFriendRequest>(msg);
                        listenGroup.FriendShipListener.OnFriendApplicationAdded(data);
                    }
                    break;
                case MessageDef.Msg_FriendApplicationDeleted:
                    {
                        var data = Utils.FromJson<LocalFriendRequest>(msg);
                        listenGroup.FriendShipListener.OnFriendApplicationDeleted(data);
                    }
                    break;
                case MessageDef.Msg_FriendApplicationAccepted:
                    {
                        var data = Utils.FromJson<LocalFriendRequest>(msg);
                        listenGroup.FriendShipListener.OnFriendApplicationAccepted(data);
                    }
                    break;
                case MessageDef.Msg_FriendApplicationRejected:
                    {
                        var data = Utils.FromJson<LocalFriendRequest>(msg);
                        listenGroup.FriendShipListener.OnFriendApplicationRejected(data);
                    }
                    break;
                case MessageDef.Msg_FriendAdded:
                    {
                        var data = Utils.FromJson<LocalFriend>(msg);
                        listenGroup.FriendShipListener.OnFriendAdded(data);
                    }
                    break;
                case MessageDef.Msg_FriendDeleted:
                    {
                        var data = Utils.FromJson<LocalFriend>(msg);
                        listenGroup.FriendShipListener.OnFriendDeleted(data);
                    }
                    break;
                case MessageDef.Msg_FriendInfoChanged:
                    {
                        var data = Utils.FromJson<LocalFriend>(msg);
                        listenGroup.FriendShipListener.OnFriendInfoChanged(data);
                    }
                    break;
                case MessageDef.Msg_BlackAdded:
                    {
                        var data = Utils.FromJson<LocalBlack>(msg);
                        listenGroup.FriendShipListener.OnBlackAdded(data);
                    }
                    break;
                case MessageDef.Msg_BlackDeleted:
                    {
                        var data = Utils.FromJson<LocalBlack>(msg);
                        listenGroup.FriendShipListener.OnBlackDeleted(data);
                    }
                    break;
                case MessageDef.Msg_JoinedGroupAdded:
                    {
                        var data = Utils.FromJson<LocalGroup>(msg);
                        listenGroup.GroupListener.OnJoinedGroupAdded(data);
                    }
                    break;
                case MessageDef.Msg_JoinedGroupDeleted:
                    {
                        var data = Utils.FromJson<LocalGroup>(msg);
                        listenGroup.GroupListener.OnJoinedGroupDeleted(data);
                    }
                    break;
                case MessageDef.Msg_GroupMemberAdded:
                    {
                        var data = Utils.FromJson<LocalGroupMember>(msg);
                        listenGroup.GroupListener.OnGroupMemberAdded(data);
                    }
                    break;
                case MessageDef.Msg_GroupMemberDeleted:
                    {
                        var data = Utils.FromJson<LocalGroupMember>(msg);
                        listenGroup.GroupListener.OnGroupMemberDeleted(data);
                    }
                    break;
                case MessageDef.Msg_GroupApplicationAdded:
                    {
                        var data = Utils.FromJson<LocalGroupRequest>(msg);
                        listenGroup.GroupListener.OnGroupApplicationAdded(data);
                    }
                    break;
                case MessageDef.Msg_GroupApplicationDeleted:
                    {
                        var data = Utils.FromJson<LocalGroupRequest>(msg);
                        listenGroup.GroupListener.OnGroupApplicationDeleted(data);
                    }
                    break;
                case MessageDef.Msg_GroupInfoChanged:
                    {
                        var data = Utils.FromJson<LocalGroup>(msg);
                        listenGroup.GroupListener.OnGroupInfoChanged(data);
                    }
                    break;
                case MessageDef.Msg_GroupDismissed:
                    {
                        var data = Utils.FromJson<LocalGroup>(msg);
                        listenGroup.GroupListener.OnGroupDismissed(data);
                    }
                    break;
                case MessageDef.Msg_GroupMemberInfoChanged:
                    {
                        var data = Utils.FromJson<LocalGroupMember>(msg);
                        listenGroup.GroupListener.OnGroupMemberInfoChanged(data);
                    }
                    break;
                case MessageDef.Msg_GroupApplicationAccepted:
                    {
                        var data = Utils.FromJson<LocalGroupRequest>(msg);
                        listenGroup.GroupListener.OnGroupApplicationAccepted(data);
                    }
                    break;
                case MessageDef.Msg_GroupApplicationRejected:
                    {
                        var data = Utils.FromJson<LocalGroupRequest>(msg);
                        listenGroup.GroupListener.OnGroupApplicationRejected(data);
                    }
                    break;
                case MessageDef.Msg_RecvCustomBusinessMessage:
                    {
                        listenGroup.CustomBusinessListener.OnRecvCustomBusinessMessage(msg);
                    }
                    break;
                case MessageDef.Msg_SelfInfoUpdated:
                    {
                        var data = Utils.FromJson<LocalUser>(msg);
                        listenGroup.UserListener.OnSelfInfoUpdated(data);
                    }
                    break;
                case MessageDef.Msg_UserStatusChanged:
                    {
                        var data = Utils.FromJson<OnlineStatus>(msg);
                        listenGroup.UserListener.OnUserStatusChanged(data);
                    }
                    break;
                case MessageDef.Msg_UserCommandAdd:
                    {
                        listenGroup.UserListener.OnUserCommandAdd(msg);
                        break;
                    }
                case MessageDef.Msg_UserCommandDelete:
                    {
                        listenGroup.UserListener.OnUserCommandDelete(msg);
                        break;
                    }
                case MessageDef.Msg_UserCommandUpdate:
                    {
                        listenGroup.UserListener.OnUserCommandUpdate(msg);
                        break;
                    }
                case MessageDef.Msg_SendMessage_Error:
                    {
                        var data = Utils.FromJson<Error>(msg);
                        if (callBackDic.TryGetValue(data.OperationId, out Delegate func))
                        {
                            if (func is OnSendMessage)
                            {
                                func.DynamicInvoke(null, data.ErrCode, data.ErrMsg);
                            }
                            callBackDic.Remove(data.OperationId);
                        }
                    }
                    break;
                case MessageDef.Msg_SendMessage_Success:
                    {
                        var data = Utils.FromJson<Success>(msg);
                        MsgStruct msgStruct = ConvertDataType(data.DataType, data.Data) as MsgStruct;
                        if (callBackDic.TryGetValue(data.OperationId, out Delegate func))
                        {
                            if (func is OnSendMessage)
                            {
                                func.DynamicInvoke(msgStruct, 0, "");
                            }
                            callBackDic.Remove(data.OperationId);
                        }
                    }
                    break;
                case MessageDef.Msg_SendMessage_Progress:
                    {
                        // var data = Utils.FromJson<Progress>(msg);
                    }
                    break;
                case MessageDef.Msg_ErrorOrSuc:
                    DispatorErrorOrSucMsg(Utils.FromJson<ErrorOrSuccess>(msg));
                    break;
            }
        }
        private static void DispatorErrorOrSucMsg(ErrorOrSuccess msg)
        {
            if (callBackDic.TryGetValue(msg.OperationId, out Delegate func))
            {
                if (func is OnBase)
                {
                    if (msg.ErrCode >= 0)
                    {
                        func.DynamicInvoke(false, msg.ErrCode, msg.ErrMsg);
                    }
                    else
                    {
                        func.DynamicInvoke(true, 0, "");
                    }
                }
                else if (func is OnInt || func is OnBool)
                {
                    func.DynamicInvoke(ConvertDataType(msg.DataType, msg.Data));
                }
                else
                {
                    if (msg.ErrCode >= 0)
                    {
                        func.DynamicInvoke(null, msg.ErrCode, msg.ErrMsg);
                    }
                    else
                    {
                        func.DynamicInvoke(ConvertDataType(msg.DataType, msg.Data), 0, "");
                    }
                }
                callBackDic.Remove(msg.OperationId);
            }
        }
        private static object ConvertDataType(int type, string msg)
        {
            switch ((DataTypeDef)type)
            {
                case DataTypeDef.DataType_Empty:
                    return null;
                case DataTypeDef.DataType_Int:
                    return Utils.FromJson<int>(msg);
                case DataTypeDef.DataType_Bool:
                    return Utils.FromJson<bool>(msg);
                case DataTypeDef.DataType_LocalConversation:
                    return Utils.FromJson<LocalConversation>(msg);
                case DataTypeDef.DataType_LocalConversation_List:
                    return Utils.FromJson<List<LocalConversation>>(msg);
                case DataTypeDef.DataType_GetConversationRecvMessageOptResp_List:
                    return Utils.FromJson<List<GetConversationRecvMessageOptResp>>(msg);
                case DataTypeDef.DataType_FindMessageList:
                    return Utils.FromJson<FindMessageList>(msg);
                case DataTypeDef.DataType_GetAdvancedHistoryMessageList:
                    return Utils.FromJson<GetAdvancedHistoryMessageList>(msg);
                case DataTypeDef.DataType_MsgStruct:
                    return Utils.FromJson<MsgStruct>(msg);
                case DataTypeDef.DataType_SearchLocalMessagesCallback:
                    return Utils.FromJson<int>(msg);
                case DataTypeDef.DataType_FullUserInfo:
                    return Utils.FromJson<int>(msg);
                case DataTypeDef.DataType_FullUserInfo_List:
                    return Utils.FromJson<List<FullUserInfo>>(msg);
                case DataTypeDef.DataType_FullUserInfoWithCache:
                    return Utils.FromJson<FullUserInfoWithCache>(msg);
                case DataTypeDef.DataType_FullUserInfoWithCache_List:
                    return Utils.FromJson<List<FullUserInfoWithCache>>(msg);
                case DataTypeDef.DataType_LocalUser:
                    return Utils.FromJson<LocalUser>(msg);
                case DataTypeDef.DataType_LocalUser_List:
                    return Utils.FromJson<List<LocalUser>>(msg);
                case DataTypeDef.DataType_OnlineStatus:
                    return Utils.FromJson<OnlineStatus>(msg);
                case DataTypeDef.DataType_OnlineStatus_List:
                    return Utils.FromJson<List<OnlineStatus>>(msg);
                case DataTypeDef.DataType_SearchFriendItem:
                    return Utils.FromJson<SearchFriendItem>(msg);
                case DataTypeDef.DataType_SearchFriendItem_List:
                    return Utils.FromJson<List<SearchFriendItem>>(msg);
                case DataTypeDef.DataType_UserIDResult:
                    return Utils.FromJson<UserIDResult>(msg);
                case DataTypeDef.DataType_UserIDResult_List:
                    return Utils.FromJson<List<UserIDResult>>(msg);
                case DataTypeDef.DataType_LocalFriendRequest:
                    return Utils.FromJson<LocalFriendRequest>(msg);
                case DataTypeDef.DataType_LocalFriendRequest_List:
                    return Utils.FromJson<List<LocalFriendRequest>>(msg);
                case DataTypeDef.DataType_LocalBlack:
                    return Utils.FromJson<LocalBlack>(msg);
                case DataTypeDef.DataType_LocalBlack_List:
                    return Utils.FromJson<List<LocalBlack>>(msg);
                case DataTypeDef.DataType_GroupInfo:
                    return Utils.FromJson<LocalGroup>(msg);
                case DataTypeDef.DataType_LocalGroup:
                    return Utils.FromJson<LocalGroup>(msg);
                case DataTypeDef.DataType_LocalGroup_List:
                    return Utils.FromJson<List<LocalGroup>>(msg);
                case DataTypeDef.DataType_LocalGroupMember:
                    return Utils.FromJson<LocalGroupMember>(msg);
                case DataTypeDef.DataType_LocalGroupMember_List:
                    return Utils.FromJson<List<LocalGroupMember>>(msg);
                case DataTypeDef.DataType_LocalAdminGroupRequest:
                    return Utils.FromJson<LocalAdminGroupRequest>(msg);
                case DataTypeDef.DataType_LocalAdminGroupRequest_List:
                    return Utils.FromJson<List<LocalAdminGroupRequest>>(msg);
                case DataTypeDef.DataType_LocalGroupRequest:
                    return Utils.FromJson<LocalGroupRequest>(msg);
                case DataTypeDef.DataType_LocalGroupRequest_List:
                    return Utils.FromJson<List<LocalGroupRequest>>(msg);
            }
            return null;
        }
    }
}