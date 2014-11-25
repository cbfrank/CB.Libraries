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

        /// <summary>
        /// if provide, then the values will be passed to the action of controller
        /// </summary>
        object ActionRouteValues { get; set; }
    }
}