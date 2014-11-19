using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Services
{
    public interface IKartotekaSRTRService
    {
        List<KartotekaSRTR> GetAll();
        bool HasData();
        List<KartotekaSRTR> GetStoredData();
        bool HasStoreData();
        string GetPath();
    }
}
