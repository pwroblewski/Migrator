using GalaSoft.MvvmLight;
using Migrator.Helpers;
using System.Collections.Generic;

namespace Migrator.Model.State
{
    public class MagmatEwpbState : ObservableObject
    {
        public string ViewName { get; set; }
        public string Wydruk { get; set; }
        public MagmatEWPB TypWydruku { get; set; }

        public List<MagmatEwpb> ListData { get; set; }
        public List<MagmatEwpb> ListDictionary { get; set; }
        public List<MagmatEwpb> MagmatEwpbService { get; set; }
        public List<MagmatEwpb> ListSigmat { get; set; }
        public List<SigmatKat> ListKat { get; set; }
        public List<SigmatAmunicja> ListAmunicja { get; set; }
            public List<SigmatPaliwa> ListPaliwa { get; set; }
        public List<SigmatMund> ListMund { get; set; }
        public List<SigmatZywnosc> ListZywnosc { get; set; }
        public List<MagmatEwpb> ListJim { get; set; }
    }
}
