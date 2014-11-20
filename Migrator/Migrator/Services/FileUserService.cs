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



        public string OpenFileDialog()
        {
            throw new NotImplementedException();
        }

        public List<Uzytkownik> GetAll(string path)
        {
            throw new NotImplementedException();
        }
    }
}
