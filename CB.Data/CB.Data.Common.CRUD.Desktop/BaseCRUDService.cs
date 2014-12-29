using System;
using System.Data.Entity;
using System.IdentityModel.Services;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using EntityFramework.Extensions;

namespace CB.Data.Common.CRUD
{
    public abstract class BaseCRUDService<T, TKey, TDbContext> : ICRUDService<T, TKey>, IDisposable
        where TDbContext : DbContext
        where T : class, new()
        where TKey : IEquatable<TKey>
    {
        private DbSet<T> _CorrespondingDbSet;
        private Expression<Func<T, TKey>> _EntityKeyExpression;
        private readonly WeakReference<Func<T, TKey>> _EntityKeyFunction;

        protected BaseCRUDService(TDbContext database)
        {
            _EntityKeyFunction = new WeakReference<Func<T, TKey>>(null, false);
            EntityKeyExpression = null;
            Database = database;
        }

        /// <summary>
        /// expression that return the Id property of entity
        /// </summary>
        public Expression<Func<T, TKey>> EntityKeyExpression
        {
            get { return _EntityKeyExpression; }
            set
            {
                if (_EntityKeyExpression != value)
                {
                    _EntityKeyExpression = value;
                    if (_EntityKeyExpression == null)
                    {
                        _EntityKeyFunction.SetTarget(null);
                    }
                    else
                    {
                        _EntityKeyFunction.SetTarget(_EntityKeyExpression.Compile());
                    }
                }
            }
        }

        /// <summary>
        /// Get a function that can return the Id value of entity
        /// </summary>
        public Func<T, TKey> EntityKeyFunction
        {
            get
            {
                Func<T, TKey> func;
                if (_EntityKeyFunction.TryGetTarget(out func))
                {
                    return func;
                }
                return null;
            }
        }

        public Expression<Func<T, bool>> GetEntityIdCompareExpression(TKey value)
        {
            var param = Expression.Parameter(typeof (T));
            var prop = ((MemberExpression) _EntityKeyExpression.Body).Member;
            var gerProp = Expression.Property(param, (PropertyInfo) prop);
            return Expression.Lambda<Func<T, bool>>(Expression.Equal(gerProp, Expression.Constant(value)), param);
        }

        protected TDbContext Database { get; private set; }

        public abstract string ResourceName { get; }
        protected abstract bool DisposeDatabase { get; }

        protected DbSet<T> CorrespondingDbSet
        {
            get { return _CorrespondingDbSet ?? (_CorrespondingDbSet = Database.Set<T>()); }
        }

        /// <summary>
        /// should throw exception when no access
        /// </summary>
        /// <param name="action"></param>
        /// <param name="entity"></param>
        protected virtual void CheckAccess(CRUDAction action, T entity)
        {
            ClaimsPrincipalPermission.CheckAccess(ResourceName, DefaultSecurityOperations.FromCRUDActions(action));
        }

        /// <summary>
        /// should throw exception when no access
        /// </summary>
        /// <param name="action"></param>
        /// <param name="entities"></param>
        protected virtual void CheckAccess(CRUDAction action, IQueryable<T> entities)
        {
            ClaimsPrincipalPermission.CheckAccess(ResourceName, DefaultSecurityOperations.FromCRUDActions(action));
        }

        /// <summary>
        /// find the matahced entity existing in database, generally, just use key to find, but for the entity that just disabled, maybe use other method to match
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        protected virtual async Task<T> FindExistingEntityAsync(T entity, bool tracking = true)
        {
            return await GetEntityByKeyAsync(EntityKeyFunction(entity), tracking);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="action"></param>
        /// <returns>result is the error codes array</returns>
        protected virtual async Task<ValidationResult<T>> ValidationAsync(T entity, CRUDAction action)
        {
            var result = new ValidationResult<T> {ExistingEntity = null};
            if (action == CRUDAction.Update || action == CRUDAction.Delete || action == CRUDAction.Create)
            {
                result.ExistingEntity = await FindExistingEntityAsync(entity, action == CRUDAction.Update);
                if (action == CRUDAction.Create)
                {
                    if (result.ExistingEntity != null)
                    {
                        throw new DataServiceException(DataServiceException.DUPLICATED_ENTITY);
                    }
                }
                else //update or delete
                {
                    if (result.ExistingEntity == null)
                    {
                        throw new DataServiceException(DataServiceException.ENTITY_NOT_FOUND);
                    }
                }
            }
            CheckAccess(action, entity);
            return result;
        }

        protected virtual async Task<T> PreCRUDAction(T entity, T existingEntity, CRUDAction action)
        {
            return await Task.FromResult(entity);
        }

        protected virtual async Task<IQueryable<T>> PreCRUDAction(IQueryable<T> entities, CRUDAction action)
        {
            if (action == CRUDAction.Create || action == CRUDAction.Update)
            {
                throw new NotSupportedException();
            }
            return await Task.FromResult(entities);
        }

        protected virtual async Task<T> PostCRUDAction(T entity, CRUDAction action)
        {
            return await Task.FromResult(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="action"></param>
        /// <returns>result is the error codes array</returns>
        protected virtual Task<ValidationResult<T>> ValidationAsync(IQueryable<T> entities, CRUDAction action)
        {
            if (action == CRUDAction.Create || action == CRUDAction.Update)
            {
                throw new NotSupportedException();
            }
            CheckAccess(action, entities);
            return Task.FromResult(new ValidationResult<T>());
        }

        public async Task<T> GetEntityByKeyAsync(TKey key, bool tracking)
        {
            var q = Query().Where(GetEntityIdCompareExpression(key));
            if (!tracking)
            {
                q = q.AsNoTracking();
            }
            return await q.FirstOrDefaultAsync();
        }

        protected virtual void DisposeInternalService()
        {
            _CorrespondingDbSet = null;
        }

        protected virtual async Task SaveChangesAsync()
        {
            await Database.SaveChangesAsync();
        }

        protected abstract IQueryable<T> DoQuery();

        #region Overrides of BaseService

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DisposeDatabase && Database != null)
                {
                    Database.Dispose();
                    Database = null;
                }
            }
        }

        #endregion

        #region Implementation of IQueryableService<out T>

        public virtual IQueryable<T> Query()
        {
            var result = DoQuery();
            CheckAccess(CRUDAction.Query, result);
            return result;
        }

        #endregion

        #region Implementation of ICreateService<T>

        public virtual async Task<T> CreateAsync(T entity)
        {
            var validationResult = await ValidationAsync(entity, CRUDAction.Create);
            validationResult.ThrowException();
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                entity = await PreCRUDAction(entity, validationResult.ExistingEntity, CRUDAction.Create);
                entity = CorrespondingDbSet.Add(entity);
                await SaveChangesAsync();
                entity = await PostCRUDAction(entity, CRUDAction.Create);
                trans.Complete();
            }
            return entity;
        }

        #endregion

        #region Implementation of IUpdateService<T>

        public virtual async Task<T> UpdateAsync(T entity)
        {
            var validationResult = await ValidationAsync(entity, CRUDAction.Update);
            validationResult.ThrowException();
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                entity = await PreCRUDAction(entity, validationResult.ExistingEntity, CRUDAction.Update);
                //entity = Database.Set<T>().Attach(entity);
                //Database.Entry(entity).State = EntityState.Modified;
                //copy won't change navigate properties, so it is save
                Database.Entry(validationResult.ExistingEntity).CurrentValues.SetValues(entity);
                //validationResult.ExistingEntity.Copy(entity);
                if (!Database.Configuration.AutoDetectChangesEnabled)
                {
                    Database.Entry(validationResult.ExistingEntity).State = EntityState.Modified;
                }
                await SaveChangesAsync();
                validationResult.ExistingEntity = await PostCRUDAction(validationResult.ExistingEntity, CRUDAction.Update);
                trans.Complete();
            }
            return validationResult.ExistingEntity;
        }

        #endregion

        #region Implementation of IDeleteService<in TKey>

        public virtual async Task DeleteAsync(TKey key)
        {
            var entity = await GetEntityByKeyAsync(key, true);
            if (entity == null)
            {
                return;
            }
            var validationResult = await ValidationAsync(entity, CRUDAction.Delete);
            validationResult.ThrowException();
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                entity = await PreCRUDAction(entity, validationResult.ExistingEntity, CRUDAction.Delete);
                Database.Set<T>().Remove(entity);
                await SaveChangesAsync();
                await PostCRUDAction(entity, CRUDAction.Delete);
                trans.Complete();
            }
        }

        public virtual async Task DeleteAsync(IQueryable<T> toBeDeleted)
        {
            var validationResult = await ValidationAsync(toBeDeleted, CRUDAction.Delete);
            validationResult.ThrowException();
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                toBeDeleted = await PreCRUDAction(toBeDeleted, CRUDAction.Delete);
                await toBeDeleted.DeleteAsync();
                trans.Complete();
            }
        }

        #endregion

        #region Implementation of IQueryableService

        IQueryable IQueryableService.Query()
        {
            return Query();
        }

        #endregion

        #region Implementation of IDeleteService

        Task IDeleteService.DeleteAsync(IQueryable toBeDeleted)
        {
            return DeleteAsync((IQueryable<T>) toBeDeleted);
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
        }

        ~BaseCRUDService()
        {
            Dispose(false);
        }
    }
}