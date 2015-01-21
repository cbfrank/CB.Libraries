using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace CB.Data.Common.CRUD
{
    public abstract class BaseCRUDServiceSetInActiveForDelete<T, TKey, TDbContext> : BaseCRUDServiceForIdKeyEntity<T, TKey, TDbContext>
        where T : class, IIdKeyEntity<TKey>, new() 
        where TKey : IEquatable<TKey> 
        where TDbContext : DbContext
    {
        private Expression<Func<T, bool>> _EntityActivePropertyExpression;
        private readonly WeakReference<Action<T, bool>> _SetEntityActiveAction;

        protected BaseCRUDServiceSetInActiveForDelete(TDbContext database) : base(database)
        {
            _SetEntityActiveAction = new WeakReference<Action<T, bool>>(null, false);
        }

        protected abstract IQueryable<T> DoQueryWithoutActiveCheck();

        protected override IQueryable<T> DoQuery()
        {
            return DoQueryWithoutActiveCheck().Where(EntityActivePropertyExpression);
        }

        private Action<T, bool> CompileSetEntityIsActiveAction()
        {
            var paramEntity = Expression.Parameter(typeof(T));
            var paramValue = Expression.Parameter(typeof(bool));
            var prop = (MemberExpression)EntityActivePropertyExpression.Body;
            var getProp = Expression.Property(paramEntity, (PropertyInfo)prop.Member);
            return Expression.Lambda<Action<T, bool>>(Expression.Assign(getProp, paramValue), paramEntity, paramValue).Compile();
        }

        public Expression<Func<T, bool>> EntityActivePropertyExpression
        {
            get { return _EntityActivePropertyExpression; }
            set
            {
                if (_EntityActivePropertyExpression != value)
                {
                    _EntityActivePropertyExpression = value;
                    if (_EntityActivePropertyExpression == null)
                    {
                        _SetEntityActiveAction.SetTarget(null);
                    }
                    else
                    {
                        _SetEntityActiveAction.SetTarget(CompileSetEntityIsActiveAction());
                    }
                }
            }
        }

        public Action<T, bool> SetEntityIsActiveAction
        {
            get
            {
                Action<T, bool> action;
                if (_SetEntityActiveAction.TryGetTarget(out action))
                {
                    return action;
                }
                else
                {
                    _SetEntityActiveAction.SetTarget(CompileSetEntityIsActiveAction());
                    if (_SetEntityActiveAction.TryGetTarget(out action))
                    {
                        return action;
                    }
                }
                return null;
            }
        }

        protected virtual async Task ActiveDeletedEntityAsync(TKey key)
        {
            var deleted = await DoQueryWithoutActiveCheck().Where(GetEntityIdCompareExpression(key)).FirstOrDefaultAsync();
            if (deleted == null)
            {
                throw new DataServiceException(DataServiceException.ENTITY_NOT_FOUND);
            }
            SetEntityIsActiveAction(deleted, true);
            await Database.SaveChangesAsync();
        }

        protected abstract Task<bool> IsEntityDeleted(T entity);

        public override async Task<T> CreateAsync(T entity)
        {
            if (await IsEntityDeleted(entity))
            {
                using (var trans = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await ActiveDeletedEntityAsync(EntityKeyFunction(entity));
                    return await UpdateAsync(entity);
                }
            }
            return await base.CreateAsync(entity);
        }

        protected virtual async Task<bool> ShouldDeletePermanentlyAsync(T entity)
        {
            return await Task.FromResult(false);
        }

        protected virtual async Task<bool> ShouldDeletePermanentlyAsync(IQueryable<T> toBeDeleted)
        {
            return await Task.FromResult(false);
        }

        public override async Task DeleteAsync(TKey key)
        {
            var entity = await GetEntityByKeyAsync(key, true);
            if (await ShouldDeletePermanentlyAsync(entity))
            {
                await base.DeleteAsync(key);
                return;
            }
            SetEntityIsActiveAction(entity, false);
            await UpdateAsync(entity);
        }

        public override async Task DeleteAsync(IQueryable<T> toBeDeleted)
        {
            if (await ShouldDeletePermanentlyAsync(toBeDeleted))
            {
                await base.DeleteAsync(toBeDeleted);
                return;
            }
            throw new NotImplementedException();
        }
    }
}
