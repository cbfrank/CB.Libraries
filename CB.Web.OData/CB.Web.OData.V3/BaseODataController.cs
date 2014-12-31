using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using CB.Data.Common.CRUD;
using CB.Ioc;

namespace CB.Web.OData
{
    public abstract class BaseODataController<T, TKey> : ODataController
        where T : class, IIdKeyEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        protected BaseODataController()
        {
            DisposeService = true;
        }

        protected bool DisposeService { get; private set; }

        [Dependency]
        public ICRUDService<T, TKey> Service { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Service != null && DisposeService && (Service is IDisposable))
                {
                    ((IDisposable)Service).Dispose();
                }
            }
            base.Dispose(disposing);
        }

        protected virtual T PreProcess(T entity, CRUDAction action)
        {
            return entity;
        }

        public virtual IQueryable<T> Get()
        {
            return Service.Query();
        }

        public virtual SingleResult<T> Get([FromODataUri] TKey key)
        {
            var result = Service.Query().Where(p => p.Id.Equals(key));
            return SingleResult.Create(result);
        }

        public virtual async Task<IHttpActionResult> Post(T entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            entity = await Service.CreateAsync(PreProcess(entity, CRUDAction.Create));
            return Created(entity);
        }

        public virtual async Task<IHttpActionResult> Put([FromODataUri] TKey key, T entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            entity = await Service.UpdateAsync(PreProcess(entity, CRUDAction.Update));
            return Updated(entity);
        }

        public virtual async Task<IHttpActionResult> Delete([FromODataUri] TKey key)
        {
            await Service.DeleteAsync(key);

            return StatusCode(HttpStatusCode.NoContent);
        }

        public virtual HttpResponseMessage GetCount(ODataQueryOptions<T> queryOptions)
        {
            var query = queryOptions.ApplyTo(Get());
            var queryResults = query as IQueryable<object>;
            var count = queryResults.Count();
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(count.ToString(CultureInfo.InvariantCulture), Encoding.UTF8, "text/plain")
            };
            return response;
        }
    }
}
