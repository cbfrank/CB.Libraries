namespace CB.MvcMenus
{
    public interface IMenuInformation
    {
        /// <summary>
        /// must be unique
        /// </summary>
        string NameKey { get; }

        string IconCSS { get; set; }
        int Order { get; set; }
        string ParentMenuNameKey { get; set; }
        IEMode IEMode { get; set; }
        string Title { get; set; }
        /// <summary>
        /// default is false, if true, then it will not be shown if it has no children menus
        /// </summary>
        bool HideIfNoChildren { get; set; }

        /// <summary>
        /// if provide, then the values will be passed to the action of controller
        /// </summary>
        object ActionRouteValues { get; set; }
    }
}