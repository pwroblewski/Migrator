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
        MagmatEWPB TypWydruku { get; set; }
        #endregion

        #region MagEwpbFile
        string OpenFile(string wydruk);
        void LoadData(string path);
        #endregion

        #region Dictionaries
        void FillDictionaryData();
        string OpenDictionaryFile();
        void LoadDictionaryData(string path);
        #endregion

        string SaveFile();


        List<SigmatKat> GetKat();
        List<SigmatAmunicja> GetAmunicja();
        List<SigmatMund> GetMund();
        List<SigmatPaliwa> GetPaliwa();
        List<SigmatZywnosc> GetZywnosc();
        List<MagmatEwpb> GetMaterialy();

        void SetKat(List<SigmatKat> listKat);
        void SetAmunicja(List<SigmatAmunicja> listAmunicja);
        void SetMund(List<SigmatMund> listMund);
        void SetPaliwa(List<SigmatPaliwa> listPaliwa);
        void SetZywnosc(List<SigmatZywnosc> listZywnosc);


        void AddMaterial(List<MagmatEwpb> listMaterialy);
        void AddSlownik(List<MagmatEwpb> listMaterialy, MagmatEWPB typWydruku);
        void AddJim(List<MagmatEwpb> listMaterialy, MagmatEWPB typWydruku);
        void Clean();
    }
}
