using Microsoft.Win32;
using Migrator.Helpers;
using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Text;

namespace Migrator.Services.SRTR
{
    public static class SRTR_Jim
    {
        public static string OpenFileDialog()
        {
            OpenFileDialog accessDialog = new OpenFileDialog() { DefaultExt = "txt", Filter = "Text files (*.txt)|*.txt|All Files (*.*)|*.*", AddExtension = true };

            if (accessDialog.ShowDialog() == true)
            {
                string safeFileName = accessDialog.SafeFileName;

                return accessDialog.FileName;
            }
            else
                return string.Empty;
        }
        public static List<WykazIlosciowy> AddJimData(string fileJimPath, List<WykazIlosciowy> listWykazIlosciowy)
        {
            using (StreamReader sr = new StreamReader(fileJimPath, Encoding.Default))
            {
                string line = null;

                while ((line = sr.ReadLine()) != null)
                {
                    string indeksMaterialowy = line.Substring(0, 18).Trim();
                    string nazwa = line.Substring(18, 40).Trim();
                    string jm = line.Substring(58, 3).Trim();

                    listWykazIlosciowy.ForEach(x =>
                    {
                        if (x.IndeksMaterialowy.Equals(indeksMaterialowy))
                        {
                            x.NazwaMaterialu = nazwa;
                            x.JednostkaMiary = jm;
                        }
                    });
                }
            }

            return listWykazIlosciowy;
        }
        public static string SaveFileDialog(List<WykazIlosciowy> listWykazIlosciowy)
        {
            string success = "Plik zapisano poprawnie.";

            SaveFileDialog saveFile = new SaveFileDialog() { FileName = "wykaz_JIM", DefaultExt = ".text", Filter = "Dokumenty tekstowe (.txt)|*.txt" };

            if (saveFile.ShowDialog() == true)
            {
                using (Stream writeStream = saveFile.OpenFile())
                {
                    try
                    {
                        StreamWriter writer = new StreamWriter(writeStream);
                        List<string> temp = new List<string>();

                        foreach (WykazIlosciowy wykaz in listWykazIlosciowy)
                        {
                            if (wykaz.IndeksMaterialowy.Length > 12 && wykaz.IndeksMaterialowy.Substring(4, 2).Equals("PL"))
                            {
                                string line = wykaz.IndeksMaterialowy.Substring(6, 7);
                                if (!temp.Contains(line))
                                {
                                    temp.Add(line);
                                    writer.WriteLine(line);
                                }

                                writer.Flush();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        return string.Empty;
                    }
                    finally
                    {
                        writeStream.Flush();
                        writeStream.Close();
                    }
                }
            }

            return success;
        }
    }
}
