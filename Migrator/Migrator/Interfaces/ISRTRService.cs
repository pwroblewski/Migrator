using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Migrator.Model;
using Migrator.Model.State;

namespace Migrator.Services
{
    public interface ISRTRService
    {
        #region Properties
        SrtrState SrtrState { get; set; }
        string ViewName { get; set; }
        List<SrtrToZwsiron> SrtrToZwsiron { get; set; }
        List<KartotekaSRTR> Kartoteka { get; set; }
        List<KartotekaSRTR> KartotekaZlik { get; set; }
        List<Uzytkownik> Users { get; set; }
        List<GrupaRodzajowaGusSRTR> GrGus { get; set; }
        List<WykazIlosciowy> Wykaz { get; set; }
        #endregion

        #region Kartoteka
        string OpenKartotekaFile();
        void LoadKartotekaData(string path);
        #endregion
        #region Uzytkownik
        string OpenUserFile();
        void LoadUserData(string path);
        #endregion
        #region GrGus
        string OpenGrGusFile();
        void LoadGrGusData(string path);
        #endregion
        #region WYkazIlosciowy
        string OpenWykazFile();
        void LoadWykazData(string path);
        #endregion
        #region Jim
        string OpenJimFile();
        void AddJimData(string fileJimPath);
        string SaveJimFile();
        #endregion

        void AddKartotekaFile(List<KartotekaSRTR> listKartoteka);
        void AddSlJed(List<Jednostka> jednostki);
        void AddJednosktaGospodarcza(string JednsotkaGosp);
        void AddJednostkiMiary(List<JednostkaMiary> listJM);
        void AddGrupaAktywow(List<GrupaAktywow> listGrupaAktywow);
        void AddAmortyzacja(List<Amortyzacja> listAmortyzacja);
        void AddUzytkownicy();
        void AddGrupaGus();
        void AddWykaz();
        void GetUsersID();
        void GetGrupaGus();
        string SaveFile();
        string SaveNSTFile();
        void Clean();
    }
}