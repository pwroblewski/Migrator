using GalaSoft.MvvmLight;
using Migrator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Model
{
    [Table ("Amortyzacja")]
    public class Amortyzacja : ObservableObject
    {
        [PrimaryKey, Unique]
        public string IdAmor { get; set; }
        public float KodStawkiAmorSrtr { get; set; }
        public string KodStawkiAmorZwsiron { get; set; }
        public float StawkaAmor { get; set; }
        [Ignore]
        public int CzasLata { get; set; }
        [Ignore]
        public int CzasMiesiace { get; set; }
    }
}
