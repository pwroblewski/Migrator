using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Migrator.Model;

namespace Migrator.Services
{
    public interface ISRTRService
    {
        #region Properties
        List<SrtrToZwsiron> SrtrToZwsiron
        {
            get;
            set;
        }
        List<KartotekaSRTR> Kartoteka
        {
            get;
            set;
        }
        List<KartotekaSRTR> KartotekaZlik
        {
            get;
            set;
        }
        List<Uzytkownik> Users
        {
            get;
            set;
        }
        List<GrupaRodzajowaGusSRTR> GrGus
        {
            get;
            set;
        }
        List<WykazIlosciowy> WykazIlosciowy
        {
            get;
            set;
        }
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

        void AddKartotekaFile(List<KartotekaSRTR> listKartoteka);
        void AddJednosktaGospodarcza(string JednsotkaGosp);
        void AddJednostkiMiary(List<JednostkaMiary> listJM);
        void AddGrupaAktywow(List<GrupaAktywow> listGrupaAktywow);
        void AddAmortyzacja(List<Amortyzacja> listAmortyzacja);
        void AddUzytkownicy(List<Uzytkownik> listUzytkownicy);
        void AddGrupaGus(List<GrupaRodzajowaGusSRTR> listGrupaGus);
        void AddWykaz(List<WykazIlosciowy> listWykaz);
        List<Uzytkownik> GetUsersID();
        List<GrupaRodzajowaGusSRTR> GetGrupaGus();
        string SaveFile();
        void Clean();
    }
}