namespace CB.Data.Common.CRUD
{
    public interface ICRUDService<T, in TKey> : IQueryableService<T>, ICreateService<T>, IUpdateService<T>, IDeleteService<T, TKey>
        where T : class
    {
    }
}