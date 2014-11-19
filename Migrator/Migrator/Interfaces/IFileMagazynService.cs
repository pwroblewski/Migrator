using Migrator.Model;
using System.Collections.Generic;

namespace Migrator.Services
{
    public interface IFileMagazynService : IFileService<Magazyn>
    {
        List<MagmatEwpb> GetMagData(List<MagmatEwpb> listMaterialy, string path);
    }
}
