using GalaSoft.MvvmLight;
using Migrator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Model
{
    [Table("GrupaRodzajowaGusZWSIRON")]
    public class GrupaRodzajowaGusZWSIRON : ObservableObject
    {
        [PrimaryKey, Unique]
        public string KodGrRodzZWSIRON { get; set; }
        public string NazwaGrRodzZWSIRON { get; set; }
        [Ignore]
        public string KodNazwaGrRodzZWSIRON
        {
            get
            {
                return string.Format("{0} {1}", KodGrRodzZWSIRON, NazwaGrRodzZWSIRON);
            }
        }
    }
}
