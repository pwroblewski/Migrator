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
    public class FileMagazynService : IFileMagazynService
    {
        private List<Magazyn> _listMagazyny = new List<Magazyn>();

        public string OpenFileDialog()
        {
            OpenFileDialog accessDialog = new OpenFileDialog() { DefaultExt = "dbf", Filter = "Database files (*.dbf)|*.dbf|All Files (*.*)|*.*", AddExtension = true };

            if (accessDialog.ShowDialog() == true)
            {
                string safeFileName = accessDialog.SafeFileName;

                if (safeFileName.Substring(0, 8).Equals("MAGAZYNY"))
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

        public List<Magazyn> GetAll(string path)
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

                            Magazyn magazyn = new Magazyn()
                            {
                                NrMagazynu = rd["NR_MAG"].ToString(),
                                NazwaMagazynu = KodowanieZnakow.PolskieZnaki(rd["NAZ_MAG"].ToString(), Modul.MAGMAT_EWPB),
                                Zaklad = rd["ZAKLAD"].ToString(),
                                Sklad = rd["SKLAD"].ToString(),
                            };

                            _listMagazyny.Add(magazyn);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Wystąpił błąd podczas odczytu danych - {0}", ex.Message);
                    MessageBox.Show(message, "Bład odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return _listMagazyny;
        }

        public List<MagmatEwpb> GetMagData(List<MagmatEwpb> listMaterialy, string path)
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
                            listMaterialy.ForEach(x =>
                                {
                                    if (x.NrMagazynu == rd["NR_MAG"].ToString())
                                    {
                                        x.NazwaMagazynu = KodowanieZnakow.PolskieZnaki(rd["NAZ_MAG"].ToString(), Modul.SRTR).ToUpper();
                                        x.Zaklad = rd["ZAKLAD"].ToString();
                                        x.Sklad = rd["SKLAD"].ToString();
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
            _listMagazyny.Clear();
        }
    }
}
