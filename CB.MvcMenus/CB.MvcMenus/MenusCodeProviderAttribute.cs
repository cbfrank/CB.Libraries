using System;

namespace CB.MvcMenus
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class MenusCodeProviderAttribute : Attribute
    {
        public MenusCodeProviderAttribute()
        {
        }
    }
}