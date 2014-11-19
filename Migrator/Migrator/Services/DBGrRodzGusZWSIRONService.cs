using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Migrator.Services
{
    public class DBGrRodzGusZWSIRONService : IDBGrRodzGusZWSIRONService
    {
        public async Task<List<GrupaRodzajowaGusZWSIRON>> GetAll()
        {
            return await App.Connection.Table<GrupaRodzajowaGusZWSIRON>().ToListAsync();
        }

        public async Task Update(GrupaRodzajowaGusZWSIRON item)
        {
            await App.Connection.UpdateAsync(item);
        }
    }
}
