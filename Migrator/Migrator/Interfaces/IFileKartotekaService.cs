using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Services
{
    public interface IFileKartotekaService : IFileService<KartotekaSRTR>
    {
        List<KartotekaSRTR> GetMagmatData();
        List<KartotekaSRTR> GetKartoteka();
        void SetKartoteka(List<KartotekaSRTR> listKartoteka);
    }
}
