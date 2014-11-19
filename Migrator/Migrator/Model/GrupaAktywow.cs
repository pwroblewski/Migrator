using Migrator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Model
{
    [Table("GrupaSkladnikowAktywowTrwalych")]
    public class GrupaAktywow
    {
        [PrimaryKey, Unique]
        public string KontoWartPoczSrtr { get; set; }
        public string GrupaSkladAktywowTrwalych { get; set; }
    }
}
