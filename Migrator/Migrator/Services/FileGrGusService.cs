using Microsoft.Win32;
using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using Migrator.Helpers;

namespace Migrator.Services
{
    class FileGrGusService : IFileGrGusService
    {
        private List<GrupaRodzajowaGusSRTR> _listGrGusSRTR = new List<GrupaRodzajowaGusSRTR>();

        public string OpenFileDialog()
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

        public List<GrupaRodzajowaGusSRTR> GetAll(string path)
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

                            GrupaRodzajowaGusSRTR grGus = new GrupaRodzajowaGusSRTR()
                            {
                                KodGrRodzSRTR = rd["GR_GUS"].ToString(),
                                NazwaGrRodzSRTR = KodowanieZnakow.PolskieZnaki(rd["GR_OPIS"].ToString(), Modul.SRTR)
                            };

                            _listGrGusSRTR.Add(grGus);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Wystąpił błąd podczas odczytu danych - {0}", ex.Message);
                    MessageBox.Show(message, "Bład odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return _listGrGusSRTR;
        }

        public List<GrupaRodzajowaGusSRTR> SyncData(List<GrupaRodzajowaGusSRTR> fileData, List<GrupaRodzajowaGusSRTR> listGrupaGus)
        {
            List<GrupaRodzajowaGusSRTR> temp = new List<GrupaRodzajowaGusSRTR>();
            foreach (GrupaRodzajowaGusSRTR grGus in listGrupaGus)
            {
                foreach (GrupaRodzajowaGusSRTR grGus2 in fileData)
                {
                    if (grGus.KodGrRodzSRTR.Equals(grGus2.KodGrRodzSRTR))
                        temp.Add(grGus2);
                }
            }
            return temp;
        }

        public void Clean()
        {
            _listGrGusSRTR.Clear();
        }
    }
}