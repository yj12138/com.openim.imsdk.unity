using System.Collections.Generic;
using System;
using System.Reflection;
using OpenIM.IMSDK.Unity.Native;
using OpenIM.IMSDK.Unity.Util;
using OpenIM.IMSDK.Unity.Listener;

namespace OpenIM.IMSDK.Unity
{
    public partial class IMSDK
    {
        public delegate void OnInt(int v);
        public delegate void OnBool(int v);
        public delegate void OnBase(bool suc, int errCode, string errMsg);
        public delegate void OnConversation(LocalConversation conversation, int errCode, string errMsg);
        public delegate void OnConversationList(List<LocalConversation> list, int errCode, string errMsg);
        public delegate void OnLocalUser(LocalUser user, int errCode, string errMsg);
        public delegate void OnLocalUserList(List<LocalUser> list, int errCode, string errMsg);
        public delegate void OnFullUserInfoList(List<FullUserInfo> list, int errCode, string errMsg);
        public delegate void OnFullUserInfoWithCacheList(List<FullUserInfoWithCache> list, int errCode, string errMsg);
        public delegate void OnGetConversationRecvMessageOptRespList(List<GetConversationRecvMessageOptResp> list, int errCode, string errMsg);
        public delegate void OnFindMesageList(List<FindMessageList> list, int errCode, string errMsg);
        public delegate void OnGetAdvancedHistoryMessageList(GetAdvancedHistoryMessageList historyMsgList, int errCode, string errMsg);
        public delegate void OnMsgStruct(MsgStruct msg, int errCode, string errMsg);
        public delegate void OnSearchLocalMessagesCallback(SearchLocalMessagesCallback v, int errCode, string errMsg);
        public delegate void OnOnlineStatusList(List<OnlineStatus> list, int errCode, string errMsg);
        public delegate void OnSearchFriendItemList(List<SearchFriendItem> list, int errCode, string errMsg);
        public delegate void OnUserIDResultList(List<UserIDResult> list, int errCode, string errMsg);
        public delegate void OnLocalFriendRequestList(List<LocalFriendRequest> list, int errCode, string errMsg);
        public delegate void OnLocalBlackList(List<LocalBlack> list, int errCode, string errMsg);
        public delegate void OnGroupInfo(LocalGroup groupInfo, int errCode, string errMsg);
        public delegate void OnLocalGroupList(List<LocalGroup> list, int errCode, string errMsg);
        public delegate void OnLocalGroupMemberList(List<LocalGroupMember> list, int errCode, string errMsg);
        public delegate void OnLocalAdminGroupRequestList(List<LocalAdminGroupRequest> list, int errCode, string errMsg);
        public delegate void OnLocalGroupRequestList(List<LocalGroupRequest> list, int errCode, string errMsg);
        public delegate void OnSendMessage(MsgStruct msg, int errCode, string errMsg);
        private static Dictionary<string, Delegate> callBackDic = new Dictionary<string, Delegate>();
        private static ListenGroup listenGroup;
        private static string GetOperationId(string prefix)
        {
            return prefix + "_" + Utils.GetOperationIndex();
        }
        public static void Polling()
        {
            if (msgCache.Count > 0)
            {
                IdMsg msg;
                while (msgCache.TryDequeue(out msg))
                {
                    Utils.Log(string.Format("{0}]:{1}", (MessageDef)msg.Id, msg.Data));
                    try
                    {
                        DispatorMsg((MessageDef)msg.Id, msg.Data);
                    }
                    catch (Exception e)
                    {
                        Utils.Error(e.ToString());
                    }
                }
            }
        }

        #region convert value 
        class Empty
        {
        }
        class BoolValue
        {
            public bool value;
        }
        class StringValue
        {
            public string value;
        }
        class IntValue
        {
            public int value;
        }
        #endregion

        #region init_login
        public static bool InitSDK(IMConfig _config, ListenGroup _listenGroup)
        {
            listenGroup = _listenGroup;
            NativeSDK.SetMessageHandler(MessageHandler);
            string config = Utils.ToJson(_config);
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                config
            };
            var res = NativeSDK.CallAPI<BoolValue>(APIKey.InitSDK, Utils.ToJson(args));
            return res.value;
        }
        public static void UnInitSDK()
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            NativeSDK.CallAPI<Empty>(APIKey.UnInitSDK, Utils.ToJson(args));
        }
        public static void Login(string uid, string token, OnBase cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                uid,
                token
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.Login, Utils.ToJson(args));
        }
        public static void Logout(OnBase cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.Logout, Utils.ToJson(args));
        }
        public static void SetAppBackGroundStatus(bool isBackground, OnBase cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                isBackground,
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.SetAppBackgroundStatus, Utils.ToJson(args));
        }
        public static void NetworkStatusChanged(OnBase cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.NetworkStatusChanged, Utils.ToJson(args));
        }
        public static LoginStatus GetLoginStatus()
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            var res = NativeSDK.CallAPI<IntValue>(APIKey.GetLoginStatus, Utils.ToJson(args));
            return (LoginStatus)res.value;
        }
        public static string GetLoginUser()
        {
            var res = NativeSDK.CallAPI<StringValue>(APIKey.GetLoginUserID, "");
            return res.value;
        }
        #endregion

        #region conversation_msg
        public static MsgStruct CreateTextMessage(string text)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                text,
            };
            var res = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateTextMessage, Utils.ToJson(args));
            return res;
        }
        public static MsgStruct CreateAdvancedTextMessage(string text, MessageEntity[] messageEntityList)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                text,
                messageEntityList = Utils.ToJson(messageEntityList)
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateAdvancedTextMessage, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateTextAtMessage(string text, string[] atUserList, AtInfo[] atUsersInfo, MsgStruct message)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                text,
                atUserList = Utils.ToJson(atUserList),
                atUsersInfo = Utils.ToJson(atUsersInfo),
                message = Utils.ToJson(message)
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateTextAtMessage, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateLocationMessage(string description, double longitude, double latitude)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                description,
                longitude,
                latitude
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateLocationMessage, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateCustomMessage(string data, string extension, string description)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                data,
                extension,
                description
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateCustomMessage, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateQuoteMessage(string text, MsgStruct message)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                text,
                message
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateQuoteMessage, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateAdvancedQuoteMessage(string text, MsgStruct message, MessageEntity[] messageEntityList)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                text,
                message = Utils.ToJson(message),
                messageEntityList = Utils.ToJson(messageEntityList)
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateAdvancedQuoteMessage, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateCardMessage(CardElem cardInfo)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                cardInfo = Utils.ToJson(cardInfo),
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateCardMessage, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateVideoMessageFromFullPath(string videoFullPath, string videoType, long duration, string snapshotFullPath)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                videoFullPath,
                videoType,
                duration,
                snapshotFullPath
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateVideoMessageFromFullPath, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateImageMessageFromFullPath(string imageFullPath)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                imageFullPath
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateImageMessageFromFullPath, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateSoundMessageFromFullPath(string soundPath, long duration)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                soundPath,
                duration
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateSoundMessageFromFullPath, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateFileMessageFromFullPath(string fileFullPath, string fileName)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                fileFullPath,
                fileName
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateFileMessageFromFullPath, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateImageMessage(string imagePath)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                imagePath
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateImageMessage, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateImageMessageByURL(string sourcePath, PictureBaseInfo sourcePicture, PictureBaseInfo bigPicture, PictureBaseInfo snapshotPicture)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                sourcePath,
                sourcePicture = Utils.ToJson(sourcePicture),
                bigPicture = Utils.ToJson(bigPicture),
                snapshotPicture = Utils.ToJson(snapshotPicture)
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateImageMessageByURL, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateSoundMessageByURL(SoundBaseInfo soundBaseInfo)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                soundBaseInfo = Utils.ToJson(soundBaseInfo)
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateSoundMessageByURL, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateSoundMessage(string soundPath, long duration)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                soundPath,
                duration
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateSoundMessage, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateVideoMessageByURL(VideoBaseInfo videoBaseInfo)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                videoBaseInfo = Utils.ToJson(videoBaseInfo)
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateVideoMessageByURL, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateVideoMessage(string videoPath, string videoType, long duration, string snapshotPath)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                videoPath,
                videoType,
                duration,
                snapshotPath,
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateVideoMessage, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateFileMessageByURL(FileElem fileBaseInfo)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                fileBaseInfo = Utils.ToJson(fileBaseInfo)
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateFileMessageByURL, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateFileMessage(string filePath, string fileName)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                filePath,
                fileName
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateFileMessage, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateMergerMessage(MsgStruct[] messageList, string title, string[] summaryList)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                messageList = Utils.ToJson(messageList),
                title,
                summaryList = Utils.ToJson(summaryList)
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateMergerMessage, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateFaceMessage(int index, string data)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                index,
                data
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateFaceMessage, Utils.ToJson(args));
            return msg;
        }
        public static MsgStruct CreateForwardMessage(MsgStruct message)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                message = Utils.ToJson(message)
            };
            var msg = NativeSDK.CallAPI<MsgStruct>(APIKey.CreateForwardMessage, Utils.ToJson(args));
            return msg;
        }
        public static void GetAllConversationList(OnConversationList cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetAllConversationList, Utils.ToJson(args));
        }
        public static void GetConversationListSplit(OnConversationList cb, int offset, int count)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                offset,
                count
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetConversationListSplit, Utils.ToJson(args));
        }
        public static void GetOneConversation(OnConversation cb, int sessionType, string sourceId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                sessionType,
                sourceId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetOneConversation, Utils.ToJson(args));
        }
        public static void GetMultipleConversation(OnConversationList cb, string[] conversationIdList)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                conversationIdList = Utils.ToJson(conversationIdList)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetMultipleConversation, Utils.ToJson(args));
        }
        public static void HideConversation(OnBase cb, string conversationId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                conversationId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.HideConversation, Utils.ToJson(args));
        }
        public static void SetConversation(OnBase cb, string conversationId, ConversationReq req)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                conversationId,
                req = Utils.ToJson(req)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.SetConversation, Utils.ToJson(args));
        }
        public static void SetConversationDraft(OnBase cb, string conversationId, string draftText)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                conversationId,
                draftText
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.SetConversationDraft, Utils.ToJson(args));
        }
        public static void GetTotalUnreadMsgCount(OnInt cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetTotalUnreadMsgCount, Utils.ToJson(args));
        }
        public static string GetAtAllTag()
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            var res = NativeSDK.CallAPI<StringValue>(APIKey.GetAtAllTag, Utils.ToJson(args));
            return res.value;
        }
        public static string GetConversationIdBySessionType(string sourceId, int sessionType)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                sourceId,
                sessionType
            };
            var res = NativeSDK.CallAPI<StringValue>(APIKey.GetConversationIDBySessionType, Utils.ToJson(args));
            return res.value;
        }
        public static void SendMessage(OnSendMessage cb, MsgStruct message, string recvId, string groupId, OfflinePushInfo offlinePushInfo, bool isOnlineOnly)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                message = Utils.ToJson(message),
                recvId,
                groupId,
                offlinePushInfo = Utils.ToJson(offlinePushInfo),
                isOnlineOnly
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.SendMessage, Utils.ToJson(args));
        }
        public static void SendMessageNotOSS(OnSendMessage cb, MsgStruct message, string recvId, string groupId, OfflinePushInfo offlinePushInfo, bool isOnlineOnly)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                message = Utils.ToJson(message),
                recvId,
                groupId,
                offlinePushInfo = Utils.ToJson(offlinePushInfo),
                isOnlineOnly
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.SendMessageNotOss, Utils.ToJson(args));
        }
        public static void FindMessageList(OnFindMesageList cb, ConversationArgs[] findMessageOptions)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                findMessageOptions = Utils.ToJson(findMessageOptions)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.FindMessageList, Utils.ToJson(args));
        }
        public static void GetAdvancedHistoryMessageList(OnGetAdvancedHistoryMessageList cb, GetAdvancedHistoryMessageListParams getMessageOptions)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                getMessageOptions = Utils.ToJson(getMessageOptions)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetAdvancedHistoryMessageList, Utils.ToJson(args));
        }
        public static void GetAdvancedHistoryMessageListReverse(OnGetAdvancedHistoryMessageList cb, GetAdvancedHistoryMessageListParams getMessageOptions)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                getMessageOptions = Utils.ToJson(getMessageOptions)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetAdvancedHistoryMessageListReverse, Utils.ToJson(args));
        }
        public static void RevokeMessage(OnGetAdvancedHistoryMessageList cb, string conversationId, string clientMsgId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                conversationId,
                clientMsgId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.RevokeMessage, Utils.ToJson(args));
        }
        public static void TypingStatusUpdate(OnBase cb, string recvId, string msgTip)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                recvId,
                msgTip
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.TypingStatusUpdate, Utils.ToJson(args));
        }
        public static void MarkConversationMessageAsRead(OnBase cb, string conversationId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                conversationId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.MarkConversationMessageAsRead, Utils.ToJson(args));
        }
        public static void DeleteMessageFromLocalStorage(OnBase cb, string conversationId, string clientMsgId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                conversationId,
                clientMsgId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.DeleteMessageFromLocalStorage, Utils.ToJson(args));
        }
        public static void DeleteMessage(OnBase cb, string conversationId, string clientMsgId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                conversationId,
                clientMsgId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.DeleteMessage, Utils.ToJson(args));
        }
        public static void HideAllConversations(OnBase cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.HideAllConversations, Utils.ToJson(args));
        }
        public static void DeleteAllMsgFromLocalAndSVR(OnBase cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.DeleteAllMsgFromLocalAndSvr, Utils.ToJson(args));
        }
        public static void DeleteAllMsgFromLocal(OnBase cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.DeleteAllMsgFromLocal, Utils.ToJson(args));
        }
        public static void ClearConversationAndDeleteAllMsg(OnBase cb, string conversationId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                conversationId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.ClearConversationAndDeleteAllMsg, Utils.ToJson(args));
        }
        public static void DeleteConversationAndDeleteAllMsg(OnBase cb, string conversationId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                conversationId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.DeleteConversationAndDeleteAllMsg, Utils.ToJson(args));
        }
        public static void InsertSingleMessageToLocalStorage(OnMsgStruct cb, MsgStruct message, string recvId, string sendId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                message = Utils.ToJson(message),
                recvId,
                sendId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.InsertSingleMessageToLocalStorage, Utils.ToJson(args));
        }
        public static void InsertGroupMessageToLocalStorage(OnMsgStruct cb, MsgStruct message, string groupId, string sendId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                message = Utils.ToJson(message),
                groupId,
                sendId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.InsertGroupMessageToLocalStorage, Utils.ToJson(args));
        }
        public static void SearchLocalMessages(OnSearchLocalMessagesCallback cb, SearchLocalMessagesParams searchParam)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                searchParam = Utils.ToJson(searchParam)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.SearchLocalMessages, Utils.ToJson(args));
        }
        public static void SetMessageLocalEx(OnBase cb, string conversationId, string clientMsgId, string localEx)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                conversationId,
                clientMsgId,
                localEx
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.SetMessageLocalEx, Utils.ToJson(args));
        }
        #endregion

        #region user
        public static void GetUsersInfo(OnFullUserInfoList cb, string[] userIds)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                userIds = Utils.ToJson(userIds)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetUsersInfo, Utils.ToJson(args));
        }
        public static void SetSelfInfo(OnBase cb, UserInfo userInfo)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                userInfo
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.SetSelfInfo, Utils.ToJson(args));
        }
        public static void GetSelfUserInfo(OnLocalUser cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetSelfUserInfo, Utils.ToJson(args));
        }
        public static void SubscribeUsersStatus(OnOnlineStatusList cb, string[] userIds)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                userIds = Utils.ToJson(userIds)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.SubscribeUsersStatus, Utils.ToJson(args));
        }
        public static void UnsubscribeUsersStatus(OnBase cb, string[] userIds)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                userIds = Utils.ToJson(userIds)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.UnsubscribeUsersStatus, Utils.ToJson(args));
        }
        public static void GetSubscribeUsersStatus(OnOnlineStatusList cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetSubscribeUsersStatus, Utils.ToJson(args));
        }
        public static void GetUserStatus(OnOnlineStatusList cb, string[] userIds)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                userIds = Utils.ToJson(userIds)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetUserStatus, Utils.ToJson(args));
        }
        #endregion

        #region friend
        public static void GetSpecifiedFriendsInfo(OnFullUserInfoList cb, string[] userIdList, bool filterBlack)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                userIdList = Utils.ToJson(userIdList),
                filterBlack
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetSpecifiedFriendsInfo, Utils.ToJson(args));
        }
        public static void GetFriendList(OnFullUserInfoList cb, bool filterBlack)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                filterBlack
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetFriendList, Utils.ToJson(args));
        }
        public static void GetFriendListPage(OnFullUserInfoList cb, int offset, int count, bool filterBlack)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                offset,
                count,
                filterBlack
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetFriendListPage, Utils.ToJson(args));
        }
        public static void SearchFriends(OnSearchFriendItemList cb, SearchFriendsParam searchParam)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                searchParam = Utils.ToJson(searchParam)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.SearchFriends, Utils.ToJson(args));
        }
        public static void UpdateFriends(OnBase cb, UpdateFriendsReq req)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                req = Utils.ToJson(req)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.UpdateFriends, Utils.ToJson(args));
        }
        public static void CheckFriend(OnUserIDResultList cb, string[] userIdList)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                userIdList = Utils.ToJson(userIdList)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.CheckFriend, Utils.ToJson(args));
        }
        public static void AddFriend(OnBase cb, ApplyToAddFriendReq userIdReqMsg)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                userIdReqMsg = Utils.ToJson(userIdReqMsg)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.AddFriend, Utils.ToJson(args));
        }
        public static void DeleteFriend(OnBase cb, string friendUserId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                friendUserId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.DeleteFriend, Utils.ToJson(args));
        }
        public static void GetFriendApplicationListAsRecipient(OnLocalFriendRequestList cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetFriendApplicationListAsRecipient, Utils.ToJson(args));
        }
        public static void GetFriendApplicationListAsApplicant(OnLocalFriendRequestList cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetFriendApplicationListAsApplicant, Utils.ToJson(args));
        }
        public static void AcceptFriendApplication(OnBase cb, ProcessFriendApplicationParams userIDHandleMsg)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                userIdHandleMsg = Utils.ToJson(userIDHandleMsg)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.AcceptFriendApplication, Utils.ToJson(args));
        }
        public static void RefuseFriendApplication(OnBase cb, ProcessFriendApplicationParams userIdHandleMsg)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                userIdHandleMsg = Utils.ToJson(userIdHandleMsg)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.RefuseFriendApplication, Utils.ToJson(args));
        }
        public static void AddBlack(OnBase cb, string blackUserId, string ex)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                blackUserId,
                ex
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.AddBlack, Utils.ToJson(args));
        }
        public static void GetBlackList(OnLocalBlackList cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetBlackList, Utils.ToJson(args));
        }
        public static void RemoveBlack(OnBase cb, string removeUserId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                removeUserId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.RemoveBlack, Utils.ToJson(args));
        }
        #endregion

        #region group
        public static void CreateGroup(OnGroupInfo cb, CreateGroupReq groupReqInfo)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupReqInfo = Utils.ToJson(groupReqInfo)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.CreateGroup, Utils.ToJson(args));
        }
        public static void JoinGroup(OnBase cb, string groupId, string reqMsg, JoinSource joinSource, string ex)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupId,
                reqMsg,
                joinSource = (int)joinSource,
                ex,
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.JoinGroup, Utils.ToJson(args));
        }
        public static void QuitGroup(OnBase cb, string groupId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.QuitGroup, Utils.ToJson(args));
        }

        public static void DismissGroup(OnBase cb, string groupId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.DismissGroup, Utils.ToJson(args));
        }
        public static void ChangeGroupMute(OnBase cb, string groupId, bool isMute)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupId,
                isMute,
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.ChangeGroupMute, Utils.ToJson(args));
        }
        public static void ChangeGroupMemberMute(OnBase cb, string groupId, string userId, int mutedSeconds)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupId,
                userId,
                mutedSeconds
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.ChangeGroupMemberMute, Utils.ToJson(args));
        }
        public static void SetGroupMemberInfo(OnBase cb, SetGroupMemberInfo groupMemberInfo)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupMemberInfo = Utils.ToJson(groupMemberInfo)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.SetGroupMemberInfo, Utils.ToJson(args));
        }
        public static void GetJoinedGroupList(OnLocalGroupList cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetJoinedGroupList, Utils.ToJson(args));
        }
        public static void GetSpecifiedGroupsInfo(OnLocalGroupList cb, string[] groupIdList)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupIdList = Utils.ToJson(groupIdList)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetSpecifiedGroupsInfo, Utils.ToJson(args));
        }
        public static void SearchGroups(OnLocalGroupList cb, SearchGroupsParam searchParam)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                searchParam = Utils.ToJson(searchParam)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.SearchGroups, Utils.ToJson(args));
        }
        public static void SetGroupInfo(OnBase cb, GroupInfoForSet groupInfo)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupInfo = Utils.ToJson(groupInfo)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.SetGroupInfo, Utils.ToJson(args));
        }
        public static void GetGroupMemberList(OnLocalGroupMemberList cb, string groupId, int filter, int offset, int count)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupId,
                filter,
                offset,
                count,
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetGroupMemberList, Utils.ToJson(args));
        }
        public static void GetGroupMemberOwnerAndAdmin(OnLocalGroupMemberList cb, string groupId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetGroupMemberOwnerAndAdmin, Utils.ToJson(args));
        }
        public static void GetGroupMemberListByJoinTimeFilter(OnLocalGroupMemberList cb, string groupId, int offset, int count, long joinTimeBegin, long joinTimeEnd, string[] filterUserIDList)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupId,
                offset,
                count,
                joinTimeBegin,
                joinTimeEnd,
                filterUserIdList = Utils.ToJson(filterUserIDList)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetGroupMemberListByJoinTimeFilter, Utils.ToJson(args));
        }
        public static void GetSpecifiedGroupMembersInfo(OnLocalGroupMemberList cb, string groupId, string[] userIdList)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupId,
                userIdList = Utils.ToJson(userIdList)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetSpecifiedGroupMembersInfo, Utils.ToJson(args));
        }
        public static void KickGroupMember(OnBase cb, string groupId, string reason, string[] userIdList)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupId,
                reason,
                userIdList = Utils.ToJson(userIdList)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.KickGroupMember, Utils.ToJson(args));
        }
        public static void TransferGroupOwner(OnBase cb, string groupId, string newOwnerUserId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupId,
                newOwnerUserId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.TransferGroupOwner, Utils.ToJson(args));
        }
        public static void InviteUserToGroup(OnBase cb, string groupId, string reason, string[] userIdList)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupId,
                reason,
                userIdList = Utils.ToJson(userIdList)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.InviteUserToGroup, Utils.ToJson(args));
        }
        public static void GetGroupApplicationListAsRecipient(OnLocalAdminGroupRequestList cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetGroupApplicationListAsRecipient, Utils.ToJson(args));
        }
        public static void GetGroupApplicationListAsApplicant(OnLocalGroupRequestList cb)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.GetGroupApplicationListAsApplicant, Utils.ToJson(args));
        }
        public static void AcceptGroupApplication(OnBase cb, string groupID, string fromUserID, string handleMsg)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.AcceptGroupApplication, Utils.ToJson(args));
        }
        public static void RefuseGroupApplication(OnBase cb, string groupId, string fromUserId, string handleMsg)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupId,
                fromUserId,
                handleMsg
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.RefuseFriendApplication, Utils.ToJson(args));
        }
        public static void SearchGroupMembers(OnLocalGroupMemberList cb, SearchGroupMembersParam searchParam)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                searchParam = Utils.ToJson(searchParam)
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.SearchGroupMembers, Utils.ToJson(args));
        }
        public static void IsJoinGroup(OnBool cb, string groupId)
        {
            var args = new
            {
                operationId = GetOperationId(MethodBase.GetCurrentMethod().Name),
                groupId
            };
            callBackDic[args.operationId] = cb;
            NativeSDK.CallAPI<Empty>(APIKey.IsJoinGroup, Utils.ToJson(args));
        }
        #endregion
    }
}