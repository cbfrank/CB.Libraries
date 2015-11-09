using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication.OAuth;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.WebUtilities;
using Newtonsoft.Json.Linq;

namespace CB.AspNet.Authentication.QQ
{
    /// <summary>
    /// Contains static methods that allow to extract user's information from a <see cref="JObject"/>
    /// instance retrieved from QQ after a successful authentication process.
    /// </summary>
    public static class QQHelper
    {
        /// <summary>
        /// Gets the QQ user ID.
        /// </summary>
        public static string GetId(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("openid");
        }

        /// <summary>
        /// Gets the user's given name.
        /// </summary>
        //public static string GetGivenName(JObject user)
        //{
        //    if (user == null)
        //    {
        //        throw new ArgumentNullException(nameof(user));
        //    }

        //    return TryGetValue(user, "name", "givenName");
        //}

        public static JObject ConvertFormFormatStrToJson(string formFormatStr)
        {
            var form = new FormCollection(FormReader.ReadForm(formFormatStr));
            var properties = new object[form.Keys.Count];
            var index = 0;
            foreach (var item in form)
            {
                properties[index] = new JProperty(item.Key, item.Value[0]);
                index++;
            }
            return new JObject(properties);
        }

        public static async Task<JObject> RefreshAccessToken(QQOptions options, string refreshToken)
        {
            using (var client = new HttpClient())
            {
                var requestParams = new QueryBuilder
            {
                { "grant_type", "refresh_token" },
                { "client_id", options.ClientId },
                { "client_secret", options.ClientSecret },
                { "refresh_token", refreshToken }
            };

                var requestMessage = new HttpRequestMessage(HttpMethod.Get,
                    (options.IsWap ? QQDefaults.TOKEN_ENDPOINT_WAP : QQDefaults.TOKEN_ENDPOINT) + requestParams);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                requestMessage.Method = HttpMethod.Get;
                var response = await client.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return ConvertFormFormatStrToJson(responseContent);
            }
        }
    }
}
