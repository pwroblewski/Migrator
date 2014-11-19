using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.Model
{
    public class SigmatZywnosc : Sigmat
    {
        public string DataWaznosci { get; set; }
        public string NrPartiiProducenta { get; set; }
        public string Opakowanie { get; set; }
        public string DataWydania { get; set; }
        public string WyposazenieIndywidualne { get; set; }
    }
}
