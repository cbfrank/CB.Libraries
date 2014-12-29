using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CB.Data.Common.CRUD
{
    public abstract class BaseCRUDServiceForIdKeyEntity<T, TKey, TDbContext> : BaseCRUDService<T, TKey, TDbContext>
        where TKey : IEquatable<TKey>
        where T : class, IIdKeyEntity<TKey>, new() 
        where TDbContext : DbContext
    {
        protected BaseCRUDServiceForIdKeyEntity(TDbContext database) : base(database)
        {
            EntityKeyExpression = entity => entity.Id;
        }

        protected override async Task<bool> CheckQueryableResultValidAsync(IQueryable<T> entitiesToBeChecked)
        {
            var test = from e in entitiesToBeChecked
                join eCanBeQuery in DoQuery() on e.Id equals eCanBeQuery.Id
                where eCanBeQuery == null
                select e;
            return ! await test.AnyAsync();
        }
    }
}
