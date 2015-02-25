using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace CB.Owin.Security.ADFS
{
    public interface IWinADFSAuthenticationWithAspNetIdentityProvider<TSignInManager>
    {
        Func<ClaimsPrincipal, string> GetLoginProviderKey { get; set; }
        Func<ClaimsPrincipal, string> GetLoginProvider { get; set; }
        Func<IOwinContext, TSignInManager> GetSignInManager { get; set; }
        Func<IOwinContext, AuthenticationTicket, Task<bool>> OnAuthenticatedAsync { get; set; }
        Func<IOwinContext, string> CustomUnauthorizedContent { get; set; }
    }
}
