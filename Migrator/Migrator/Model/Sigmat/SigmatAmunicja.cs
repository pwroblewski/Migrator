using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.Model
{
    public class SigmatAmunicja : Sigmat
    {
        public string NrPartii { get; set; }
        public string DataProdukcji { get; set; }
        public string DataZablokowania { get; set; }
        public string Wyroznik { get; set; }
        public string DataGwarancji { get; set; }
        public string ZnacznikBlokowania { get; set; }
        public string NrSeryjny { get; set; }
        public string DataGwarancjiJBR { get; set; }
        public string Zapalnik { get; set; }
        public string ZapalnikDataGwarancji { get; set; }
        public string ZapalnikDataGwarancjiJBR { get; set; }
        public string Zaplonnik { get; set; }
        public string ZaplonnikDataGwarancji { get; set; }
        public string ZaplonnikDataGwarancjiJBR { get; set; }
        public string Ladunek { get; set; }
        public string LadunekDataGwarancji { get; set; }
        public string LadunekDataGwarancjiJBR { get; set; }
        public string Pocisk { get; set; }
        public string PociskDataGwarancji { get; set; }
        public string PociskDataGwarancjiJBR { get; set; }
        public string Zrodlo { get; set; }
        public string ZrodloDataGwarancji { get; set; }
        public string ZrodloDataGwarancjiJBR { get; set; }
        public string Smugacz { get; set; }
        public string SmugaczDataGwarancji { get; set; }
        public string SmugaczDataGwarancjiJBR { get; set; }
        public string DataWydania { get; set; }
        public string WyposazenieIndywidualne { get; set; }
    }
}
