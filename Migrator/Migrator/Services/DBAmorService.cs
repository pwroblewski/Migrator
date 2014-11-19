using Migrator.Helpers;
using Migrator.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Migrator.Services
{
    public class DBAmorService : IDBAmorService
    {
        public async Task<List<Amortyzacja>> GetAll()
        {
            List<Amortyzacja> amorList = await App.Connection.Table<Amortyzacja>().ToListAsync();

            foreach (var amor in amorList)
            {
                double temp;

                if (amor.StawkaAmor != 0.0)
                {
                    amor.CzasLata = Convert.ToInt32(Math.Floor(Convert.ToDecimal(1 / amor.StawkaAmor)));
                    temp = (1 / amor.StawkaAmor) - amor.CzasLata;
                }
                else
                {
                    amor.CzasLata = 0;
                    temp = 0;
                }

                amor.CzasMiesiace = Convert.ToInt32(Math.Ceiling(temp * 12));
            }

            return amorList;
        }
        
        public async Task Update(Amortyzacja item)
        {
            await App.Connection.UpdateAsync(item);
        }
    }
}
