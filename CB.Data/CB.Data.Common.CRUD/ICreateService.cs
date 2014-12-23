using System.Threading.Tasks;

namespace CB.Data.Common.CRUD
{
    public interface ICreateService<T> where T : class
    {
        Task<T> CreateAsync(T entity);
    }
}