using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Services
{
    public interface IFileJimService : IFileService<WykazIlosciowy>
    {
        string SaveFileDialog(List<WykazIlosciowy> listWykazIlosciowy);
        string SaveFileDialog(List<MagmatEwpb> listMaterialy);
        string SaveFileDialog(List<ZestawienieKlas> listZestawienieKlas);
        List<WykazIlosciowy> AddJimData(string fileJimPath, List<WykazIlosciowy> listWykazIlosciowy);
        List<MagmatEwpb> AddJimData(string fileJimPath, List<MagmatEwpb> listMaterialy);
    }
}
