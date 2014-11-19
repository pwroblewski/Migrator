using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.Services
{
    public interface IFileZestawienieService
    {
        string[] OpenFileDialog();
        List<Zestawienie> GetAll(string[] path);
        List<ZestawienieKlas> AddJimData(string path);
        List<Zestawienie> GetZestawienie();
        void SetZestawienie(List<Zestawienie> listZestawienie);
        List<ZestawienieKlas> GetZestawienieKlas();
        void SetZestawienieKlas(List<ZestawienieKlas> listZestawienieKlas);
        void ZapiszZestawienieKlas(List<ZestawienieKlas> listZestawienieKlas);
        void ZapiszPliki();
        void Clean();
    }
}
