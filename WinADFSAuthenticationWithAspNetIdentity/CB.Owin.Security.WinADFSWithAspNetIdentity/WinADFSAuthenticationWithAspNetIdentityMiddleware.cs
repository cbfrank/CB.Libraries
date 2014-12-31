using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;

namespace CB.Owin.Security.ADFS
{
    public class WindowsAndADFSAuthenticationMiddleware<TUser, TKey, TSignInManager> : AuthenticationMiddleware<WinADFSAuthenticationWithAspNetIdentityOptions<TSignInManager>>
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>, IConvertible
        where TSignInManager : SignInManager<TUser, TKey>
    {
        public WindowsAndADFSAuthenticationMiddleware(OwinMiddleware next, WinADFSAuthenticationWithAspNetIdentityOptions<TSignInManager> options)
            : base(next, options)
        {
        }

        protected override AuthenticationHandler<WinADFSAuthenticationWithAspNetIdentityOptions<TSignInManager>> CreateHandler()
        {
            return new WinADFSAuthenticationWithAspNetIdentityHandler<TUser, TKey, TSignInManager>();
        }
    }
}