using Microsoft.AspNet.Authentication.OAuth;
using Microsoft.AspNet.Http;

namespace CB.AspNet.Authentication.QQ
{
    /// <summary>
    /// Configuration options for <see cref="QQMiddleware"/>.
    /// </summary>
    public class QQOptions : OAuthOptions
    {
        /// <summary>
        /// Initializes a new <see cref="QQOptions"/>.
        /// The ClientId is Required by qq, which is called AppId in QQ, get it with the steps on the page http://wiki.connect.qq.com/%e5%87%86%e5%a4%87%e5%b7%a5%e4%bd%9c_oauth2-0
        /// </summary>
        public QQOptions() : base()
        {
            AuthenticationScheme = QQDefaults.AUTHENTICATION_SCHEME;
            DisplayName = AuthenticationScheme;
            CallbackPath = new PathString("/signin-qq");
            AuthorizationEndpoint = QQDefaults.AUTHORIZATION_ENDPOINT;
            TokenEndpoint = QQDefaults.TOKEN_ENDPOINT;
            UserInformationEndpoint = QQDefaults.USER_INFORMATION_ENDPOINT;
            SaveTokensAsClaims = true;
            IsWap = false;
        }

        public bool IsWap { get; set; }

#if DEVTEST
        public string CallbackFullPathForDebug { get; set; }
#endif

        public static QQOptions CreateOption(bool isWap = false)
        {
            var option = new QQOptions();
            if (isWap)
            {
                if (option.AuthorizationEndpoint == QQDefaults.AUTHORIZATION_ENDPOINT)
                {
                    option.AuthorizationEndpoint = QQDefaults.AUTHORIZATION_ENDPOINT_WAP;
                }
                if (option.TokenEndpoint == QQDefaults.TOKEN_ENDPOINT)
                {
                    option.TokenEndpoint = QQDefaults.TOKEN_ENDPOINT_WAP;
                }
                if (option.UserInformationEndpoint == QQDefaults.USER_INFORMATION_ENDPOINT)
                {
                    option.UserInformationEndpoint = QQDefaults.USER_INFORMATION_ENDPOINT_WAP;
                }
            }
            option.IsWap = isWap;
            return option;
        }
    }
}