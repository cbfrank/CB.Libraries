using System.Linq;
using System.Threading.Tasks;

namespace CB.Data.Common.CRUD
{
    public interface IDeleteService
    {
        Task DeleteAsync(IQueryable toBeDeleted);
    }

    public interface IDeleteService<in T, in TKey> : IDeleteService where T : class
    {
        Task DeleteAsync(TKey key);
        Task DeleteAsync(IQueryable<T> toBeDeleted);
    }
}