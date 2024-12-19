using OpenIM.IMSDK.Native;
using OpenIM.IMSDK.Listener;
using OpenIM.Proto;
using OpenIM.IMSDK.Util;
using System.Collections.Generic;
using System;

namespace OpenIM.IMSDK
{
    public interface ISendMsg
    {
        public void OnError(int code, string errMsg);
        public void OnSuccess(IMMessage msg);
        public void OnProgress(long progress);
    }
    public interface IUploadFile
    {
        public void OnError(int code, string errMsg);
        public void OnSuccess(string url);
        public void OnProgress(long progress);
    }
    public interface IUploadLogs
    {
        public void OnError(int code, string errMsg);
        public void OnSuccess();
        public void OnProgress(long progress);
    }

    public partial class IMSDK
    {
        public delegate void ErrorHandler(int errCode, string errMsg);

        private static Dictionary<ulong, Delegate> callBackDic = new Dictionary<ulong, Delegate>();
        private static Dictionary<ulong, ISendMsg> sendMsgCallBackDic = new Dictionary<ulong, ISendMsg>();
        private static Dictionary<ulong, IUploadFile> uploadFileCallBackDic = new Dictionary<ulong, IUploadFile>();
        private static Dictionary<ulong, IUploadLogs> uploadLogsCallBackDic = new Dictionary<ulong, IUploadLogs>();
        private static ErrorHandler errorHandler;
        private static IConnListener connListener;
        private static IConversationListener conversationListener;
        private static IFriendShipListener friendShipListener;
        private static IGroupListener groupListener;
        private static IMessageListener messageListener;
        private static IUserListener userListener;
        private static ICustomBusinessListener customBusinessListener;

        public static void SetErrorHandler(ErrorHandler handler)
        {
            errorHandler = handler;
        }
        #region  set listener
        public static void SetConnListener(IConnListener l)
        {
            connListener = l;
        }
        public static void SetConversationListener(IConversationListener l)
        {
            conversationListener = l;
        }
        public static void SetFriendShipListener(IFriendShipListener l)
        {
            friendShipListener = l;
        }
        public static void SetGroupListener(IGroupListener l)
        {
            groupListener = l;
        }
        public static void SetMessageListener(IMessageListener l)
        {
            messageListener = l;
        }
        public static void SetUserListener(IUserListener l)
        {
            userListener = l;
        }
        public static void SetCustomBusinessListener(ICustomBusinessListener l)
        {
            customBusinessListener = l;
        }
        #endregion


        static void Interal_Init()
        {
            NativeSDK.Init(OnRecvEvent);
        }

        public static void Polling()
        {
            try
            {
                if (events.Count > 0)
                {
                    FfiResult result;
                    while (events.TryDequeue(out result))
                    {
                        HandleFFIResult(result);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.Log(e.ToString());
            }
        }

        static ulong handleId = 0;
        static ulong GetHandleId()
        {
            handleId++;
            return handleId;
        }

        #region init_login
        public static void Version(Action<string> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.Version, new VersionReq { });
        }
        public static void InitSDK(Action<bool> cb, IMConfig config)
        {
            Interal_Init();

            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.InitSdk, new InitSDKReq
            {
                Config = config
            });
        }
        public static void Login(Action<bool> cb, string uid, string token)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.Login, new LoginReq
            {
                UserID = uid,
                Token = token,
            });
        }
        public static void Logout(Action<bool> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.Logout, new LogoutReq
            {

            });
        }
        public static void SetAppBackGroundStatus(Action<bool> cb, bool isBackground)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.SetAppBackgroundStatus, new SetAppBackgroundStatusReq
            {
                IsBackground = isBackground
            });
        }
        public static void NetworkStatusChanged(Action<bool> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.NetworkStatusChanged, new NetworkStatusChangedReq { });
        }
        public static void GetLoginStatus(Action<LoginStatus> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetLoginStatus, new GetLoginStatusReq { });
        }
        #endregion

        #region conversation_msg
        public static void CreateTextMessage(Action<IMMessage> cb, string text)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateTextMessage, new CreateTextMessageReq
            {
                Text = text
            });
        }
        public static void CreateAdvancedTextMessage(Action<IMMessage> cb, string text, MessageEntity[] messageEntityList)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new CreateAdvancedTextMessageReq
            {
                Text = text,
            };
            req.MessageEntities.Add(messageEntityList);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateAdvancedTextMessage, req);
        }
        public static void CreateTextAtMessage(Action<IMMessage> cb, string text, string[] atUserList, AtInfo[] atUsersInfo, IMMessage message)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new CreateTextAtMessageReq
            {
                Text = text,
                QuoteMessage = message
            };
            req.UserIDList.Add(atUserList);
            req.UsersInfo.Add(atUsersInfo);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateTextAtMessage, req);
        }
        public static void CreateLocationMessage(Action<IMMessage> cb, string description, double longitude, double latitude)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateLocationMessage, new CreateLocationMessageReq
            {
                Description = description,
                Longitude = longitude,
                Latitude = latitude
            });
        }
        public static void CreateCustomMessage(Action<IMMessage> cb, string data, string extension, string description)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateCustomMessage, new CreateCustomMessageReq
            {
                Data = data,
                Extension = extension,
                Description = description
            });
        }
        public static void CreateQuoteMessage(Action<IMMessage> cb, string text, IMMessage message)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateQuoteMessage, new CreateQuoteMessageReq
            {
                Text = text,
                QuoteMessage = message
            });
        }
        public static void CreateAdvancedQuoteMessage(Action<IMMessage> cb, string text, IMMessage message, MessageEntity[] messageEntityList)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new CreateAdvancedQuoteMessageReq
            {
                Text = text,
                QuoteMessage = message,
            };
            req.MessageEntities.Add(messageEntityList);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateAdvancedQuoteMessage, req);
        }
        public static void CreateCardMessage(Action<IMMessage> cb, string userId, string nickName, string faceURL, string ex)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateCardMessage, new CreateCardMessageReq
            {
                UserID = userId,
                Nickname = nickName,
                FaceURL = faceURL,
                Ex = ex
            });
        }
        public static void CreateImageMessage(Action<IMMessage> cb, CreateImageMessageReq req)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateImageMessage, req);
        }
        public static void CreateSoundMessage(Action<IMMessage> cb, string soundPath, long duration)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateSoundMessage, new CreateSoundMessageReq
            {
                SoundPath = soundPath,
                Duration = duration
            });
        }
        public static void CreateVideoMessage(Action<IMMessage> cb, string videoPath, string videoType, long duration, string snapshotPath)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateVideoMessage, new CreateVideoMessageReq
            {
                VideoSourcePath = videoPath,
                VideoType = videoType,
                Duration = duration,
                SnapshotSourcePath = snapshotPath
            });
        }
        public static void CreateFileMessage(Action<IMMessage> cb, string filePath, string fileName)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateFileMessage, new CreateFileMessageReq
            {
                FileSourcePath = filePath,
                FileName = fileName
            });
        }
        public static void CreateMergerMessage(Action<IMMessage> cb, IMMessage[] messageList, string title, string[] summaryList)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new CreateMergerMessageReq
            {
                Title = title,
            };
            req.Messages.Add(messageList);
            req.Summaries.Add(summaryList);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateMergerMessage, req);
        }
        public static void CreateFaceMessage(Action<IMMessage> cb, int index, string data)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateFaceMessage, new CreateFaceMessageReq
            {
                Index = index,
                Data = data,
            });
        }
        public static void CreateForwardMessage(Action<IMMessage> cb, IMMessage message)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateForwardMessage, new CreateForwardMessageReq
            {
                Message = message
            });
        }
        public static void GetAllConversationList(Action<IMConversation[]> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetAllConversationList, new GetAllConversationListReq { });
        }
        public static void GetConversationListSplit(Action<IMConversation[]> cb, int pageNumber, int showNumber)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetConversationListSplit, new GetConversationListSplitReq
            {
                Pagination = new RequestPagination
                {
                    PageNumber = pageNumber,
                    ShowNumber = showNumber,
                }
            });
        }
        public static void GetOneConversation(Action<IMConversation> cb, SessionType sessionType, string sourceId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetOneConversation, new GetOneConversationReq
            {
                SessionType = sessionType,
                SourceID = sourceId,
            });
        }
        public static void GetMultipleConversation(Action<IMConversation[]> cb, string[] conversationIdList)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new GetMultipleConversationReq();
            req.ConversationIDList.Add(conversationIdList);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetMultipleConversation, req);
        }
        public static void HideConversation(Action<bool> cb, string conversationId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.HideConversation, new HideConversationReq
            {
                ConversationID = conversationId,
            });
        }
        public static void HideAllConversations(Action<bool> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.HideAllConversations, new HideAllConversationsReq { });
        }
        public static void SetConversation(Action<bool> cb, SetConversationReq req)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.SetConversation, req);
        }
        public static void SetConversationDraft(Action<bool> cb, string conversationId, string draftText)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.SetConversationDraft, new SetConversationDraftReq
            {
                ConversationID = conversationId,
                DraftText = draftText,
            });
        }
        public static void GetTotalUnreadMsgCount(Action<int> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetTotalUnreadMsgCount, new GetTotalUnreadMsgCountReq { });
        }
        public static void GetAtAllTag(Action<string> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetAtAllTag, new GetAtAllTagReq { });
        }
        public static void GetConversationIdBySessionType(Action<string> cb, string sourceId, SessionType sessionType)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetConversationIdbySessionType, new GetConversationIDBySessionTypeReq
            {
                SourceID = sourceId,
                SessionType = sessionType,
            });
        }
        public static void SendMessage(ISendMsg cb, IMMessage message, string recvId, string groupId, bool isOnlineOnly)
        {
            var handleId = GetHandleId();
            sendMsgCallBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.SendMessage, new SendMessageReq
            {
                Message = message,
                RecvID = recvId,
                GroupID = groupId,
                IsOnlineOnly = isOnlineOnly,
            });
        }
        public static void FindMessageList(Action<int, SearchByConversationResult[]> cb, ConversationArgs[] findMessageOptions)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new FindMessageListReq();
            req.ConversationsArgs.Add(findMessageOptions);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.FindMessageList, req);
        }
        public static void GetHistoryMessageList(Action<GetHistoryMessageListResp> cb, string conversationId, string startClientMsgId, int count, bool isReverse)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetHistoryMessageList, new GetHistoryMessageListReq
            {
                ConversationID = conversationId,
                StartClientMsgID = startClientMsgId,
                Count = count,
                IsReverse = isReverse,
            });
        }
        public static void RevokeMessage(Action<bool> cb, string conversationId, string clientMsgId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.RevokeMessage, new RevokeMessageReq
            {
                ConversationID = conversationId,
                ClientMsgID = clientMsgId
            });
        }
        public static void TypingStatusUpdate(Action<bool> cb, string recvId, string msgTip)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.TypingStatusUpdate, new TypingStatusUpdateReq
            {
                RecvID = recvId,
                MsgTip = msgTip
            });
        }
        public static void MarkConversationMessageAsRead(Action<bool> cb, string conversationId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.MarkConversationMessageAsRead, new MarkConversationMessageAsReadReq
            {
                ConversationID = conversationId
            });
        }
        public static void MarkAllConversationMessageAsRead(Action<bool> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.MarkAllConversationMessageAsRead, new MarkConversationMessageAsReadReq { });
        }
        public static void DeleteMessageFromLocal(Action<bool> cb, string conversationId, string clientMsgId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.DeleteMessageFromLocal, new DeleteMessageFromLocalReq
            {
                ConversationID = conversationId,
                ClientMsgID = clientMsgId
            });
        }
        public static void DeleteMessage(Action<bool> cb, string conversationId, string clientMsgId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.DeleteMessage, new DeleteMessageReq
            {
                ConversationID = conversationId,
                ClientMsgID = clientMsgId
            });
        }
        public static void DeleteAllMsgFromLocalAndServer(Action<bool> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.DeleteAllMsgFromLocalAndServer, new DeleteAllMsgFromLocalAndServerReq { });
        }
        public static void DeleteAllMessageFromLocal(Action<bool> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.DeleteAllMessageFromLocal, new DeleteAllMessageFromLocalReq { });
        }
        public static void ClearConversationAndDeleteAllMsg(Action<bool> cb, string conversationId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.ClearConversationAndDeleteAllMsg, new ClearConversationAndDeleteAllMsgReq
            {
                ConversationID = conversationId
            });
        }
        public static void DeleteConversationAndDeleteAllMsg(Action<bool> cb, string conversationId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.DeleteConversationAndDeleteAllMsg, new DeleteConversationAndDeleteAllMsgReq
            {
                ConversationID = conversationId
            });
        }
        public static void InsertSingleMessageToLocal(Action<IMMessage> cb, IMMessage message, string recvId, string sendId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.InsertSingleMessageToLocal, new InsertSingleMessageToLocalReq
            {
                Msg = message,
                RecvID = recvId,
                SendID = sendId
            });
        }
        public static void InsertGroupMessageToLocalStorage(Action<IMMessage> cb, IMMessage message, string groupId, string sendId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.InsertGroupMessageToLocal, new InsertGroupMessageToLocalReq
            {
                Msg = message,
                GroupID = groupId,
                SendID = sendId
            });
        }
        public static void SearchLocalMessages(Action<int, SearchByConversationResult[]> cb, SearchLocalMessagesReq req)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.SearchLocalMessages, req);
        }
        public static void SearchConversation(Action<IMConversation[]> cb, string searchParam)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.SearchConversation, new SearchConversationReq
            {
                SearchParam = searchParam
            });
        }
        public static void SetMessageLocalEx(Action<bool> cb, string conversationId, string clientMsgId, string localEx)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.SetMessageLocalEx, new SetMessageLocalExReq
            {
                ConversationID = conversationId,
                ClientMsgID = clientMsgId,
                LocalEx = localEx
            });
        }
        #endregion

        #region user
        public static void GetUsersInfo(Action<IMUser[]> cb, string[] userIds)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new GetUsersInfoReq();
            req.UserIDs.Add(userIds);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetUsersInfo, req);
        }
        public static void SetSelfInfo(Action<bool> cb, SetSelfInfoReq req)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.SetSelfInfo, req);
        }
        public static void GetSelfUserInfo(Action<IMUser> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetSelfUserInfo, new GetSelfUserInfoReq());
        }
        public static void SubscribeUsersOnlineStatus(Action<UserOnlinePlatform[]> cb, string[] userIds)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new SubscribeUsersOnlineStatusReq();
            req.UserIDs.Add(userIds);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.SubscribeUsersOnlineStatus, req);
        }
        public static void UnsubscribeUsersOnlineStatus(Action<bool> cb, string[] userIds)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new UnsubscribeUsersOnlineStatusReq();
            req.UserIDs.Add(userIds);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.UnsubscribeUsersOnlineStatus, req);
        }
        public static void ProcessUserCommandGetAll(Action<CommandInfo[]> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.ProcessUserCommandGetAll, new ProcessUserCommandGetAllReq { });
        }
        public static void ProcessUserCommandAdd(Action<bool> cb, string userId, int type, string uuid, string value, string ex)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.ProcessUserCommandAdd, new ProcessUserCommandAddReq
            {
                UserID = userId,
                Type = type,
                Uuid = uuid,
                Value = value,
                Ex = ex
            });
        }
        public static void ProcessUserCommandDelete(Action<bool> cb, string userId, int type, string uuid)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.ProcessUserCommandDelete, new ProcessUserCommandDeleteReq
            {
                UserID = userId,
                Type = type,
                Uuid = uuid,
            });
        }
        public static void ProcessUserCommandUpdate(Action<bool> cb, string userId, int type, string uuid, string value, string ex)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.ProcessUserCommandUpdate, new ProcessUserCommandUpdateReq
            {
                UserID = userId,
                Type = type,
                Uuid = uuid,
                Value = value,
                Ex = ex
            });
        }
        #endregion

        #region friend
        public static void GetSpecifiedFriends(Action<IMFriend[]> cb, string[] userIdList, bool filterBlack)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new GetSpecifiedFriendsReq
            {
                FilterBlack = filterBlack
            };
            req.FriendUserIDs.Add(userIdList);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetSpecifiedFriends, req);
        }
        public static void GetFriends(Action<IMFriend[]> cb, bool filterBlack)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetFriends, new GetFriendsReq
            {
                FilterBlack = filterBlack
            });
        }
        public static void GetFriendsPage(Action<IMFriend[]> cb, int pageNumber, int showNumber, bool filterBlack)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetFriendsPage, new GetFriendsPageReq
            {
                FilterBlack = filterBlack,
                Pagination = new RequestPagination
                {
                    PageNumber = pageNumber,
                    ShowNumber = showNumber,
                }
            });
        }

        public static void SearchFriends(Action<SearchFriendsInfo[]> cb, string keyWord, bool searchUserId, bool searchNickName, bool searchRemark)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.SearchFriends, new SearchFriendsReq
            {
                Keyword = keyWord,
                SearchUserID = searchUserId,
                SearchNickname = searchNickName,
                SearchRemark = searchRemark
            });
        }
        public static void UpdateFriend(Action<bool> cb, string userId, bool pinned, string remark, string ex)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.UpdateFriend, new UpdateFriendReq
            {
                UserID = userId,
                Pinned = pinned,
                Remark = remark,
                Ex = ex,
            });
        }
        public static void CheckFriend(Action<CheckFriendInfo[]> cb, string[] userIdList)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new CheckFriendReq { };
            req.FriendUserIDs.Add(userIdList);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CheckFriend, req);
        }
        public static void AddFriend(Action<bool> cb, string userId, string reqMsg, string ex)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.AddFriend, new AddFriendReq
            {
                UserID = userId,
                ReqMsg = reqMsg,
                Ex = ex,
            });
        }
        public static void DeleteFriend(Action<bool> cb, string friendUserId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.DeleteFriend, new DeleteFriendReq
            {
                UserID = friendUserId
            });
        }
        public static void GetFriendApplication(Action<IMFriendApplication[]> cb, bool isSender)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetFriendApplication, new GetFriendApplicationReq
            {
                Send = isSender
            });
        }

        public static void HandleFriendApplication(Action<bool> cb, string userId, string handleMsg, ApprovalStatus status)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.HandleFriendApplication, new HandleFriendApplicationReq
            {
                UserID = userId,
                HandleMsg = handleMsg,
                Status = status,
            });
        }
        public static void AddBlack(Action<bool> cb, string userId, string ex)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.AddBlack, new AddBlackReq
            {
                UserID = userId,
                Ex = ex
            });
        }
        public static void GetBlacks(Action<IMBlack[]> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetBlacks, new GetBlacksReq { });
        }
        public static void DeleteBlack(Action<bool> cb, string removeUserId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.DeleteBlack, new DeleteBlackReq
            {
                UserID = removeUserId
            });
        }
        #endregion

        #region group
        public static void CreateGroup(Action<IMGroup> cb, CreateGroupReq req)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.CreateGroup, req);
        }
        public static void JoinGroup(Action<bool> cb, string groupId, string reqMsg, GroupJoinSource joinSource, string ex)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.JoinGroup, new JoinGroupReq
            {
                GroupID = groupId,
                ReqMsg = reqMsg,
                JoinSource = joinSource,
                Ex = ex
            });
        }
        public static void QuitGroup(Action<bool> cb, string groupId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.QuitGroup, new QuitGroupReq
            {
                GroupID = groupId
            });
        }

        public static void DismissGroup(Action<bool> cb, string groupId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.DismissGroup, new DismissGroupReq
            {
                GroupID = groupId
            });
        }
        public static void ChangeGroupMute(Action<bool> cb, string groupId, bool isMute)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.ChangeGroupMute, new ChangeGroupMuteReq
            {
                GroupID = groupId,
                Mute = isMute,
            });
        }
        public static void ChangeGroupMemberMute(Action<bool> cb, string groupId, string userId, uint mutedSeconds)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.ChangeGroupMemberMute, new ChangeGroupMemberMuteReq
            {
                GroupID = groupId,
                UserID = userId,
                MutedSeconds = mutedSeconds
            });
        }
        public static void SetGroupMemberInfo(Action<bool> cb, string groupId, string userId, string nickName, string faceUrl, GroupMemberRoleLevel roleLevel, string ex)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.SetGroupMemberInfo, new SetGroupMemberInfoReq
            {
                GroupID = groupId,
                UserID = userId,
                Nickname = nickName,
                FaceURL = faceUrl,
                RoleLevel = roleLevel,
                Ex = ex
            });
        }
        public static void GetJoinedGroups(Action<IMGroup[]> cb)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetJoinedGroups, new GetJoinedGroupsReq { });
        }
        public static void GetJoinedGroupsPage(Action<IMGroup[]> cb, int pageNumber, int showNumber)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetJoinedGroupsPage, new GetJoinedGroupsPageReq
            {
                Pagination = new RequestPagination
                {
                    PageNumber = pageNumber,
                    ShowNumber = showNumber
                }
            });
        }
        public static void GetSpecifiedGroupsInfo(Action<IMGroup[]> cb, string[] groupIdList)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new GetSpecifiedGroupsInfoReq();
            req.GroupIDs.Add(groupIdList);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetSpecifiedGroupsInfo, req);
        }
        public static void SearchGroups(Action<IMGroup[]> cb, string keyWord, bool searchGroupId, bool searchGroupName)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.SearchGroups, new SearchGroupsReq
            {
                Keyword = keyWord,
                SearchGroupID = searchGroupId,
                SearchGroupName = searchGroupName
            });
        }
        public static void SetGroupInfo(Action<bool> cb, SetGroupInfoReq req)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.SetGroupInfo, req);
        }
        public static void GetGroupMembers(Action<IMGroupMember[]> cb, string groupId, GroupMemberFilter filter, int pageNumber, int showNumber)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetGroupMembers, new GetGroupMembersReq
            {
                GroupID = groupId,
                Filter = filter,
                Pagination = new RequestPagination
                {
                    PageNumber = pageNumber,
                    ShowNumber = showNumber
                }
            });
        }
        public static void GetGroupMemberOwnerAndAdmin(Action<IMGroupMember[]> cb, string groupId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetGroupMemberOwnerAndAdmin, new GetGroupMemberOwnerAndAdminReq
            {
                GroupID = groupId
            });
        }
        public static void GetGroupMembersByJoinTimeFilter(Action<IMGroupMember[]> cb, string groupId, long joinTimeBegin, long joinTimeEnd, int pageNumber, int showNumber)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new GetGroupMembersByJoinTimeFilterReq
            {
                GroupID = groupId,
                JoinTimeBegin = joinTimeBegin,
                JoinTimeEnd = joinTimeEnd,
                Pagination = new RequestPagination
                {
                    PageNumber = pageNumber,
                    ShowNumber = showNumber
                }
            };
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetGroupMembersByJoinTimeFilter, req);
        }
        public static void GetSpecifiedGroupMembersInfo(Action<IMGroupMember[]> cb, string groupId, string[] userIdList)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new GetSpecifiedGroupMembersInfoReq
            {
                GroupID = groupId
            };
            req.UserIDs.Add(userIdList);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetSpecifiedGroupMembersInfo, req);
        }
        public static void KickGroupMember(Action<bool> cb, string groupId, string reason, string[] userIdList)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new KickGroupMemberReq
            {
                GroupID = groupId,
                Reason = reason
            };
            req.KickedUserIDs.Add(userIdList);
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.KickGroupMember, req);
        }
        public static void TransferGroupOwner(Action<bool> cb, string groupId, string ownerUserId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.TransferGroupOwner, new TransferGroupOwnerReq
            {
                GroupID = groupId,
                OwnerUserID = ownerUserId
            });
        }
        public static void InviteUserToGroup(Action<bool> cb, string groupId, string reason, string[] userIdList)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new InviteUserToGroupReq
            {
                GroupID = groupId,
                Reason = reason,
            };
            req.UserIDs.Add(userIdList);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.InviteUserToGroup, req);
        }
        public static void GetGroupApplication(Action<IMGroupApplication[]> cb, bool isSender)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetGroupApplication, new GetGroupApplicationReq
            {
                Send = isSender
            });
        }
        public static void HandleGroupApplication(Action<bool> cb, string groupId, string fromUserId, string handleMsg, ApprovalStatus status)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.HandleGroupApplication, new HandleGroupApplicationReq
            {
                GroupID = groupId,
                FromUserID = fromUserId,
                HandledMsg = handleMsg,
                Status = status,
            });
        }

        public static void SearchGroupMembers(Action<IMGroupMember> cb, string groupId, string keyWord, bool searchUserId, bool searchNickName, int pageNumber, int showNumber)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.SearchGroupMembers, new SearchGroupMembersReq
            {
                GroupID = groupId,
                Keyword = keyWord,
                SearchUserID = searchUserId,
                SearchMemberNickname = searchNickName,
                Pagination = new RequestPagination
                {
                    PageNumber = pageNumber,
                    ShowNumber = showNumber
                }
            });
        }
        public static void IsJoinGroup(Action<bool> cb, string groupId)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.IsJoinGroup, new IsJoinGroupReq
            {
                GroupID = groupId
            });
        }
        public static void GetUsersInGroup(Action<string[]> cb, string groupId, string[] userIdList)
        {
            var handleId = GetHandleId();
            callBackDic[handleId] = cb;
            var req = new GetUsersInGroupReq
            {
                GroupID = groupId,
            };
            req.UserIDs.Add(userIdList);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.GetUsersInGroup, req);
        }
        #endregion

        public static void UploadLogs(IUploadLogs cb, int line, string ex, UploadSDKDataMode mode)
        {
            var handleId = GetHandleId();
            uploadLogsCallBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.UploadLogs, new UploadSDKDataReq
            {
                Line = line,
                Ex = ex,
                Mode = mode
            });
        }
        public static void UploadFile(IUploadFile cb, string filePath, string name, string mimeType, string fileCategory)
        {
            var handleId = GetHandleId();
            uploadFileCallBackDic[handleId] = cb;
            NativeSDK.CallAPI(handleId, FuncRequestEventName.UploadFile, new UploadFileReq
            {
                Filepath = filePath,
                Name = name,
                MimeType = mimeType,
                FileCategory = fileCategory
            });
        }
        public static void Log(LogLevel level, string file, int line, string msg, string err, LogKv[] kvs)
        {
            var handleId = GetHandleId();
            var req = new LogReq
            {
                LogLevel = level,
                File = file,
                Line = line,
                Msg = msg,
                Err = err,
            };
            req.Kvs.Add(kvs);
            NativeSDK.CallAPI(handleId, FuncRequestEventName.Log, req);
        }
    }
}