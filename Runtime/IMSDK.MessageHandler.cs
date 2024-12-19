using Google.Protobuf;
using OpenIM.IMSDK.Native;
using OpenIM.IMSDK.Util;
using OpenIM.Proto;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.InteropServices;

namespace OpenIM.IMSDK
{
    public partial class IMSDK
    {
        static ConcurrentQueue<FfiResult> events = new ConcurrentQueue<FfiResult>();
        private static void OnRecvEvent(IntPtr dataPtr, int len)
        {
            try
            {
                byte[] byteArray = new byte[len];
                Marshal.Copy(dataPtr, byteArray, 0, len);
                var result = FfiResult.Parser.ParseFrom(byteArray);
                events.Enqueue(result);
            }
            catch (Exception e)
            {
                Utils.Log(e.ToString());
            }
        }
        static void HandleFFIResult(FfiResult result)
        {
            var errCode = result.ErrCode;
            var errMsg = result.ErrMsg;
            var handleId = result.HandleID;
            var funcName = result.FuncName;
            var data = result.Data;
            bool suc = errCode == 0;
            dispatchEvent(suc, handleId, funcName, data, errCode, errMsg);
            if (!suc)
            {
                errorHandler(errCode, errMsg);
            }
            NativeSDK.DropHandle(handleId);
        }
        static void dispatchEvent(bool suc, ulong handleId, FuncRequestEventName eventName, ByteString data, int errCode, string errMsg)
        {
            callBackDic.TryGetValue(handleId, out var callBack);
            switch (eventName)
            {
                case FuncRequestEventName.None:
                    break;
                case FuncRequestEventName.InitSdk:
                    {
                        var resp = InitSDKResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Suc);
                    }
                    break;

                case FuncRequestEventName.Login:
                    {
                        var resp = LoginResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.Logout:
                    {
                        var resp = LogoutResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.SetAppBackgroundStatus:
                    {
                        var resp = SetAppBackgroundStatusResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.NetworkStatusChanged:
                    {
                        var resp = NetworkStatusChangedResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.GetLoginStatus:
                    {
                        var resp = GetLoginStatusResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Status);
                    }
                    break;

                case FuncRequestEventName.Version:
                    {
                        var resp = VersionResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Version);
                    }
                    break;

                case FuncRequestEventName.SendMessage:
                    {
                        sendMsgCallBackDic.TryGetValue(handleId, out var msgCallBack);
                        if (msgCallBack != null)
                        {
                            var resp = SendMessageResp.Parser.ParseFrom(data);
                            if (suc)
                            {
                                msgCallBack.OnSuccess(resp.Message);
                            }
                            else
                            {
                                msgCallBack.OnError(errCode, errMsg);
                            }
                            sendMsgCallBackDic.Remove(handleId);
                        }
                    }
                    break;

                case FuncRequestEventName.UploadLogs:
                    {
                        uploadLogsCallBackDic.TryGetValue(handleId, out var uploadLogCallBack);
                        if (uploadLogCallBack != null)
                        {
                            var resp = UploadSDKDataResp.Parser.ParseFrom(data);
                            if (suc)
                            {
                                uploadLogCallBack.OnSuccess();
                            }
                            else
                            {
                                uploadLogCallBack.OnError(errCode, errMsg);
                            }
                            uploadLogsCallBackDic.Remove(handleId);
                        }
                    }
                    break;

                case FuncRequestEventName.UploadFile:
                    {
                        uploadFileCallBackDic.TryGetValue(handleId, out var uploadFileCallBack);
                        if (uploadFileCallBack != null)
                        {
                            var resp = UploadFileResp.Parser.ParseFrom(data);
                            if (suc)
                            {
                                uploadFileCallBack.OnSuccess(resp.Url);
                            }
                            else
                            {
                                uploadFileCallBack.OnError(errCode, errMsg);
                            }
                            uploadFileCallBackDic.Remove(handleId);
                        }
                    }
                    break;

                case FuncRequestEventName.Log:
                    break;

                case FuncRequestEventName.UpdateFcmToken:
                    break;

                case FuncRequestEventName.SetAppBadge:
                    break;

                case FuncRequestEventName.EventOnSendMsgProgress:
                    {
                        sendMsgCallBackDic.TryGetValue(handleId, out var msgCallBack);
                        if (msgCallBack != null)
                        {
                            var resp = EventOnSendMsgProgressData.Parser.ParseFrom(data);
                            msgCallBack.OnProgress(resp.Progress);
                        }
                    }
                    break;

                case FuncRequestEventName.EventOnUploadFileProgress:
                    {
                        uploadFileCallBackDic.TryGetValue(handleId, out var uploadFileCallBack);
                        if (uploadFileCallBack != null)
                        {
                            var resp = EventOnUploadFileProgressData.Parser.ParseFrom(data);
                            uploadFileCallBack.OnProgress(resp.Progress);
                        }
                    }

                    break;

                case FuncRequestEventName.EventOnUploadLogsProgress:
                    {
                        uploadLogsCallBackDic.TryGetValue(handleId, out var uploadLogCallBack);
                        if (uploadLogCallBack != null)
                        {
                            var resp = EventOnUploadFileProgressData.Parser.ParseFrom(data);
                            uploadLogCallBack.OnProgress(resp.Progress);
                        }
                    }
                    break;

                case FuncRequestEventName.EventOnConnecting:
                    {
                        var resp = EventOnConnectingData.Parser.ParseFrom(data);
                        connListener?.OnConnecting();
                    }
                    break;

                case FuncRequestEventName.EventOnConnectSuccess:
                    {
                        var resp = EventOnConnectSuccessData.Parser.ParseFrom(data);
                        connListener?.OnConnectSuccess();
                    }
                    break;

                case FuncRequestEventName.EventOnConnectFailed:
                    {
                        var resp = EventOnConnectFailedData.Parser.ParseFrom(data);
                        connListener?.OnConnectFailed(resp.ErrCode, resp.ErrMsg);
                    }
                    break;

                case FuncRequestEventName.EventOnKickedOffline:
                    {
                        var resp = EventOnKickedOfflineData.Parser.ParseFrom(data);
                        connListener?.OnKickedOffline();
                    }
                    break;

                case FuncRequestEventName.EventOnUserTokenExpired:
                    {
                        var resp = EventOnUserTokenExpiredData.Parser.ParseFrom(data);
                        connListener?.OnUserTokenExpired();
                    }
                    break;

                case FuncRequestEventName.EventOnUserTokenInvalid:
                    {
                        var resp = EventOnUserTokenInvalidData.Parser.ParseFrom(data);
                        connListener?.OnUserTokenInvalid(resp.ErrMsg);
                    }
                    break;

                case FuncRequestEventName.EventOnSyncServerStart:
                    {
                        var resp = EventOnSyncServerStartData.Parser.ParseFrom(data);
                        conversationListener?.OnSyncServerStart(resp.Reinstalled);
                    }
                    break;

                case FuncRequestEventName.EventOnSyncServerFinish:
                    {
                        var resp = EventOnSyncServerFinishData.Parser.ParseFrom(data);
                        conversationListener?.OnSyncServerFinish(resp.Reinstalled);
                    }
                    break;

                case FuncRequestEventName.EventOnSyncServerFailed:
                    {
                        var resp = EventOnSyncServerFailedData.Parser.ParseFrom(data);
                        conversationListener?.OnSyncServerFailed(resp.Reinstalled);
                    }
                    break;

                case FuncRequestEventName.EventOnSyncServerProgress:
                    {
                        var resp = EventOnSyncServerProgressData.Parser.ParseFrom(data);
                        conversationListener?.OnSyncServerProgress(resp.Progress);
                    }
                    break;

                case FuncRequestEventName.EventOnNewConversation:
                    {
                        var resp = EventOnNewConversationData.Parser.ParseFrom(data);
                        conversationListener?.OnNewConversation(resp.ConversationList.ToArray());
                    }
                    break;

                case FuncRequestEventName.EventOnConversationChanged:
                    {
                        var resp = EventOnConversationChangedData.Parser.ParseFrom(data);
                        conversationListener?.OnConversationChanged(resp.ConversationList.ToArray());
                    }
                    break;

                case FuncRequestEventName.EventOnTotalUnreadMessageCountChanged:
                    {
                        var resp = EventOnTotalUnreadMessageCountChangedData.Parser.ParseFrom(data);
                        conversationListener?.OnTotalUnreadMessageCountChanged(resp.TotalUnreadCount);
                    }
                    break;

                case FuncRequestEventName.EventOnConversationUserInputStatusChanged:
                    {
                        var resp = EventOnConversationUserInputStatusChangedData.Parser.ParseFrom(data);
                        conversationListener?.OnConversationUserInputStatusChanged(resp.ConversationID, resp.UserID, resp.Platforms.ToArray());
                    }
                    break;

                case FuncRequestEventName.EventOnRecvNewMessage:
                    {
                        var resp = EventOnRecvNewMessageData.Parser.ParseFrom(data);
                        messageListener?.OnRecvNewMessage(resp.Message);
                    }
                    break;

                case FuncRequestEventName.EventOnRecvC2CreadReceipt:
                    {
                        var resp = EventOnRecvC2CReadReceiptData.Parser.ParseFrom(data);
                        messageListener?.OnRecvC2CReadReceipt(resp.MsgReceiptList.ToArray());
                    }
                    break;

                case FuncRequestEventName.EventOnNewRecvMessageRevoked:
                    {
                        var resp = EventOnNewRecvMessageRevokedData.Parser.ParseFrom(data);
                        messageListener?.OnNewRecvMessageRevoked(resp.Revoked);
                    }
                    break;

                case FuncRequestEventName.EventOnRecvOfflineNewMessage:
                    {
                        var resp = EventOnRecvOfflineNewMessageData.Parser.ParseFrom(data);
                        messageListener?.OnRecvOfflineNewMessage(resp.Message);
                    }
                    break;

                case FuncRequestEventName.EventOnMessageDeleted:
                    {
                        var resp = EventOnMessageDeletedData.Parser.ParseFrom(data);
                        messageListener?.OnMessageDeleted(resp.Message);
                    }
                    break;

                case FuncRequestEventName.EventOnRecvOnlineOnlyMessage:
                    {
                        var resp = EventOnRecvOnlineOnlyMessageData.Parser.ParseFrom(data);
                        messageListener?.OnRecvOnlineOnlyMessage(resp.Message);
                    }
                    break;

                case FuncRequestEventName.EventOnMessageEdited:
                    {
                        var resp = EventOnMessageEditedData.Parser.ParseFrom(data);
                        messageListener?.OnMessageEdited(resp.Message);
                    }
                    break;

                case FuncRequestEventName.EventOnFriendApplicationAdded:
                    {
                        var resp = EventOnFriendApplicationAddedData.Parser.ParseFrom(data);
                        friendShipListener?.OnFriendApplicationAdded(resp.Application);
                    }
                    break;

                case FuncRequestEventName.EventOnFriendApplicationDeleted:
                    {
                        var resp = EventOnFriendApplicationDeletedData.Parser.ParseFrom(data);
                        friendShipListener?.OnFriendApplicationDeleted(resp.Application);
                    }
                    break;

                case FuncRequestEventName.EventOnFriendApplicationAccepted:
                    {
                        var resp = EventOnFriendApplicationAcceptedData.Parser.ParseFrom(data);
                        friendShipListener?.OnFriendApplicationAccepted(resp.Application);
                    }
                    break;

                case FuncRequestEventName.EventOnFriendApplicationRejected:
                    {
                        var resp = EventOnFriendApplicationRejectedData.Parser.ParseFrom(data);
                        friendShipListener?.OnFriendApplicationRejected(resp.Application);
                    }
                    break;

                case FuncRequestEventName.EventOnFriendAdded:
                    {
                        var resp = EventOnFriendAddedData.Parser.ParseFrom(data);
                        friendShipListener?.OnFriendAdded(resp.Friend);
                    }
                    break;

                case FuncRequestEventName.EventOnFriendDeleted:
                    {
                        var resp = EventOnFriendDeletedData.Parser.ParseFrom(data);
                        friendShipListener?.OnFriendDeleted(resp.Friend);
                    }
                    break;

                case FuncRequestEventName.EventOnFriendInfoChanged:
                    {
                        var resp = EventOnFriendInfoChangedData.Parser.ParseFrom(data);
                        friendShipListener?.OnFriendInfoChanged(resp.Friend);
                    }
                    break;

                case FuncRequestEventName.EventOnBlackAdded:
                    {
                        var resp = EventOnBlackAddedData.Parser.ParseFrom(data);
                        friendShipListener?.OnBlackAdded(resp.Black);
                    }
                    break;

                case FuncRequestEventName.EventOnBlackDeleted:
                    {
                        var resp = EventOnBlackDeletedData.Parser.ParseFrom(data);
                        friendShipListener?.OnBlackDeleted(resp.Black);
                    }
                    break;

                case FuncRequestEventName.EventOnJoinedGroupAdded:
                    {
                        var resp = EventOnJoinedGroupAddedData.Parser.ParseFrom(data);
                        groupListener?.OnJoinedGroupAdded(resp.Group);
                    }
                    break;

                case FuncRequestEventName.EventOnJoinedGroupDeleted:
                    {
                        var resp = EventOnJoinedGroupDeletedData.Parser.ParseFrom(data);
                        groupListener?.OnJoinedGroupDeleted(resp.Group);
                    }
                    break;

                case FuncRequestEventName.EventOnGroupMemberAdded:
                    {
                        var resp = EventOnGroupMemberAddedData.Parser.ParseFrom(data);
                        groupListener?.OnGroupMemberAdded(resp.Member);
                    }
                    break;

                case FuncRequestEventName.EventOnGroupMemberDeleted:
                    {
                        var resp = EventOnGroupMemberDeletedData.Parser.ParseFrom(data);
                        groupListener?.OnGroupMemberDeleted(resp.Member);
                    }
                    break;

                case FuncRequestEventName.EventOnGroupApplicationAdded:
                    {
                        var resp = EventOnGroupApplicationAddedData.Parser.ParseFrom(data);
                        groupListener?.OnGroupApplicationAdded(resp.Application);
                    }
                    break;

                case FuncRequestEventName.EventOnGroupApplicationDeleted:
                    {
                        var resp = EventOnGroupApplicationDeletedData.Parser.ParseFrom(data);
                        groupListener?.OnGroupApplicationDeleted(resp.Application);
                    }
                    break;

                case FuncRequestEventName.EventOnGroupInfoChanged:
                    {
                        var resp = EventOnGroupInfoChangedData.Parser.ParseFrom(data);
                        groupListener?.OnGroupInfoChanged(resp.Group);
                    }
                    break;

                case FuncRequestEventName.EventOnGroupDismissed:
                    {
                        var resp = EventOnGroupDismissedData.Parser.ParseFrom(data);
                        groupListener?.OnGroupDismissed(resp.Group);
                    }
                    break;

                case FuncRequestEventName.EventOnGroupMemberInfoChanged:
                    {
                        var resp = EventOnGroupMemberInfoChangedData.Parser.ParseFrom(data);
                        groupListener?.OnGroupMemberInfoChanged(resp.Member);
                    }
                    break;

                case FuncRequestEventName.EventOnGroupApplicationAccepted:
                    {
                        var resp = EventOnGroupApplicationAcceptedData.Parser.ParseFrom(data);
                        groupListener?.OnGroupApplicationAccepted(resp.Application);
                    }
                    break;

                case FuncRequestEventName.EventOnGroupApplicationRejected:
                    {
                        var resp = EventOnGroupApplicationRejectedData.Parser.ParseFrom(data);
                        groupListener?.OnGroupApplicationRejected(resp.Application);
                    }
                    break;

                case FuncRequestEventName.EventOnRecvCustomBusinessMessage:
                    {
                        var resp = EventOnRecvCustomBusinessMessageData.Parser.ParseFrom(data);
                        customBusinessListener?.OnRecvCustomBusinessMessage(resp.BusinessMessage);
                    }
                    break;

                case FuncRequestEventName.EventOnSelfInfoUpdated:
                    {
                        var resp = EventOnSelfInfoUpdatedData.Parser.ParseFrom(data);
                        userListener?.OnSelfInfoUpdated(resp.User);
                    }
                    break;

                case FuncRequestEventName.EventOnUserOnlineStatusChanged:
                    {
                        var resp = EventOnUserOnlineStatusChangedData.Parser.ParseFrom(data);
                        userListener?.OnUserOnlineStatusChanged(resp.UserID, resp.Platforms.ToArray());
                    }
                    break;

                case FuncRequestEventName.EventOnUserCommandAdd:
                    {
                        var resp = EventOnUserCommandAddData.Parser.ParseFrom(data);
                        userListener?.OnUserCommandAdd(resp.Command);
                    }
                    break;

                case FuncRequestEventName.EventOnUserCommandDelete:
                    {
                        var resp = EventOnUserCommandDeleteData.Parser.ParseFrom(data);
                        userListener?.OnUserCommandDelete(resp.Command);
                    }
                    break;

                case FuncRequestEventName.EventOnUserCommandUpdate:
                    {
                        var resp = EventOnUserCommandUpdateData.Parser.ParseFrom(data);
                        userListener?.OnUserCommandUpdate(resp.Command);
                    }
                    break;

                case FuncRequestEventName.CreateGroup:
                    {
                        var resp = CreateGroupResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.GroupInfo);
                    }
                    break;

                case FuncRequestEventName.JoinGroup:
                    {
                        var resp = JoinGroupResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.QuitGroup:
                    {
                        var resp = QuitGroupResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.DismissGroup:
                    {
                        var resp = DismissGroupResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.ChangeGroupMute:
                    {
                        var resp = ChangeGroupMuteResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.ChangeGroupMemberMute:
                    {
                        var resp = ChangeGroupMemberMuteResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.TransferGroupOwner:
                    {
                        var resp = TransferGroupOwnerResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.KickGroupMember:
                    {
                        var resp = KickGroupMemberResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.SetGroupInfo:
                    {
                        var resp = SetGroupInfoResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.SetGroupMemberInfo:
                    {
                        var resp = SetGroupMemberInfoResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.GetJoinedGroups:
                    {
                        var resp = GetJoinedGroupsResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Groups.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.GetJoinedGroupsPage:
                    {
                        var resp = GetJoinedGroupsPageResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Groups.ToArray() };
                        callBack?.DynamicInvoke(args);

                    }
                    break;

                case FuncRequestEventName.GetSpecifiedGroupsInfo:
                    {
                        var resp = GetSpecifiedGroupsInfoResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Groups.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.SearchGroups:
                    {
                        var resp = SearchGroupsResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Groups.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.GetGroupMemberOwnerAndAdmin:
                    {
                        var resp = GetGroupMemberOwnerAndAdminResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Members.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.GetGroupMembersByJoinTimeFilter:
                    {
                        var resp = GetGroupMembersByJoinTimeFilterResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Members.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.GetSpecifiedGroupMembersInfo:
                    {
                        var resp = GetSpecifiedGroupMembersInfoResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Members.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.GetGroupMembers:
                    {
                        var resp = GetGroupMembersResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Members.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.GetGroupApplication:
                    {
                        var resp = GetGroupApplicationResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Applications.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.SearchGroupMembers:
                    {
                        var resp = SearchGroupMembersResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Members.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.IsJoinGroup:
                    {
                        var resp = IsJoinGroupResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Joined);
                    }
                    break;

                case FuncRequestEventName.GetUsersInGroup:
                    {
                        var resp = GetUsersInGroupResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.UserIDs.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.InviteUserToGroup:
                    {
                        var resp = InviteUserToGroupResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.HandleGroupApplication:
                    {
                        var resp = HandleGroupApplicationResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.GetSpecifiedFriends:
                    {
                        var resp = GetSpecifiedFriendsResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Friends.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.AddFriend:
                    {
                        var resp = AddFriendResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.GetFriendApplication:
                    {
                        var resp = GetFriendApplicationResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Applications.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.HandleFriendApplication:
                    {
                        var resp = HandleFriendApplicationResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.CheckFriend:
                    {
                        var resp = CheckFriendResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Result.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.DeleteFriend:
                    {
                        var resp = DeleteFriendResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.GetFriends:
                    {
                        var resp = GetFriendsResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Friends.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.GetFriendsPage:
                    {
                        var resp = GetFriendsPageResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Friends.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.SearchFriends:
                    {
                        var resp = SearchFriendsResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Friends.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.AddBlack:
                    {
                        var resp = AddBlackResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.DeleteBlack:
                    {
                        var resp = DeleteBlackResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.GetBlacks:
                    {
                        var resp = GetBlacksResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Blacks.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.UpdateFriend:
                    {
                        var resp = UpdateFriendResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.GetAllConversationList:
                    {
                        var resp = GetAllConversationListResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.ConversationList.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.GetConversationListSplit:
                    {
                        var resp = GetConversationListSplitResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.ConversationList.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.HideConversation:
                    {
                        var resp = HideConversationResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.GetAtAllTag:
                    {
                        var resp = GetAtAllTagResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Tag);
                    }
                    break;

                case FuncRequestEventName.GetOneConversation:
                    {
                        var resp = GetOneConversationResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Conversation);
                    }
                    break;

                case FuncRequestEventName.GetMultipleConversation:
                    {
                        var resp = GetMultipleConversationResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.ConversationList.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.HideAllConversations:
                    {
                        var resp = HideAllConversationsResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.SetConversationDraft:
                    {
                        var resp = SetConversationDraftResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.SetConversation:
                    {
                        var resp = SetConversationResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.GetTotalUnreadMsgCount:
                    {
                        var resp = GetTotalUnreadMsgCountResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.TotalUnreadCount);
                    }
                    break;

                case FuncRequestEventName.GetConversationIdbySessionType:
                    {
                        var resp = GetConversationIDBySessionTypeResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.ConversationID);
                    }
                    break;

                case FuncRequestEventName.FindMessageList:
                    {
                        var resp = FindMessageListResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.TotalCount, resp.FindResultItems.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.GetHistoryMessageList:
                    {
                        var resp = GetHistoryMessageListResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp);
                    }
                    break;

                case FuncRequestEventName.RevokeMessage:
                    {
                        var resp = RevokeMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.TypingStatusUpdate:
                    {
                        var resp = TypingStatusUpdateResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.MarkConversationMessageAsRead:
                    {
                        var resp = MarkConversationMessageAsReadResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.MarkAllConversationMessageAsRead:
                    {
                        var resp = MarkAllConversationMessageAsReadResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.DeleteMessageFromLocal:
                    {
                        var resp = DeleteMessageFromLocalResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.DeleteMessage:
                    {
                        var resp = DeleteMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.DeleteAllMsgFromLocalAndServer:
                    {
                        var resp = DeleteAllMsgFromLocalAndServerResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.DeleteAllMessageFromLocal:
                    {
                        var resp = DeleteAllMessageFromLocalResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.ClearConversationAndDeleteAllMsg:
                    {
                        var resp = ClearConversationAndDeleteAllMsgResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.DeleteConversationAndDeleteAllMsg:
                    {
                        var resp = DeleteConversationAndDeleteAllMsgResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.InsertSingleMessageToLocal:
                    {
                        var resp = InsertSingleMessageToLocalResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Msg);
                    }
                    break;

                case FuncRequestEventName.InsertGroupMessageToLocal:
                    {
                        var resp = InsertGroupMessageToLocalResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Msg);
                    }
                    break;

                case FuncRequestEventName.SearchLocalMessages:
                    {
                        var resp = SearchLocalMessagesResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Count, resp.SearchResultItems };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.SetMessageLocalEx:
                    {
                        var resp = SetMessageLocalExResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Success);
                    }
                    break;

                case FuncRequestEventName.SearchConversation:
                    {
                        var resp = SearchConversationResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.ConversationList.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.CreateTextMessage:
                    {
                        var resp = CreateTextAtMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;

                case FuncRequestEventName.CreateAdvancedTextMessage:
                    {
                        var resp = CreateAdvancedTextMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;

                case FuncRequestEventName.CreateTextAtMessage:
                    {
                        var resp = CreateTextAtMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;

                case FuncRequestEventName.CreateLocationMessage:
                    {
                        var resp = CreateLocationMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;

                case FuncRequestEventName.CreateCustomMessage:
                    {
                        var resp = CreateCustomMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;

                case FuncRequestEventName.CreateQuoteMessage:
                    {
                        var resp = CreateQuoteMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;

                case FuncRequestEventName.CreateAdvancedQuoteMessage:
                    {
                        var resp = CreateAdvancedQuoteMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;


                case FuncRequestEventName.CreateCardMessage:
                    {
                        var resp = CreateCardMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;

                case FuncRequestEventName.CreateImageMessage:
                    {
                        var resp = CreateImageMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;

                case FuncRequestEventName.CreateSoundMessage:
                    {
                        var resp = CreateSoundMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;

                case FuncRequestEventName.CreateVideoMessage:
                    {
                        var resp = CreateVideoMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;

                case FuncRequestEventName.CreateFileMessage:
                    {
                        var resp = CreateFileMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;

                case FuncRequestEventName.CreateMergerMessage:
                    {
                        var resp = CreateMergerMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;

                case FuncRequestEventName.CreateFaceMessage:
                    {
                        var resp = CreateFaceMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;

                case FuncRequestEventName.CreateForwardMessage:
                    {
                        var resp = CreateForwardMessageResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.Message);
                    }
                    break;

                case FuncRequestEventName.ProcessUserCommandGetAll:
                    {
                        var resp = ProcessUserCommandGetAllResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Commands.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.GetSelfUserInfo:
                    {
                        var resp = GetSelfUserInfoResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(resp.User);
                    }
                    break;

                case FuncRequestEventName.SetSelfInfo:
                    {
                        var resp = SetSelfInfoResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.ProcessUserCommandAdd:
                    {
                        var resp = ProcessUserCommandAddResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.ProcessUserCommandDelete:
                    {
                        var resp = ProcessUserCommandDeleteResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.ProcessUserCommandUpdate:
                    {
                        var resp = ProcessUserCommandUpdateResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;

                case FuncRequestEventName.GetUsersInfo:
                    {
                        var resp = GetUsersInfoResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Users.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.SubscribeUsersOnlineStatus:
                    {
                        var resp = SubscribeUsersOnlineStatusResp.Parser.ParseFrom(data);
                        var args = new object[] { resp.Status.ToArray() };
                        callBack?.DynamicInvoke(args);
                    }
                    break;

                case FuncRequestEventName.UnsubscribeUsersOnlineStatus:
                    {
                        var resp = UnsubscribeUsersOnlineStatusResp.Parser.ParseFrom(data);
                        callBack?.DynamicInvoke(suc);
                    }
                    break;
            }

            if (callBack != null)
            {
                callBackDic.Remove(handleId);
            }
        }
    }
}