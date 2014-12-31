using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace CB.Owin.Security.ADFS
{
    public class WinADFSAuthenticationWithAspNetIdentityProvider<TSignInManager> : IWinADFSAuthenticationWithAspNetIdentityProvider<TSignInManager>
    {
        public Func<ClaimsPrincipal, string> GetLoginProviderKey { get; set; }
        public Func<ClaimsPrincipal, string> GetLoginProvider { get; set; }
        public Func<IOwinContext, TSignInManager> GetSignInManager { get; set; }
        public Func<IOwinContext, AuthenticationTicket, Task<bool>> OnAuthenticatedAsync { get; set; }
    }
}