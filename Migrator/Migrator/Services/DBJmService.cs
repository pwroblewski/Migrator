using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Services
{
    public class DBJmService : IDBJmService
    {
        public async Task<List<JednostkaMiary>> GetAll()
        {
            return await App.Connection.Table<JednostkaMiary>().ToListAsync();
        }

        public async Task Update(Model.JednostkaMiary item)
        {
            await App.Connection.UpdateAsync(item);
        }
    }
}
