using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Migrator.Model.State
{
    public class SrtrState
    {
        public string ViewName { get; set; }
        public List<KartotekaSRTR> ListKartoteka { get; set; }
        public List<KartotekaSRTR> ListKartotekaZlik { get; set; }
        public List<Uzytkownik> ListUzytkownik { get; set; }
        public List<GrupaRodzajowaGusSRTR> ListGrupaGus { get; set; }
        public List<WykazIlosciowy> ListWykazIlosciowy { get; set; }
        public List<SrtrToZwsiron> ListWynik { get; set; }
    }
}
