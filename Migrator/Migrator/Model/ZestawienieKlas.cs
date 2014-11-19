using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.Model
{
    public class ZestawienieKlas : ObservableObject
    {
        public string Jim { get; set; }
        public string KlasaZaop { get; set; }
        public string Nazwa { get; set; }
        public string Jm { get; set; }
        public string StaryNrInd { get; set; }
        public string GrupaKlasaKwo { get; set; }
        public string Gestor { get; set; }
        public string KlasyfikatorHier { get; set; }
        public string WagaBrutto { get; set; }
        public string JednWagi { get; set; }
        public string WagaNetto { get; set; }
        public string Objetosc { get; set; }
        public string JednObj { get; set; }
        public string Wymiary { get; set; }
        public string KodCpv { get; set; }
        public string WyroznikCpv { get; set; }
        public string Norma { get; set; }
        public string WyroznikProdNiebezp { get; set; }
        public string KlasyfikacjaPartii { get; set; }
    }
}