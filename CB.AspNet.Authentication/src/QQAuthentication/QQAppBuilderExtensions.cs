using System;
using JetBrains.Annotations;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.OptionsModel;

namespace CB.AspNet.Authentication.QQ
{
    /// <summary>
    /// Extension methods for using <see cref="QQAuthenticationMiddleware"/>.
    /// </summary>
    public static class QQAppBuilderExtensions
    {
        public static IApplicationBuilder UseQQAuthentication(this IApplicationBuilder app, QQOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<QQMiddleware>(options);
        }

        public static IApplicationBuilder UseQQAuthentication(this IApplicationBuilder app, Action<QQOptions> configureOptions, bool isWap = false)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var options = QQOptions.CreateOption(isWap);
            if (configureOptions != null)
            {
                configureOptions(options);
            }
            return app.UseQQAuthentication(options);
        }
    }
}