using System.Collections.Generic;

namespace CB.MvcMenus
{
    public class MenusProviderMetadata
    {
        public MenusProviderMetadata()
        {
            ChildrenMenus = new List<MenusProviderMetadata>();
        }

        public string Name { get; set; }
        public string IconCSS { get; set; }
        public string ActionUrl { get; set; }
        public IEMode IEMode { get; set; }
        internal int Order { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// if true, then it will not be shown if it has no children menus
        /// </summary>
        public bool HideIfNoChildren { get; set; }

        public List<MenusProviderMetadata> ChildrenMenus { get; private set; }
    }
}