using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Services
{
    public interface IDBGrRodzGusSRTRService : IDBService<GrupaRodzajowaGusSRTR>
    {
        Task<List<GrupaRodzajowaGusSRTR>> SyncGusSRTRData(List<GrupaRodzajowaGusSRTR> listGrGusSRTR);
        Task SyncFileGusSRTRData(List<GrupaRodzajowaGusSRTR> listGrGusSRTR);
    }
}
