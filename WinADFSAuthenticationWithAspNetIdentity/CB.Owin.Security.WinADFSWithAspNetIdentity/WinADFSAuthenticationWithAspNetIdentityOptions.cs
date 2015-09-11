using System;
using Microsoft.Owin.Security;

namespace CB.Owin.Security.ADFS
{
    public class WinADFSAuthenticationWithAspNetIdentityOptions<TSignInManager> : AuthenticationOptions
    {
        public WinADFSAuthenticationWithAspNetIdentityOptions(string authenticationType)
            : base(authenticationType)
        {
            AuthenticationMode = AuthenticationMode.Passive;
            ExpireTimeSpan = new TimeSpan(1, 0, 0);
            SignInPersistent = false;
            RememberBrowser = false;
            //SignInAgainWhenOnlyWinADFSAuthenticationWithAspNetIdentityAuthenticationType = true;
            Provider = new WinADFSAuthenticationWithAspNetIdentityProvider<TSignInManager>();
        }

        /// <summary>
        /// default is 1 hours
        /// </summary>
        public TimeSpan ExpireTimeSpan { get; set; }
        /// <summary>
        /// Default is false, that is to say, every time, when user close the web pages and open it again, the authentication will alwasy
        /// query the user infromation (roles, claims and login infor) from database, and then the Identities contains at least two 
        /// identities, one is the windows identity (or ADFS identity), the other is the identity of type WindowsAndADFSAuthenticationOptions.AuthenticationType which is created from the user informatio in dabtabase
        /// if it is set to true, then when user close the web pages and reopen it, if the time it no longer than ExpireTimeSpan
        /// then the identities will only contains one identity, which is of type WindowsAndADFSAuthenticationOptions.AuthenticationType
        /// and is build from the cookie which is based on the data in database last time the user is readed
        /// in this case, there is no windows identity (or ADFS identity)
        /// so if the claims in windows identity (or ADFS identity) is importaint to your application
        /// please remember copy these claims (of windows identity (or ADFS identity)) in the overwirte method TSignInManager.CreateUserIdentityAsync (or TSignInManager.UserManager.CreateIdentityAsync)
        /// otherwise, you won't get these claims when SignInPersistent is true and user cloase web pages and reopen again in the ExpireTimeSpan
        /// if you want to custom the claims you also need to write the logic in the overwirte method TSignInManager.CreateUserIdentityAsync (or TSignInManager.UserManager.CreateIdentityAsync)
        /// </summary>
        public bool SignInPersistent { get; set; }
        /// <summary>
        /// when the Identities only contains the identity of authenticationType as current AuthenticationType, then sign out and sign in again to make sure
        /// windows authention or ADFS happens
        /// this case can be the SignInPersistent is set to true and user close browser and open again, then the authention get from cookie and windows authention or ADFS won't happen
        /// defualt is true
        /// </summary>
        //public bool SignInAgainWhenOnlyWinADFSAuthenticationWithAspNetIdentityAuthenticationType { get; set; }
        public bool RememberBrowser { get; set; }
        public IWinADFSAuthenticationWithAspNetIdentityProvider<TSignInManager> Provider { get; set; }
    }
}