using Microsoft.Win32;
using Migrator.Helpers;
using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Windows;

namespace Migrator.Services
{
    public class FileUserService : IFileUserService
    {
        private List<Uzytkownik> _listUzytkownik = new List<Uzytkownik>();

        public string OpenFileDialog()
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

        public List<Uzytkownik> GetAll(string path)
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

                            if(user.Ulica.Length == 10)
                            {
                                long number;
                                if(long.TryParse(user.Ulica, out number))
                                {
                                    user.Mpk = user.Ulica;
                                }
                            }

                            if (user.IdZwsiron.Equals("6999999"))
                                user.IdZwsiron = null;

                            _listUzytkownik.Add(user);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Wystąpił błąd podczas odczytu danych - {0}", ex.Message);
                    MessageBox.Show(message, "Bład odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return _listUzytkownik;
        }

        public List<Uzytkownik> SyncData(List<Uzytkownik> tempList, List<Uzytkownik> listUzytkownicy)
        {
            List<Uzytkownik> temp = new List<Uzytkownik>();
            foreach (Uzytkownik user in listUzytkownicy)
            {
                foreach (Uzytkownik user2 in tempList)
                {
                    if (user.IdSrtr.Equals(user2.IdSrtr))
                        temp.Add(user2);
                }
            }
            return temp;
        }

        public List<MagmatEwpb> GetUserData(List<MagmatEwpb> listMaterialy, string path)
        {
            Clean();

            using (OleDbConnection connection = new OleDbConnection(String.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}; Extended Properties=DBASE IV;", Path.GetDirectoryName(path))))
            {
                try
                {
                    connection.Open();

                    using (OleDbCommand cmd = connection.CreateCommand())
                    {
                        string command = string.Format("SELECT KOD_UZYT, NAZWA_UZYT, OSOBA_UP, TELEFAX FROM {0}", Path.GetFileNameWithoutExtension(path));
                        cmd.CommandText = command;

                        OleDbDataReader rd = cmd.ExecuteReader();

                        while (rd.Read())
                        {
                            listMaterialy.ForEach(x =>
                            {
                                if (x.Uzytkownik == rd["KOD_UZYT"].ToString().PadLeft(4, '0'))
                                {
                                    x.NazwaUzytkownika = KodowanieZnakow.PolskieZnaki(rd["NAZWA_UZYT"].ToString().Trim(), Modul.SRTR).ToUpper();
                                    x.OsobaUpowazniona = KodowanieZnakow.PolskieZnaki(rd["OSOBA_UP"].ToString().Trim(), Modul.SRTR).ToUpper();
                                    x.UzytkownikZwsiron = rd["TELEFAX"].ToString().Trim();
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Wystąpił błąd podczas odczytu danych - {0}", ex.Message);
                    MessageBox.Show(message, "Bład odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            List<MagmatEwpb> temp = new List<MagmatEwpb>();
            temp.AddRange(listMaterialy);

            return temp;
        }

        public void Clean()
        {
            _listUzytkownik.Clear();
        }


    }
}
