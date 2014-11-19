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
        //void LoadKartoteka();
        string SaveFile();
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
        List<SrtrToZwsiron> GetSrtrToZwsiron();
        void SetSrtrToZwsiron(List<SrtrToZwsiron> listSrtrToZwsiron);
        void Clean();
    }
}
