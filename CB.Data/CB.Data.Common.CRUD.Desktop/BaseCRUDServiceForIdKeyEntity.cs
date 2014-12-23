using System;
using System.Data.Entity;

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
    }
}
