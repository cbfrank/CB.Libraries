using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace CB.MvcMenus
{
    public class MenusProviderControllerInfo
    {
        public MenusProviderControllerInfo(Type controllerType, MethodInfo menuActionMethodInfo, IMenuInformation attribute)
        {
            Attribute = attribute;
            //ControllerType = controllerType;
            if (controllerType != null)
            {
                ControllerDescriptor = new ReflectedControllerDescriptor(controllerType);
            }
            UnionAuthorizationFilters = new IAuthorizationFilter[0];
            if (controllerType != null && menuActionMethodInfo != null)
            {
                ActionDescriptor = new ReflectedActionDescriptor(menuActionMethodInfo, menuActionMethodInfo.Name, ControllerDescriptor);
                UnionAuthorizationFilters = ActionDescriptor.GetFilterAttributes(true).OfType<IAuthorizationFilter>().ToArray();
                if (!UnionAuthorizationFilters.Any())
                {
                    UnionAuthorizationFilters = ControllerDescriptor.GetFilterAttributes(true).OfType<IAuthorizationFilter>().ToArray();
                }
            }
        }

        //public Type ControllerType { get; }
        public IMenuInformation Attribute { get; }

        protected ControllerDescriptor ControllerDescriptor { get; }
        public ActionDescriptor ActionDescriptor { get; protected set; }
        
        /// <summary>
        /// the IAuthorizationFilter on action, if there is no, then the IAuthorizationFilter on controller
        /// </summary>
        public IEnumerable<IAuthorizationFilter> UnionAuthorizationFilters { get; }

        public virtual string GetMenuUrl(UrlHelper urlHelper)
        {
            if (ActionDescriptor == null || ControllerDescriptor == null)
            {
                return string.Empty;
            }
            return urlHelper.Action(ActionDescriptor.ActionName, ControllerDescriptor.ControllerName, Attribute.ActionRouteValues);
        }
    }
}