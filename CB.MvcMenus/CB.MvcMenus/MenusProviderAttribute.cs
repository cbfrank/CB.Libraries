using System;
using System.Web.UI.WebControls;

namespace CB.MvcMenus
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class MenusProviderAttribute : Attribute, IMenuInformation
    {
        /// <summary>
        /// must be unique
        /// </summary>
        public string NameKey { get; private set; }
        public string IconCSS { get; set; }
        public int Order { get; set; }
        public IEMode IEMode { get; set; }

        /// <summary>
        /// if provide, then the values will be passed to the action of controller
        /// </summary>
        object IMenuInformation.ActionRouteValues { get; set; }
        public string ParentMenuNameKey { get; set; }
        public string Title { get; set; }

        public MenusProviderAttribute(string nameKey)
        {
            Title = NameKey = nameKey;
            Order = 0;
            IEMode = IEMode.Edge;
            ParentMenuNameKey = string.Empty;
            ((IMenuInformation) this).ActionRouteValues = null;
        }
    }
}