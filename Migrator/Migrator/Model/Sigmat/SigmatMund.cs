using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.Model
{
    public class SigmatMund : Sigmat
    {
        public string Rozmiar { get; set; }
        public string RokProdukcji { get; set; }
        public string RokGwarancji { get; set; }
        public string DataWydania { get; set; }
        public string WyposazenieInduwidualne { get; set; }
        public string Pododdzial { get; set; }
        public string OkresUzywalnosci { get; set; }
        public string TypPozycjiPodzestawu { get; set; }
    }
}
