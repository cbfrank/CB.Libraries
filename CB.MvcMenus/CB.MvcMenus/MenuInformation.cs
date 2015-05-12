namespace CB.MvcMenus
{
    public class MenuInformation : IMenuInformation
    {
        public MenuInformation(string nameKey)
        {
            Title = NameKey = nameKey;
        }

        #region Implementation of IMenuInformation

        /// <summary>
        /// must be unique
        /// </summary>
        public string NameKey { get; private set; }
        public string IconCSS { get; set; }
        public int Order { get; set; }
        public string ParentMenuNameKey { get; set; }
        public IEMode IEMode { get; set; }
        public string Title { get; set; }
        public bool HideIfNoChildren { get; set; }

        /// <summary>
        /// if provide, then the values will be passed to the action of controller
        /// </summary>
        public object ActionRouteValues { get; set; }

        #endregion
    }
}