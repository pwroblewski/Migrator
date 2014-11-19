using Migrator.Helpers;
using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.Services
{
    public interface IFileMagmatEwpbService : IFileService<MagmatEwpb>
    {
        List<MagmatEwpb> GetDictData(MagmatEWPB module);
        void AddApp(MagmatEWPB typ);

        void AddZakladSklad(string zaklad, string sklad);
        List<MagmatEwpb> GetData();
        void SetData(List<MagmatEwpb> listMagmatEwpb);
    }
}
