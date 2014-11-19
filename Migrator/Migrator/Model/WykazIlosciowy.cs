using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Model
{
    public class WykazIlosciowy : ObservableObject
    {
        public string NrInwentarzowy { get; set; }
        public string WartoscPoczatkowa { get; set; }
        public string IndeksMaterialowy { get; set; }
        public string Umorzenie { get; set; }
        public string Zaklad { get; set; }
        public string NazwaMaterialu { get; set; }
        public string JednostkaMiary { get; set; }
    }
}
