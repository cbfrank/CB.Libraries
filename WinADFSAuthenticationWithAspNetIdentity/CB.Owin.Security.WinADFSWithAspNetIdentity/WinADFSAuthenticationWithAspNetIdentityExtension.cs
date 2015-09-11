using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace CB.Owin.Security.ADFS
{
    public static class WindowsAndADFSAuthenticationExtension
    {
        public static IAppBuilder UserWindowsAndADFSAuthentication<TUser, TKey, TUserManager, TSignInManager>(this IAppBuilder app,
            WinADFSAuthenticationWithAspNetIdentityOptions<TSignInManager> options)
            where TUserManager : UserManager<TUser, TKey>
            where TUser : class, IUser<TKey>
            where TKey : IEquatable<TKey>, IConvertible
            where TSignInManager : SignInManager<TUser, TKey>
        {
            //var cookieOptions = new CookieAuthenticationOptions
            //{
            //    AuthenticationType = options.AuthenticationType,
            //    AuthenticationMode = AuthenticationMode.Active,
            //    CookieName = ".AspNet." + options.AuthenticationType,
            //    ExpireTimeSpan = options.ExpireTimeSpan,
            //    Provider = new CookieAuthenticationProvider
            //    {
            //        //        // Enables the application to validate the security stamp when the user logs in.
            //        //        // This is a security feature which is used when you change a password or add an external login to your account.  
            //        OnValidateIdentity = context => SecurityStampValidator.OnValidateIdentity<TUserManager, TUser, TKey>(
            //            TimeSpan.FromMinutes(30), (manager, user) => manager.CreateIdentityAsync(user, options.AuthenticationType),
            //            identity => identity.GetUserId<TKey>())(context)
            //    }
            //};
            //app.UseCookieAuthentication(cookieOptions);

            if (app == null)
                throw new ArgumentNullException("app");
            app.Use<WindowsAndADFSAuthenticationMiddleware<TUser, TKey, TSignInManager>>(options);
            return app;
        }
    }
}