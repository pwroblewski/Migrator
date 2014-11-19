using Migrator.Model;
using System.Collections.Generic;

namespace Migrator.Services
{
    public interface IFileJednostkaService : IFileService<Jednostka>
    {
        List<MagmatEwpb> GetJedData(List<MagmatEwpb> listMaterialy, string path);
    }
}


