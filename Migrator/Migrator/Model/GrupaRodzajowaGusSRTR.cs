using GalaSoft.MvvmLight;
using Migrator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Model
{
    [Table("GrupaRodzajowaGusSRTR")]
    public class GrupaRodzajowaGusSRTR : ObservableObject
    {
        [PrimaryKey, Unique]
        public string KodGrRodzSRTR { get; set; }
        public string NazwaGrRodzSRTR { get; set; }
        public string KodGrRodzZWSIRON { get; set; }
    }
}
