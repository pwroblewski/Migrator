using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Services
{
    public class DBGrupaAktywowService : IDBGrupaAktywowService
    {
        public async Task<List<GrupaAktywow>> GetAll()
        {
            return await App.Connection.Table<GrupaAktywow>().ToListAsync();
        }

        public async Task Update(GrupaAktywow item)
        {
            await App.Connection.UpdateAsync(item);
        }
    }
}
