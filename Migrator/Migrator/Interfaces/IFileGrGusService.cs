using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Migrator.Model;

namespace Migrator.Services
{
    public interface IFileGrGusService : IFileService<GrupaRodzajowaGusSRTR>
    {
        List<GrupaRodzajowaGusSRTR> SyncData(List<GrupaRodzajowaGusSRTR> fileData, List<GrupaRodzajowaGusSRTR> listGrupaGus);
    }
}
