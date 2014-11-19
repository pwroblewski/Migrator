using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.Model
{
    public class SigmatKat : Sigmat
    {
        public string NrSeryjny { get; set; }
        public string DataNabycia { get; set; }
        public string WartoscPoczatkowa { get; set; }
        public string WartoscUmorzenia { get; set; }
        public string StawkaAmortyzacji { get; set; }
        public string KlasSrodkowTrwalych { get; set; }
        public string DataProdukcji { get; set; }
        public string DataGwarancji { get; set; }
        public string DataWydania { get; set; }
        public string WyposazenieIndywidualne { get; set; }
        public string Pododdzial { get; set; }
        public string KodStan { get; set; }
    }
}
