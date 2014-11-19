using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.Model
{
    public class MagmatEwpb : ObservableObject, ICloneable
    {
        public string App { get; set; }
        public int Lp { get; set; }
        public string Jim { get; set; }
        public string Material { get; set; }
        public string Jm { get; set; }
        public int Ilosc { get; set; }
        public string Wartosc { get; set; }
        public string Cena { get; set; }
        public string Kategoria { get; set; }
        public string NrSeryjny { get; set; }
        public string Info { get; set; }

        public string Klasyfikacja { get; set; }

        public string Uzytkownik { get; set; }
        public string UzytkownikZwsiron { get; set; }
        public string NazwaUzytkownika { get; set; }
        public string OsobaUpowazniona { get; set; }

        public string Jednostka { get; set; }
        public string NazwaJednostki { get; set; }

        public string NrMagazynu { get; set; }
        public string NazwaMagazynu { get; set; }

        public string Zaklad { get; set; }
        public string Sklad { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
