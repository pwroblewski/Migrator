using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Helpers
{
    public class GrupaGusComparer : IEqualityComparer<GrupaRodzajowaGusSRTR>
    {
        public bool Equals(GrupaRodzajowaGusSRTR x, GrupaRodzajowaGusSRTR y)
        {
            return x.KodGrRodzSRTR == y.KodGrRodzSRTR;
        }

        public int GetHashCode(GrupaRodzajowaGusSRTR obj)
        {
            return (obj.KodGrRodzSRTR).GetHashCode();
        }
    }
}
