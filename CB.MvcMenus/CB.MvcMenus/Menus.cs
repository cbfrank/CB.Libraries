using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace CB.MvcMenus
{
    public static class Menus
    {
        private static List<MenusProviderControllerInfo> _FunctionsProviderControllerInfos;

        /// <summary>
        /// should be called before invoke RenderMenus, just call one time
        /// </summary>
        /// <param name="assemblies"></param>
        public static void FindAllMenus(params Assembly[] assemblies)
        {
            var controllers = (from t in assemblies.SelectMany(a => a.GetTypes())
                              where t.IsSubclassOf(typeof(Controller))
                              select t).ToArray();

            //is controller && marked with FunctionsProviderAttribute && has the entry action
            _FunctionsProviderControllerInfos =
                (from t in controllers
                    from m in t.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    let attributes = m.GetCustomAttributes(typeof (MenusProviderAttribute), false)
                    where attributes != null && attributes.Length > 0
                    //&& marked with MenusProviderAttribute
                    from a in attributes.Cast<MenusProviderAttribute>()
                    let controllerDescriptor = new ReflectedControllerDescriptor(t)
                    select new MenusProviderControllerInfo(t, m, a))
                    .Union(
                        from t in assemblies.SelectMany(a => a.GetTypes())
                        where t.IsSubclassOf(typeof (Controller))
                        from attr in t.GetCustomAttributes(typeof (MenuParentProviderAttribute), false)
                        select new MenusProviderControllerInfo(t, null, (MenuParentProviderAttribute) attr)).ToList();

            var methods = from m in controllers.SelectMany(c => c.GetMethods(BindingFlags.Public | BindingFlags.Static))
                let attributes = m.GetCustomAttributes(typeof (MenusCodeProviderAttribute), false)
                where attributes != null && attributes.Length > 0
                select m;
            foreach (var method in methods)
            {
                method.Invoke(null, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <param name="actionExpression">is the expression that call the action of the controller, for example controller=>controller.Index() </param>
        /// <param name="menuInformation"></param>
        public static void AddMenuInfo<TController>(Expression<Action<TController>> actionExpression, IMenuInformation menuInformation) where TController : Controller
        {
            MethodInfo method = null;
            if (actionExpression != null)
            {
                Debug.Assert(actionExpression.Body is MethodCallExpression, "actionExpression should be MethodCallExpression");
                method = ((MethodCallExpression) actionExpression.Body).Method;
            }
            _FunctionsProviderControllerInfos.Add(new MenusProviderControllerInfo(typeof (TController), method, menuInformation));
        }

        public static IEnumerable<MenusProviderMetadata> GetAllMenuMetadatas(ControllerContext controllerContext, UrlHelper urlHelper)
        {
            //key is the parent menu name key
            var groupsMenus = new Dictionary<string, List<MenusProviderMetadata>>();
            foreach (var controller in _FunctionsProviderControllerInfos)
            {
                var authorized = true;

                #region If Define MvcAuthorizeAttribute on Method or Controller, check if current user has the access

                if (controller.ActionDescriptor == null) //this is a parent menu, because only parent menu can marked on Controller, so for parent menu, we let it always authorized
                {

                }
                else
                {
                    var authorizationContext = new AuthorizationContext(controllerContext, controller.ActionDescriptor);
                    foreach (var filterAttribute in controller.UnionAuthorizationFilters)
                    {
                        filterAttribute.OnAuthorization(authorizationContext);
                        if (authorizationContext.Result is HttpUnauthorizedResult)
                        {
                            authorized = false;
                            break;
                        }
                    }
                    if (!authorized) continue;
                }

                #endregion

                var metadata = new MenusProviderMetadata
                {
                    IconCSS = controller.Attribute.IconCSS,
                    Name = controller.Attribute.NameKey,
                    IEMode = controller.Attribute.IEMode,
                    Order = controller.Attribute.Order
                };
                if (controller.ActionDescriptor != null)
                {
                    metadata.ActionUrl = urlHelper.Action(controller.ActionDescriptor.ActionName,
                        controller.ControllerDescriptor.ControllerName, controller.Attribute.ActionRouteValues);
                }
                if (!groupsMenus.ContainsKey(controller.Attribute.ParentMenuNameKey))
                {
                    groupsMenus[controller.Attribute.ParentMenuNameKey] = new List<MenusProviderMetadata>();
                }
                groupsMenus[controller.Attribute.ParentMenuNameKey].Add(metadata);
            }

            var result = new List<MenusProviderMetadata>();
            if (groupsMenus.ContainsKey(string.Empty))
            {
                result.AddRange(groupsMenus[string.Empty]);
                result.Sort((m1, m2) => m1.Order - m2.Order);
                foreach (var menu in result)
                {
                    FillChildrenMenus(menu, groupsMenus);
                }
            }
            return result;
        }

        public static IEnumerable<MenusProviderMetadata> GetAllMenuMetadatas(this WebViewPage page)
        {
            return GetAllMenuMetadatas(page.ViewContext, page.Url);
        }

        public static IEnumerable<MenusProviderMetadata> GetAllMenuMetadatas(this Controller controller)
        {
            return GetAllMenuMetadatas(controller.ControllerContext, controller.Url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="groupsMenus">key is the parent menu name key, so each item is a list of menu that has same parent menu</param>
        private static void FillChildrenMenus(MenusProviderMetadata metadata, Dictionary<string, List<MenusProviderMetadata>> groupsMenus)
        {
            metadata.ChildrenMenus.Clear();
            if (groupsMenus.ContainsKey(metadata.Name))
            {
                metadata.ChildrenMenus.AddRange(groupsMenus[metadata.Name]);
                metadata.ChildrenMenus.Sort((m1, m2) => m1.Order - m2.Order);
                foreach (var childMenu in metadata.ChildrenMenus)
                {
                    FillChildrenMenus(childMenu, groupsMenus);
                }
            }
        }

        private static string RenderMenu(MenusProviderMetadata menu,
            Func<MenusProviderMetadata, string> generateMenuFunc)
        {
            var menuHtml = generateMenuFunc(menu);
            var children = new StringBuilder();
            if (menu.ChildrenMenus.Count > 0)
            {
                foreach (var childMenu in menu.ChildrenMenus)
                {
                    children.AppendLine(RenderMenu(childMenu, generateMenuFunc));
                }
            }
            var childrenStr = string.Empty;
            if (children.Length > 0)
            {
                childrenStr = children.ToString();
            }
            return string.Format(menuHtml, childrenStr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="generateMenuFunc">return the menu html string, {0} in the string will be replaced with the html string of children menus</param>
        public static HelperResult RenderMenus(this WebViewPage page, Func<MenusProviderMetadata, string> generateMenuFunc)
        {
            return new HelperResult(tw =>
            {
                var menus = GetAllMenuMetadatas(page.ViewContext, page.Url);
                foreach (var menu in menus)
                {
                    tw.Write(new HtmlString(RenderMenu(menu, generateMenuFunc)));
                }
            });
        }
    }
}