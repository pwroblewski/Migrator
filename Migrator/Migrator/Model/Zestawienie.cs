using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.Model
{
    public class Zestawienie : ObservableObject
    {
        public string Jim { get; set; }
        public string Material { get; set; }
        public string MaterialProf { get; set; }
        public string MaterialKonto { get; set; }
        public string Zaklad { get; set; }
        public string Sklad { get; set; }
        public string Uzytkownik { get; set; }
        public string Dzial { get; set; }
        public string SymbolKat { get; set; }
        public string Wskaznik { get; set; }
        public string ZapasBezp { get; set; }
        public string TypWyceny { get; set; }
        public string Rodzaj { get; set; }
        public string StarCena { get; set; }
        public string JednCena { get; set; }
        public string CenaSrednia { get; set; }
        public string CenaStand { get; set; }
        public string ZakladDost { get; set; }

        public Zestawienie(string Jim, string Zaklad, string Sklad, string Uzytkownik)
        {
            this.Jim = Jim;
            Material = string.Empty;
            MaterialProf = string.Empty;
            MaterialKonto = string.Empty;
            this.Zaklad = Zaklad;
            this.Sklad = Sklad;
            this.Uzytkownik = Uzytkownik;
            Dzial = string.Empty;
            SymbolKat = string.Empty;
            Wskaznik = "X";
            ZapasBezp = "0";
            TypWyceny = "X";
            Rodzaj = string.Empty;
            StarCena = "V";
            JednCena = "1";
            CenaSrednia = "0";
            CenaStand = "0";
            ZakladDost = Zaklad;
        }
    }
}
