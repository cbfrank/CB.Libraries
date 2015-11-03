namespace CB.AspNet.Authentication.QQ
{
    public static class QQDefaults
    {
        public const string AUTHENTICATION_SCHEME = "QQ";

        public const string AUTHORIZATION_ENDPOINT = "https://graph.qq.com/oauth2.0/authorize";

        public const string AUTHORIZATION_ENDPOINT_WAP = "https://graph.z.qq.com/moc2/authorize";

        public const string TOKEN_ENDPOINT = "https://graph.qq.com/oauth2.0/token";

        public const string TOKEN_ENDPOINT_WAP = "https://graph.z.qq.com/moc2/token";

        public const string USER_INFORMATION_ENDPOINT = "https://graph.qq.com/oauth2.0/me";

        public const string USER_INFORMATION_ENDPOINT_WAP = "https://graph.z.qq.com/moc2/me";

        /// <summary>
        /// Defines all the scopes for the QQ
        /// </summary>
        /// <see cref="http://wiki.connect.qq.com/api%e5%88%97%e8%a1%a8"/>
        public static class QQScopes
        {
            /// <summary>
            /// The scope to get qq user information
            /// </summary>
            public const string GET_USER_INFO = "get_user_info";

            public const string GET_VIP_INFO = "get_vip_info";
            public const string GET_VIP_RICH_INFO = "get_vip_rich_info";
            public const string LIST_ALBUM = " list_album";
            public const string UPLOAD_PIC = "upload_pic";
            public const string ADD_ALBUM = "add_album";
            public const string LIST_PHOTO = "list_photo";
            public const string ADD_T = "add_t";
            public const string DEL_T = "del_t";
            public const string ADD_PIC_T = "add_pic_t";
            public const string GET_REPOST_LIST = "get_repost_list";
            public const string GET_OTHER_INFO = "get_other_info";
            public const string GET_FANSLIST = "get_fanslist";
            public const string GET_IDOLLIST = "get_idollist";
            public const string ADD_IDOL = "add_idol";
            public const string DEL_IDOL = "del_idol";

        }
    }
}
