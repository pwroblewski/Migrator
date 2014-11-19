using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.Model
{
    public class SigmatPaliwa : Sigmat
    {
        public string DataProdukcji { get; set; }
        public string TypOpakowania { get; set; }
        public string Wycena { get; set; }
        public string Orzeczenie { get; set; }
        public string DataWydania { get; set; }
        public string WyposazenieIndywidualne { get; set; }
    }
}
