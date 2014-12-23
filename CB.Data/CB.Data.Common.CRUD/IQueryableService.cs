using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CB.Data.Common.CRUD
{
    public interface IQueryableService
    {
        IQueryable Query();
    }

    public interface IQueryableService<out T> : IQueryableService where T : class
    {
        new IQueryable<T> Query();
    }
}
