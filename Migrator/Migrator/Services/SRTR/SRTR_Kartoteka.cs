using Microsoft.Win32;
using Migrator.Helpers;
using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Windows;

namespace Migrator.Services.SRTR
{
    public static class SRTR_Kartoteka
    {
        public static string OpenFileDialog()
        {
            OpenFileDialog accessDialog = new OpenFileDialog() { DefaultExt = "dbf", Filter = "Database files (*.dbf)|*.dbf|All Files (*.*)|*.*", AddExtension = true };

            if (accessDialog.ShowDialog() == true)
            {
                string safeFileName = accessDialog.SafeFileName;

                if (safeFileName.Substring(0, 6).Equals("DM_KAR"))
                    return accessDialog.FileName;
                else
                {
                    MessageBox.Show("Niepoprawna nazwa pliku", "Bład odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
                    return string.Empty;
                }
            }
            else
                return string.Empty;
        }

        public static List<List<KartotekaSRTR>> LoadData(string path)
        {
            using (OleDbConnection connection = new OleDbConnection(String.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}; Extended Properties=DBASE IV;", Path.GetDirectoryName(path))))
            {
                List<List<KartotekaSRTR>> list = new List<List<KartotekaSRTR>>();
                List<KartotekaSRTR> listKartoteka = new List<KartotekaSRTR>();
                List<KartotekaSRTR> listKartotekaZlik = new List<KartotekaSRTR>();
                try
                {
                    connection.Open();

                    using (OleDbCommand cmd = connection.CreateCommand())
                    {
                        string command = string.Format("SELECT * FROM {0}", Path.GetFileNameWithoutExtension(path));
                        cmd.CommandText = command;

                        OleDbDataReader rd = cmd.ExecuteReader();

                        while (rd.Read())
                        {

                            KartotekaSRTR kartoteka = new KartotekaSRTR();
                            {
                                #region KartotekaSRTR object
                                kartoteka.Nr_kolejny = rd["NR_KOLEJNY"].ToString();
                                kartoteka.Indeks_m = rd["INDEKS_M"].ToString();
                                kartoteka.Nazwa_sr = KodowanieZnakow.PolskieZnaki(rd["NAZWA_SR"].ToString(), Modul.SRTR);
                                kartoteka.Gr_gus = rd["GR_GUS"].ToString();
                                kartoteka.Kod_jed = rd["KOD_JED"].ToString();
                                kartoteka.Kod_uzyt = rd["KOD_UZYT"].ToString();
                                kartoteka.Konto_amo = rd["KONTO_AMO"].ToString();
                                kartoteka.Konto_umo = rd["KONTO_UMO"].ToString();
                                kartoteka.Konto_wpc = rd["KONTO_WPC"].ToString();
                                kartoteka.Rodz_amor = rd["RODZ_AMOR"].ToString();
                                kartoteka.Kat_sprz = rd["KAT_SPRZ"].ToString();
                                kartoteka.Data_nab = rd["DATA_NAB"].ToString();
                                kartoteka.Dowod_nab = rd["DOWOD_NAB"].ToString();
                                kartoteka.Konto_nab = rd["KONTO_NAB"].ToString();
                                kartoteka.Uwagi_nab = rd["UWAGI_NAB"].ToString();
                                kartoteka.Data_prz = rd["DATA_PRZ"].ToString();
                                kartoteka.War_pocz = rd["WAR_POCZ"].ToString();
                                kartoteka.Bo_wart_in = rd["BO_WART_IN"].ToString();
                                kartoteka.Bo_wart_um = rd["BO_WART_UM"].ToString();
                                kartoteka.Data_lik = rd["DATA_LIK"].ToString();
                                kartoteka.Wart_inw_2 = rd["WART_INW_2"].ToString();
                                kartoteka.Wsp_am_1 = rd["WSP_AM_1"].ToString();
                                kartoteka.Wsp_am_2 = rd["WSP_AM_2"].ToString();
                                kartoteka.Bo_am_2 = rd["BO_AM_2"].ToString();
                                kartoteka.Wsk_bl = rd["WSK_BL"].ToString();
                                kartoteka.Rodz_lik = rd["RODZ_LIK"].ToString();
                                kartoteka.Blok_am = rd["BLOK_AM"].ToString();
                                kartoteka.Am_sezon = rd["AM_SEZON"].ToString();
                                kartoteka.Nr_seryjny = rd["NR_SERYJNY"].ToString();
                                kartoteka.Data_nab2 = rd["DATA_NAB2"].ToString();
                                kartoteka.Nr_dok = rd["NR_DOK"].ToString();
                                kartoteka.Data_dok = rd["DATA_DOK"].ToString();
                                kartoteka.Nr_jw = rd["NR_JW"].ToString();
                                kartoteka.Nr_branz = rd["NR_BRANZ"].ToString();
                                kartoteka.Nr_rejest = rd["NR_REJEST"].ToString();
                                kartoteka.Nr_podzes = rd["NR_PODZES"].ToString();
                                kartoteka.Data_prod = rd["DATA_PROD"].ToString();
                                kartoteka.Data_gwar = rd["DATA_GWAR"].ToString();
                                kartoteka.Grupa_uz = rd["GRUPA_UZ"].ToString();
                                kartoteka.Nr_part = rd["NR_PART"].ToString();
                                kartoteka.Ost_nap = rd["OST_NAP"].ToString();
                                kartoteka.Rok_nap = rd["ROK_NAP"].ToString();
                                kartoteka.Przebieg = rd["PRZEBIEG"].ToString();
                                kartoteka.Zap_rem = rd["ZAP_REM"].ToString();
                                kartoteka.Stan_spr = rd["STAN_SPR"].ToString();
                                kartoteka.Jed_miary = rd["JED_MIARY"].ToString();
                                kartoteka.Rodz_zap = rd["RODZ_ZAP"].ToString();
                                kartoteka.Iden_prz = rd["IDEN_PRZ"].ToString();
                                kartoteka.Iden_wyd = rd["IDEN_WYD"].ToString();
                                kartoteka.Iden_si = rd["IDEN_SI"].ToString();
                                kartoteka.Uwagi = rd["UWAGI"].ToString();
                                kartoteka.Jed_a = rd["JED_A"].ToString();
                                kartoteka.Uzyt_a = rd["UZYT_A"].ToString();
                                kartoteka.Kamo_a = rd["KAMO_A"].ToString();
                                kartoteka.Kumo_a = rd["KUMO_A"].ToString();
                                kartoteka.Kwpc_a = rd["KWPC_A"].ToString();
                                kartoteka.Knab_a = rd["KNAB_A"].ToString();
                                kartoteka.Umo_pocz = rd["UMO_POCZ"].ToString();
                                kartoteka.Kod_kresk = rd["KOD_KRESK"].ToString();
                                kartoteka.Ilosc_inw = rd["ILOSC_INW"].ToString();
                                kartoteka.Korekta_um = rd["KOREKTA_UM"].ToString();
                                #endregion
                            };

                            if (kartoteka.Konto_wpc != null && kartoteka.Konto_wpc.Substring(0, 4).Equals("3103") && kartoteka.Data_lik == null)
                                listKartotekaZlik.Add(kartoteka);
                            else
                                listKartoteka.Add(kartoteka);
                        }

                        list.Add(listKartoteka);
                        list.Add(listKartotekaZlik);
                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Wystąpił błąd podczas odczytu danych - {0}", ex.Message);
                    MessageBox.Show(message, "Bład odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return list;
            }
        }
    }
}
