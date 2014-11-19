using Microsoft.Win32;
using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using Migrator.Helpers;
using System.IO.Compression;

namespace Migrator.Services
{
    public class FileKartotekaService : IFileKartotekaService
    {
        private List<KartotekaSRTR> _listKartoteka = new List<KartotekaSRTR>();
        private List<KartotekaSRTR> _listKartotekaZlik = new List<KartotekaSRTR>();

        public string OpenFileDialog()
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

        public List<KartotekaSRTR> GetAll(string path)
        {
            Clean();

            using (OleDbConnection connection = new OleDbConnection(String.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}; Extended Properties=DBASE IV;", Path.GetDirectoryName(path))))
            {
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

                            KartotekaSRTR kartoteka = new KartotekaSRTR()
                            {
                                #region KartotekaSRTR object
                                Nr_kolejny = rd["NR_KOLEJNY"].ToString(),
                                Indeks_m = rd["INDEKS_M"].ToString(),
                                Nazwa_sr = KodowanieZnakow.PolskieZnaki(rd["NAZWA_SR"].ToString(), Modul.SRTR),
                                Gr_gus = rd["GR_GUS"].ToString(),
                                Kod_jed = rd["KOD_JED"].ToString(),
                                Kod_uzyt = rd["KOD_UZYT"].ToString(),
                                Konto_amo = rd["KONTO_AMO"].ToString(),
                                Konto_umo = rd["KONTO_UMO"].ToString(),
                                Konto_wpc = rd["KONTO_WPC"].ToString(),
                                Rodz_amor = rd["RODZ_AMOR"].ToString(),
                                Kat_sprz = rd["KAT_SPRZ"].ToString(),
                                Data_nab = rd["DATA_NAB"].ToString(),
                                Dowod_nab = rd["DOWOD_NAB"].ToString(),
                                Konto_nab = rd["KONTO_NAB"].ToString(),
                                Uwagi_nab = rd["UWAGI_NAB"].ToString(),
                                Data_prz = rd["DATA_PRZ"].ToString(),
                                War_pocz = rd["WAR_POCZ"].ToString(),
                                Bo_wart_in = rd["BO_WART_IN"].ToString(),
                                Bo_wart_um = rd["BO_WART_UM"].ToString(),
                                Data_lik = rd["DATA_LIK"].ToString(),
                                Wart_inw_2 = rd["WART_INW_2"].ToString(),
                                Wsp_am_1 = rd["WSP_AM_1"].ToString(),
                                Wsp_am_2 = rd["WSP_AM_2"].ToString(),
                                Bo_am_2 = rd["BO_AM_2"].ToString(),
                                Wsk_bl = rd["WSK_BL"].ToString(),
                                Rodz_lik = rd["RODZ_LIK"].ToString(),
                                Blok_am = rd["BLOK_AM"].ToString(),
                                Am_sezon = rd["AM_SEZON"].ToString(),
                                Nr_seryjny = rd["NR_SERYJNY"].ToString(),
                                Data_nab2 = rd["DATA_NAB2"].ToString(),
                                Nr_dok = rd["NR_DOK"].ToString(),
                                Data_dok = rd["DATA_DOK"].ToString(),
                                Nr_jw = rd["NR_JW"].ToString(),
                                Nr_branz = rd["NR_BRANZ"].ToString(),
                                Nr_rejest = rd["NR_REJEST"].ToString(),
                                Nr_podzes = rd["NR_PODZES"].ToString(),
                                Data_prod = rd["DATA_PROD"].ToString(),
                                Data_gwar = rd["DATA_GWAR"].ToString(),
                                Grupa_uz = rd["GRUPA_UZ"].ToString(),
                                Nr_part = rd["NR_PART"].ToString(),
                                Ost_nap = rd["OST_NAP"].ToString(),
                                Rok_nap = rd["ROK_NAP"].ToString(),
                                Przebieg = rd["PRZEBIEG"].ToString(),
                                Zap_rem = rd["ZAP_REM"].ToString(),
                                Stan_spr = rd["STAN_SPR"].ToString(),
                                Jed_miary = rd["JED_MIARY"].ToString(),
                                Rodz_zap = rd["RODZ_ZAP"].ToString(),
                                Iden_prz = rd["IDEN_PRZ"].ToString(),
                                Iden_wyd = rd["IDEN_WYD"].ToString(),
                                Iden_si = rd["IDEN_SI"].ToString(),
                                Uwagi = rd["UWAGI"].ToString(),
                                Jed_a = rd["JED_A"].ToString(),
                                Uzyt_a = rd["UZYT_A"].ToString(),
                                Kamo_a = rd["KAMO_A"].ToString(),
                                Kumo_a = rd["KUMO_A"].ToString(),
                                Kwpc_a = rd["KWPC_A"].ToString(),
                                Knab_a = rd["KNAB_A"].ToString(),
                                Umo_pocz = rd["UMO_POCZ"].ToString(),
                                Kod_kresk = rd["KOD_KRESK"].ToString(),
                                Ilosc_inw = rd["ILOSC_INW"].ToString(),
                                Korekta_um = rd["KOREKTA_UM"].ToString()
                                #endregion
                            };

                            if (kartoteka.Konto_wpc != null && kartoteka.Konto_wpc.Substring(0, 4).Equals("3103") && kartoteka.Data_lik == null)
                                _listKartotekaZlik.Add(kartoteka);
                            else
                                _listKartoteka.Add(kartoteka);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Wystąpił błąd podczas odczytu danych - {0}", ex.Message);
                    MessageBox.Show(message, "Bład odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return _listKartoteka;
        }

        public List<KartotekaSRTR> GetKartoteka()
        {
            return _listKartoteka;
        }

        public void SetKartoteka(List<KartotekaSRTR> listKartoteka)
        {
            _listKartoteka = listKartoteka;
        }

        public List<KartotekaSRTR> GetMagmatData()
        {
            return _listKartotekaZlik;
        }

        public void Clean()
        {
            _listKartoteka.Clear();
            _listKartotekaZlik.Clear();
        }
    }
}