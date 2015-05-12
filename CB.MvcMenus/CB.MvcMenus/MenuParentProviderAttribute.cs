using System;

namespace CB.MvcMenus
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class MenuParentProviderAttribute : Attribute, IMenuInformation
    {
        public MenuParentProviderAttribute(string nameKey)
        {
            Title = NameKey = nameKey;
            Order = 0;
            IEMode = IEMode.Edge;
            ParentMenuNameKey = string.Empty;
            ((IMenuInformation) this).ActionRouteValues = null;
            HideIfNoChildren = false;
        }

        /// <summary>
        /// must be unique
        /// </summary>
        public string NameKey { get; private set; }
        public string IconCSS { get; set; }
        public int Order { get; set; }
        public IEMode IEMode { get; set; }
        public string Title { get; set; }
        public bool HideIfNoChildren { get; set; }

        /// <summary>
        /// if provide, then the values will be passed to the action of controller
        /// </summary>
        object IMenuInformation.ActionRouteValues { get; set; }
        public string ParentMenuNameKey { get; set; }
    }
}