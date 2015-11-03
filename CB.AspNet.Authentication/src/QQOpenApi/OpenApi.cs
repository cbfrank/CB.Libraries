using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CB.QQ.QQOpenApi
{
    public class OpenApi : IDisposable
    {
        public OpenApi(OpenApiOption option)
        {
            Options = option;
            HttpClient = new HttpClient();
        }

        public HttpClient HttpClient { get; private set; }

        public OpenApiOption Options { get; private set; }

        protected virtual async Task<JObject> GetAsync(string uri, params KeyValuePair<string, string>[] queryParams )
        {
            var requestParams = new QueryBuilder
            {
                { "access_token", Options.AccessToken },
                { "oauth_consumer_key", Options.AppId },
                { "openid", Options.OpenId }
            };
            foreach (var param in queryParams)
            {
                requestParams.Add(param.Key, param.Value);
            }
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri + requestParams);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await HttpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<JObject> GetUserInfoAsync()
        {
            return await GetAsync("https://graph.qq.com/user/get_user_info");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (HttpClient != null)
                {
                    HttpClient.Dispose();
                    HttpClient = null;
                }
            }
        }

        ~OpenApi()
        {
            Dispose(false);
        }
    }
}
