using Migrator.Helpers;
using Migrator.Model;
using Migrator.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.Services
{
    public interface IMAG_EWPBService
    {
        #region Properties
        MagmatEwpbState MagmatEwpbState { get; set; }
        string ViewName { get; set; }
        List<MagmatEwpb> Materialy { get; set; }
        List<MagmatEwpb> Dictionaries { get; set; }
        List<SigmatKat> Kat { get; set; }
        List<SigmatAmunicja> Amunicja { get; set; }
        List<SigmatPaliwa> Paliwa { get; set; }
        List<SigmatMund> Mund { get; set; }
        List<SigmatZywnosc> Zywnosc { get; set; }
        MagmatEWPB TypWydruku { get; set; }
        #endregion

        #region MagEwpbFile
        string OpenFile(string wydruk);
        void LoadData(string path);
        void AddDomyslnyZakladSklad(string zaklad, string sklad);
        #endregion
        #region Dictionaries
        void FillDictionaryData();
        string OpenDictionaryFile();
        void LoadDictionaryData(string path);
        List<Jednostka> LoadSlJedData(string path);
        #endregion
        #region Jim
        string SaveJimFile();
        string OpenJimFile();
        void AddJimData(string path);
        #endregion

        void AddDictionary();
        void AddJim();
        void SaveFile(string header);
        void Clean();
    }
}
