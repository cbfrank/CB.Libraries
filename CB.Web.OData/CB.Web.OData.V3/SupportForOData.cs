using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData.Routing;
using System.Web.Http.OData.Routing.Conventions;
using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Library;

namespace CB.Web.OData
{
    public class CountODataRoutingConvention : EntitySetRoutingConvention
    {
        public override string SelectAction(ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
        {
            if (controllerContext.Request.Method == HttpMethod.Get && odataPath.PathTemplate == "~/entityset/$count")
            {
                if (actionMap.Contains("GetCount"))
                {
                    return "GetCount";
                }
            }
            return null;
        }
    }

    public class CountODataPathHandler : DefaultODataPathHandler
    {
        protected override ODataPathSegment ParseAtEntityCollection(IEdmModel model, ODataPathSegment previous, IEdmType previousEdmType, string segment)
        {
            if (segment == "$count")
            {
                return new CountPathSegment();
            }
            return base.ParseAtEntityCollection(model, previous, previousEdmType, segment);
        }
    }

    public class CountPathSegment : ODataPathSegment
    {
        public override string SegmentKind
        {
            get
            {
                return "$count";
            }
        }

        public override IEdmType GetEdmType(IEdmType previousEdmType)
        {
            return EdmCoreModel.Instance.FindDeclaredType("Edm.Int32");
        }

        public override IEdmEntitySet GetEntitySet(IEdmEntitySet previousEntitySet)
        {
            return previousEntitySet;
        }

        public override string ToString()
        {
            return "$count";
        }
    }
}
