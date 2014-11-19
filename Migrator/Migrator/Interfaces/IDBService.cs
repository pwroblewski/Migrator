using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Services
{
    public interface IDBService<T>
    {
        Task<List<T>> GetAll();
        Task Update(T item);
    }
}
