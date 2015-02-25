using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace CB.Owin.Security.ADFS
{
    public class WinADFSAuthenticationWithAspNetIdentityHandler<TUser, TKey, TSignInManager> :
        AuthenticationHandler<WinADFSAuthenticationWithAspNetIdentityOptions<TSignInManager>>
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>, IConvertible
        where TSignInManager : SignInManager<TUser, TKey>
    {
        public override async Task<bool> InvokeAsync()
        {
            //Please NOTE: if Options.SignInPersistent is true, and next time reopen the web site, user will get it directly from cookie and windows authentication won't be called
            if (Context.Authentication.User.Identities.All(i => i.AuthenticationType != Options.AuthenticationType))
            {
                var ticket = await AuthenticateAsync();
                if (Options.Provider.OnAuthenticatedAsync != null)
                {
                    return await Options.Provider.OnAuthenticatedAsync(Context, ticket);
                }
                else
                {
                    if (ticket != null)
                    {
                        return false;
                    }
                    var unauthorizedContent = "You are not allowed to be used this web site.";
                    if (Options.Provider.CustomUnauthorizedContent != null)
                    {
                        unauthorizedContent = Options.Provider.CustomUnauthorizedContent(Context);
                    }
                    if (string.IsNullOrEmpty(unauthorizedContent))
                    {
                        Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }
                    else
                    {
                        Context.Response.Write(unauthorizedContent);
                    }
                    return true;
                }
            }
            else if (Options.SignInAgainWhenOnlyWinADFSAuthenticationWithAspNetIdentityAuthenticationType &&
                     Context.Authentication.User.Identities.All(i => i.AuthenticationType == Options.AuthenticationType))
            {
                //if only have ECO_AUTHENTICATION_TYPE, then mean the coolie is still valid, so the authentication get it directly, and windows / ADFS won't be called
                //so we have to force user logout of the ECO_AUTHENTICATION_TYPE and force them to refresh
                Context.Authentication.SignOut(Options.AuthenticationType);
                Context.Response.Redirect(Context.Request.Path.ToString());
            }
            return false;
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationTicket ticket = null;

            var signInManager = Options.Provider.GetSignInManager == null ? Context.Get<TSignInManager>() : Options.Provider.GetSignInManager(Context);
            if (signInManager.AuthenticationType != Options.AuthenticationType)
            {
                throw new ArgumentException(
                    string.Format(
                        "The instance of type {0} has the AuthenticationType different from the Options.AuthenticationType ({1}), they should be same",
                        typeof (TSignInManager).FullName, Options.AuthenticationType));
            }

            var userClaimsPrincipal = Context.Authentication.User;
            var loginProviderKey = Options.Provider.GetLoginProviderKey(userClaimsPrincipal);
            //if providerKey is not found, then user Indentity.Name in this case this is because it is windows authentication
            loginProviderKey = string.IsNullOrEmpty(loginProviderKey)
                ? userClaimsPrincipal.Identity.Name
                : loginProviderKey;
            var loginProvider = userClaimsPrincipal.Identity.AuthenticationType;
            if (Options.Provider.GetLoginProvider != null)
            {
                loginProvider = Options.Provider.GetLoginProvider(userClaimsPrincipal);
            }
            //find the user with login infor
            var user = await signInManager.UserManager.FindAsync(new UserLoginInfo(loginProvider, loginProviderKey));
            if (user != null)
            {
                await signInManager.SignInAsync(user, Options.SignInPersistent, Options.RememberBrowser);
                var identity = await signInManager.CreateUserIdentityAsync(user);
                Helper.AddUserIdentity(identity);
                ticket = new AuthenticationTicket(
                    identity, new AuthenticationProperties
                    {
                        IsPersistent = Options.SignInPersistent,
                        IssuedUtc = DateTimeOffset.UtcNow,
                        ExpiresUtc = DateTimeOffset.UtcNow + Options.ExpireTimeSpan
                    });
            }

            return ticket;
        }
    }
}