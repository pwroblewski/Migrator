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
    public static class SRTR_GrGus
    {
        public static string OpenFileDialog()
        {
            OpenFileDialog accessDialog = new OpenFileDialog() { DefaultExt = "dbf", Filter = "Database files (*.dbf)|*.dbf|All Files (*.*)|*.*", AddExtension = true };

            if (accessDialog.ShowDialog() == true)
            {
                string safeFileName = accessDialog.SafeFileName;

                if (safeFileName.Substring(0, 6).Equals("SL_GUS"))
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

        public static List<GrupaRodzajowaGusSRTR> LoadData(string path)
        {
            using (OleDbConnection connection = new OleDbConnection(String.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}; Extended Properties=DBASE IV;", Path.GetDirectoryName(path))))
            {
                List<GrupaRodzajowaGusSRTR> list = new List<GrupaRodzajowaGusSRTR>();

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

                            GrupaRodzajowaGusSRTR grGus = new GrupaRodzajowaGusSRTR()
                            {
                                KodGrRodzSRTR = rd["GR_GUS"].ToString(),
                                NazwaGrRodzSRTR = KodowanieZnakow.PolskieZnaki(rd["GR_OPIS"].ToString(), Modul.SRTR)
                            };

                            list.Add(grGus);
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
