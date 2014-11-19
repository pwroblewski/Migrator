using Migrator.Model;
using System.Collections.Generic;

namespace Migrator.Services
{
    public interface IFileUserService : IFileService<Uzytkownik>
    {
        List<MagmatEwpb> GetUserData(List<MagmatEwpb> listMaterialy, string path);
        List<Uzytkownik> SyncData(List<Uzytkownik> tempList, List<Uzytkownik> listUzytkownicy);
    }
}
