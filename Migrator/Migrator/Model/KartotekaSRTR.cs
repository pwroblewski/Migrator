using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Model
{
    public class KartotekaSRTR : ObservableObject
    {
        public KartotekaSRTR()
        { }

        public string Nr_kolejny { get; set; }
        public string Indeks_m { get; set; } 
        public string Nazwa_sr { get; set; } 
        public string Gr_gus { get; set; } 
        public string Kod_jed { get; set; } 
        public string Kod_uzyt { get; set; }    // 5
        public string Konto_amo { get; set; } 
        public string Konto_umo { get; set; } 
        public string Konto_wpc { get; set; } 
        public string Rodz_amor { get; set; } 
        public string Kat_sprz { get; set; }    // 10
        public string Data_nab { get; set; } 
        public string Dowod_nab { get; set; } 
        public string Konto_nab { get; set; } 
        public string Uwagi_nab { get; set; } 
        public string Data_prz { get; set; } 
        public string War_pocz { get; set; } 
        public string Bo_wart_in { get; set; }  // 17
        public string Bo_wart_um { get; set; } 
        public string Data_lik { get; set; } 
        public string Wart_inw_2 { get; set; } 
        public string Wsp_am_1 { get; set; } 
        public string Wsp_am_2 { get; set; } 
        public string Bo_am_2 { get; set; } 
        public string Wsk_bl { get; set; } 
        public string Rodz_lik { get; set; }        // 25
        public string Blok_am { get; set; } 
        public string Am_sezon { get; set; } 
        public string Nr_seryjny { get; set; } 
        public string Data_nab2 { get; set; } 
        public string Nr_dok { get; set; } 
        public string Data_dok { get; set; } 
        public string Nr_jw { get; set; } 
        public string Nr_branz { get; set; }            // 33
        public string Nr_rejest { get; set; } 
        public string Nr_podzes { get; set; } 
        public string Data_prod { get; set; } 
        public string Data_gwar { get; set; } 
        public string Grupa_uz { get; set; } 
        public string Nr_part { get; set; } 
        public string Ost_nap { get; set; }         // 40
        public string Rok_nap { get; set; } 
        public string Przebieg { get; set; } 
        public string Zap_rem { get; set; } 
        public string Stan_spr { get; set; } 
        public string Jed_miary { get; set; } 
        public string Rodz_zap { get; set; } 
        public string Iden_prz { get; set; }        //47
        public string Iden_wyd { get; set; } 
        public string Iden_si { get; set; } 
        public string Uwagi { get; set; } 
        public string Jed_a { get; set; } 
        public string Uzyt_a { get; set; } 
        public string Kamo_a { get; set; } 
        public string Kumo_a { get; set; } 
        public string Kwpc_a { get; set; }      // 55
        public string Knab_a { get; set; } 
        public string Umo_pocz { get; set; } 
        public string Kod_kresk { get; set; } 
        public string Ilosc_inw { get; set; } 
        public string Korekta_um { get; set; }
    }
}
