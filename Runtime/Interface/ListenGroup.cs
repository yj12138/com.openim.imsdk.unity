namespace OpenIM.IMSDK.Unity.Listener
{
    public class ListenGroup
    {
        public IConnListener ConnListener;
        public IConversationListener ConversationListener;
        public IGroupListener GroupListener;
        public IFriendShipListener FriendShipListener;
        public IAdvancedMsgListener AdvancedMsgListener;
        public IUserListener UserListener;
        public ICustomBusinessListener CustomBusinessListener;
        public IBatchMsgListener BatchMsgListener;
        public ListenGroup(
            IConnListener connListener,
            IConversationListener conversationListener,
            IGroupListener groupListener,
            IFriendShipListener friendShipListener,
            IAdvancedMsgListener advancedMsgListener,
            IUserListener userListener,
            ICustomBusinessListener customBusinessListener,
            IBatchMsgListener batchMsgListener
        )
        {
            this.ConnListener = connListener;
            this.ConversationListener = conversationListener;
            this.GroupListener = groupListener;
            this.FriendShipListener = friendShipListener;
            this.AdvancedMsgListener = advancedMsgListener;
            this.UserListener = userListener;
            this.CustomBusinessListener = customBusinessListener;
            this.BatchMsgListener = batchMsgListener;
        }
    }
}

