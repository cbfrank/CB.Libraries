using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication.OAuth;
using Microsoft.AspNet.Http.Extensions;
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

        
    }
}
