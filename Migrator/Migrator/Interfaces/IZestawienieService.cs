using Migrator.Model;
using Migrator.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.Services
{
    public interface IZestawienieService
    {
        #region Properties
        ZestawienieState ZestawienieState { get; set; }
        string ViewName { get; set; }
        List<Zestawienie> Zestawienia { get; set; }
        List<ZestawienieKlas> ZestawieniaKlas { get; set; }
        #endregion

        #region Zestawienie
        string[] OpenFileDialog();
        void LoadData(string[] paths);
        #endregion

        #region Jim
        string SaveJimFile();
        string OpenJimFile();
        void AddJimData(string path);
        #endregion


        void ZapiszPliki();
        void Clean();
    }
}
