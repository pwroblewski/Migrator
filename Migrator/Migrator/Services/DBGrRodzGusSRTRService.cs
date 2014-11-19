using Migrator.Helpers;
using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Migrator.Services
{
    public class DBGrRodzGusSRTRService : IDBGrRodzGusSRTRService
    {
        public async Task<List<GrupaRodzajowaGusSRTR>> GetAll()
        {
            return await App.Connection.Table<GrupaRodzajowaGusSRTR>().ToListAsync();
        }

        public async Task Update(GrupaRodzajowaGusSRTR item)
        {
            await App.Connection.UpdateAsync(item);
        }

        public async Task<List<GrupaRodzajowaGusSRTR>> SyncGusSRTRData(List<GrupaRodzajowaGusSRTR> listGrGusSRTR)
        {
            List<GrupaRodzajowaGusSRTR> _listGrGusSRTR = new List<GrupaRodzajowaGusSRTR>();

            foreach (GrupaRodzajowaGusSRTR grGus in listGrGusSRTR)
            {
                var q = from f in App.Connection.Table<GrupaRodzajowaGusSRTR>()
                        where f.KodGrRodzSRTR == grGus.KodGrRodzSRTR
                        select f;
                var grupa = q.FirstOrDefaultAsync();

                if (grupa.Result != null)
                {
                    _listGrGusSRTR.Add(await q.FirstAsync());
                }
                else
                {
                    _listGrGusSRTR.Add(grGus);
                    await App.Connection.InsertAsync(grGus);
                }
            }

            return _listGrGusSRTR;
        }


        public async Task SyncFileGusSRTRData(List<GrupaRodzajowaGusSRTR> listGrGusSRTR)
        {
            foreach (GrupaRodzajowaGusSRTR grGus in listGrGusSRTR)
            {
                var q = from f in App.Connection.Table<GrupaRodzajowaGusSRTR>()
                        where f.KodGrRodzSRTR == grGus.KodGrRodzSRTR
                        select f;
                var grupa = await q.FirstOrDefaultAsync();

                try
                {
                    if (grupa != null)
                    {
                        grupa = ExtensionMethods.MergeWith<GrupaRodzajowaGusSRTR>(grupa, grGus);
                        await App.Connection.UpdateAsync(grupa);
                    }
                    else
                    {
                        await App.Connection.InsertAsync(grGus);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
