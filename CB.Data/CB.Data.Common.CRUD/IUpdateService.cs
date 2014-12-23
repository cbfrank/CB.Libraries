using System.Threading.Tasks;

namespace CB.Data.Common.CRUD
{
    public interface IUpdateService<T> where T : class
    {
        Task<T> UpdateAsync(T entity);
    }
}