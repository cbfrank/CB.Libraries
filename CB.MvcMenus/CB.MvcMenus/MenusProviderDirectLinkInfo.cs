using System;
using System.Reflection;
using System.Web.Mvc;

namespace CB.MvcMenus
{
    public class MenusProviderDirectLinkInfo : MenusProviderControllerInfo
    {
        public MenusProviderDirectLinkInfo(IMenuInformation attribute, string actionUrl) : base(null, null, attribute)
        {
            if (string.IsNullOrEmpty(actionUrl))
            {
                throw new ArgumentNullException(nameof(actionUrl));
            }
            ActionUrl = actionUrl;
        }

        public string ActionUrl { get; }

        public override string GetMenuUrl(UrlHelper urlHelper)
        {
            return ActionUrl;
        }
    }
}