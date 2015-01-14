using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Model
{
    public class SrtrToZwsiron : ObservableObject
    {
        public string GrupaAktywow { get; set; }
        public string JednostkaGospodarcza { get; set; }
        public string IndeksMaterialowy { get; set; }
        public string Nazwa { get; set; }
        public string JednostkaMiary { get; set; }
        public string Jeden { get; set; }
        public string KatSprzetu { get; set; }
        public string NrSeryjny { get; set; }
        public string NrInwentarzowy { get; set; }
        public string WartoscPoczatkowa { get; set; }
        public string Umorzenie { get; set; }
        public string Zero { get; set; }
        public string StawkaAmor { get; set; }
        public string GrupaGus { get; set; }
        public string DataNabycia { get; set; }
        public string DataNabycia2 { get; set; }
        public string DataNabycia3 { get; set; }
        public string Mpk { get; set; }
        public string AmorCzasLata { get; set; }
        public string AmorCzasMisiace { get; set; }
        public string Kwo { get; set; }
        public string IdUzytZwsiron { get; set; }
        public string Zaklad { get; set; }
       
        //--------------------------------------------------------
        public string IdUzytSrtr { get; set; }
        public string GrupaAktywowSRTR { get; set; }
        public string JednsotkaMiarySRTR { get; set; }
        public string WartoscPoczatkowaSRTR { get; set; }
        public string WspolczynnikAmortyzacjiSRTR { get; set; }
        public string GrupaGusSRTR { get; set; }
    }
}
