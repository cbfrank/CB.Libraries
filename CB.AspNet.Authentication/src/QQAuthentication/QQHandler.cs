using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CB.QQ.QQOpenApi;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Authentication.OAuth;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.WebUtilities;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Http.Internal;

namespace CB.AspNet.Authentication.QQ
{
    internal class QQHandler : OAuthHandler<QQOptions>
    {
        public QQHandler(HttpClient httpClient)
            : base(httpClient)
        {
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            // Get the QQ OpenId
            var openIdQuery = new QueryBuilder()
            {
                { "access_token", tokens.AccessToken }
            };

            var openIdRequest = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint + openIdQuery.ToString());
            var payload = await CallJsonAsync(openIdRequest, true);

            var context = new OAuthCreatingTicketContext(Context, Options, Backchannel, tokens, payload)
            {
                Properties = properties,
                Principal = new ClaimsPrincipal(identity)
            };

            var identifier = QQHelper.GetId(payload);
            if (!string.IsNullOrEmpty(identifier))
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identifier, ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            using (var api = new OpenApi(new OpenApiOption {AccessToken = tokens.AccessToken, AppId = Options.ClientId, OpenId = identifier}))
            {
                var userInfo = await api.GetUserInfoAsync();
                identity.AddClaim(new Claim(ClaimTypes.Name, userInfo.Value<string>("nickname"), ClaimValueTypes.String, Options.ClaimsIssuer));
            }

            await Options.Events.CreatingTicket(context);

            return new AuthenticationTicket(context.Principal, context.Properties, context.Options.AuthenticationScheme);
        }

#if DEVTEST
        private string GetTestRedirectUriForDebug(string originalRedirectUri)
        {
            if (string.IsNullOrEmpty(Options.CallbackFullPathForDebug))
            {
                return originalRedirectUri;
            }
            return Options.CallbackFullPathForDebug;
        }
#endif

        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri)
        {
#if DEVTEST
            redirectUri = GetTestRedirectUriForDebug(redirectUri);
#endif
            var tokenRequestParameters = new Dictionary<string, string>()
            {
                { "client_id", Options.ClientId },
                { "redirect_uri", redirectUri },
                { "client_secret", Options.ClientSecret },
                { "code", code },
                { "grant_type", "authorization_code" },
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Options.TokenEndpoint);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Content = new FormUrlEncodedContent(tokenRequestParameters);
            
            return new OAuthTokenResponse(await CallFormAsync(requestMessage));
        }

        // TODO: Abstract this properties override pattern into the base class?
        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
#if DEVTEST
            redirectUri = GetTestRedirectUriForDebug(redirectUri);
#endif
            var scope = FormatScope();

            var queryStrings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"response_type", "code"},
                {"client_id", Options.ClientId},
                {"redirect_uri", redirectUri}
            };

            AddQueryString(queryStrings, properties, "scope", scope);
            if (Options.IsWap)
            {
                AddQueryString(queryStrings, properties, "display", "mobile"); //if is wap, then set to "mobile"     
                AddQueryString(queryStrings, properties, "g_ut", "1"); //optional parameter see http://wiki.connect.qq.com/%E4%BD%BF%E7%94%A8authorization_code%E8%8E%B7%E5%8F%96access_token#Step1.EF.BC.9A.E8.8E.B7.E5.8F.96AuthorizationCode    
            }

            var state = Options.StateDataFormat.Protect(properties);
            queryStrings.Add("state", state); //http://wiki.connect.qq.com/%E4%BD%BF%E7%94%A8authorization_code%E8%8E%B7%E5%8F%96access_token#Step1.EF.BC.9A.E8.8E.B7.E5.8F.96AuthorizationCode

            var authorizationEndpoint = QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, queryStrings);
            return authorizationEndpoint;
        }

        private static void AddQueryString(IDictionary<string, string> queryStrings, AuthenticationProperties properties,
            string name, string defaultValue = null)
        {
            string value;
            if (!properties.Items.TryGetValue(name, out value))
            {
                value = defaultValue;
            }
            else
            {
                // Remove the parameter from AuthenticationProperties so it won't be serialized to state parameter
                properties.Items.Remove(name);
            }

            if (value == null)
            {
                return;
            }

            queryStrings[name] = value;
        }

        /// <summary>
        /// send http request message and conver resule as fomr data and convert to a JObject
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<JObject> CallFormAsync(HttpRequestMessage message)
        {
            var response = await Backchannel.SendAsync(message, Context.RequestAborted);
            response.EnsureSuccessStatusCode();
            return QQHelper.ConvertFormFormatStrToJson(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// send http request message and read result as JObject
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callbackFormat">if true, the result is a json content but in "callback(json result)"</param>
        /// <returns></returns>
        private async Task<JObject> CallJsonAsync(HttpRequestMessage message, bool callbackFormat)
        {
            var response = await Backchannel.SendAsync(message, Context.RequestAborted);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            if (callbackFormat)
            {
                var regex = new Regex(@"callback\((?<json>.+)\)", RegexOptions.Compiled|RegexOptions.IgnoreCase);
                var match = regex.Match(content);
                if (match.Success)
                {
                    content = match.Groups["json"].Value;
                }
            }
            return JObject.Parse(content);
        }
    }
}