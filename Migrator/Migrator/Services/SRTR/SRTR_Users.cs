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
    public static class SRTR_Users
    {
        public static string OpenFileDialog()
        {
            OpenFileDialog accessDialog = new OpenFileDialog() { DefaultExt = "dbf", Filter = "Database files (*.dbf)|*.dbf|All Files (*.*)|*.*", AddExtension = true };

            if (accessDialog.ShowDialog() == true)
            {
                string safeFileName = accessDialog.SafeFileName;

                if (safeFileName.Substring(0, 6).Equals("SL_UZY"))
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

        public static List<Uzytkownik> LoadData(string path)
        {
            using (OleDbConnection connection = new OleDbConnection(String.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}; Extended Properties=DBASE IV;", Path.GetDirectoryName(path))))
            {
                List<Uzytkownik> list = new List<Uzytkownik>();
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

                            Uzytkownik user = new Uzytkownik();
                            user.IdSrtr = rd["KOD_UZYT"].ToString().Trim();
                            user.NazwaUzytkownika = KodowanieZnakow.PolskieZnaki(rd["NAZWA_UZYT"].ToString().Trim(), Modul.SRTR);
                            user.KodJednostki = KodowanieZnakow.PolskieZnaki(rd["KOD_JED"].ToString().Trim(), Modul.SRTR);
                            user.TypUzytkownika = KodowanieZnakow.PolskieZnaki(rd["TYP_UZYT"].ToString().Trim(), Modul.SRTR);
                            user.OsobaUpowazniona = KodowanieZnakow.PolskieZnaki(rd["OSOBA_UP"].ToString().Trim(), Modul.SRTR);
                            user.Poczta = KodowanieZnakow.PolskieZnaki(rd["POCZTA"].ToString().Trim(), Modul.SRTR);
                            user.Ulica = KodowanieZnakow.PolskieZnaki(rd["ULICA"].ToString().Trim(), Modul.SRTR);
                            user.Telefon = KodowanieZnakow.PolskieZnaki(rd["TELEFON"].ToString().Trim(), Modul.SRTR);
                            user.IdZwsiron = KodowanieZnakow.PolskieZnaki(rd["TELEFAX"].ToString().Trim(), Modul.SRTR);

                            if (user.Ulica.Length == 10)
                            {
                                long number;
                                if (long.TryParse(user.Ulica, out number))
                                {
                                    user.Mpk = user.Ulica;
                                }
                            }

                            if (user.IdZwsiron.Equals("6999999"))
                                user.IdZwsiron = null;

                            list.Add(user);
                        }
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
