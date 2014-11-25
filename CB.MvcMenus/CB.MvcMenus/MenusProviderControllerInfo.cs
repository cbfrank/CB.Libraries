using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CB.MvcMenus
{
    public class MenusProviderControllerInfo
    {
        public MenusProviderControllerInfo(Type controllerType, MethodInfo menuActionMethodInfo, IMenuInformation attribute)
        {
            Attribute = attribute;
            ControllerType = controllerType;
            ControllerDescriptor = new System.Web.Mvc.ReflectedControllerDescriptor(controllerType);
            UnionAuthorizationFilters = new System.Web.Mvc.IAuthorizationFilter[0];
            if (menuActionMethodInfo != null)
            {
                ActionDescriptor = new System.Web.Mvc.ReflectedActionDescriptor(menuActionMethodInfo, menuActionMethodInfo.Name, ControllerDescriptor);
                UnionAuthorizationFilters = ActionDescriptor.GetFilterAttributes(true).OfType<System.Web.Mvc.IAuthorizationFilter>().ToArray();
                if (!UnionAuthorizationFilters.Any())
                {
                    UnionAuthorizationFilters = ControllerDescriptor.GetFilterAttributes(true).OfType<System.Web.Mvc.IAuthorizationFilter>().ToArray();
                }
            }
        }

        public Type ControllerType { get; private set; }
        public IMenuInformation Attribute { get; private set; }

        public System.Web.Mvc.ControllerDescriptor ControllerDescriptor { get; private set; }
        public System.Web.Mvc.ActionDescriptor ActionDescriptor { get; private set; }
        
        /// <summary>
        /// the IAuthorizationFilter on action, if there is no, then the IAuthorizationFilter on controller
        /// </summary>
        public IEnumerable<System.Web.Mvc.IAuthorizationFilter> UnionAuthorizationFilters { get; private set; }
    }
}