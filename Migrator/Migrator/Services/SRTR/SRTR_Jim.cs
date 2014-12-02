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
        public static List<MagmatEwpb> AddJimData(string fileJimPath, List<MagmatEwpb> listMaterialy)
        {
            using (StreamReader sr = new StreamReader(fileJimPath, Encoding.Default))
            {
                string line = null;

                listMaterialy.ForEach(x => x.Info = "Brak JIM");

                while ((line = sr.ReadLine()) != null)
                {
                    listMaterialy.ForEach(x =>
                    {
                        if (x.Jim.Equals(line.Substring(0, 18).Trim()))
                        {
                            x.Info = string.Empty;
                            x.Klasyfikacja = line.Substring(255).Trim();
                        }
                    });
                }
            }

            List<MagmatEwpb> temp = new List<MagmatEwpb>();
            temp.AddRange(listMaterialy);

            return temp;
        }
        public static List<ZestawienieKlas> AddJimData(string fileJimPath, List<ZestawienieKlas> listZestawienieKlas, List<Zestawienie> listZestawienie)
        {
            using (StreamReader sr = new StreamReader(fileJimPath, Encoding.Default))
            {
                string line = null;

                while ((line = sr.ReadLine()) != null)
                {
                    foreach (var zestawienie in listZestawienieKlas)
                    {
                        if (zestawienie.Jim.Equals(line.Substring(0, 18).Trim()))
                        {
                            zestawienie.Jim = line.Substring(0, 18);
                            zestawienie.KlasaZaop = line.Substring(18, 1);
                            zestawienie.Nazwa = line.Substring(19, 40);
                            zestawienie.Jm = line.Substring(59, 3);
                            zestawienie.StaryNrInd = line.Substring(62, 18);
                            zestawienie.GrupaKlasaKwo = line.Substring(80, 9);
                            zestawienie.Gestor = line.Substring(89, 18);
                            zestawienie.KlasyfikatorHier = line.Substring(107, 18);
                            zestawienie.WagaBrutto = line.Substring(125, 17);
                            zestawienie.JednWagi = line.Substring(142, 3);
                            zestawienie.WagaNetto = line.Substring(145, 17);
                            zestawienie.Objetosc = line.Substring(162, 17);
                            zestawienie.JednObj = line.Substring(179, 3);
                            zestawienie.Wymiary = line.Substring(182, 32);
                            zestawienie.KodCpv = line.Substring(214, 18);
                            zestawienie.WyroznikCpv = line.Substring(232, 2);
                            zestawienie.Norma = line.Substring(234, 18);
                            zestawienie.WyroznikProdNiebezp = line.Substring(252, 3);
                            zestawienie.KlasyfikacjaPartii = line.Substring(255);
                        }
                    }

                    listZestawienie.ForEach(x =>
                    {
                        if (x.Jim.Equals(line.Substring(0, 18).Trim()))
                            x.Material = line.Substring(19, 40);
                    });
                }
            }

            List<ZestawienieKlas> temp = new List<ZestawienieKlas>();
            temp.AddRange(listZestawienieKlas);

            return temp;
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
                        using (StreamWriter writer = new StreamWriter(writeStream))
                        {
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
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        return string.Empty;
                    }
                }
            }

            return success;
        }
        public static string SaveFileDialog(List<MagmatEwpb> listMaterialy)
        {
            string success = "Plik zapisano poprawnie.";

            SaveFileDialog saveFile = new SaveFileDialog() { FileName = "wykaz_JIM", DefaultExt = ".text", Filter = "Dokumenty tekstowe (.txt)|*.txt" };

            if (saveFile.ShowDialog() == true)
            {
                using (Stream writeStream = saveFile.OpenFile())
                {
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(writeStream))
                        {
                            List<string> temp = new List<string>();

                            foreach (MagmatEwpb material in listMaterialy)
                            {
                                if (material.Jim.Length > 12 && material.Jim.Substring(4, 2).Equals("PL"))
                                {
                                    string line = material.Jim.Substring(6, 7);
                                    if (!temp.Contains(line))
                                    {
                                        temp.Add(line);
                                        writer.WriteLine(line);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        return string.Empty;
                    }
                }
            }

            return success;
        }
        public static string SaveFileDialog(List<ZestawienieKlas> listZestawieniaKlas)
        {
            string success = "Plik zapisano poprawnie.";

            SaveFileDialog saveFile = new SaveFileDialog() { FileName = "wykaz_JIM", DefaultExt = ".text", Filter = "Dokumenty tekstowe (.txt)|*.txt" };

            if (saveFile.ShowDialog() == true)
            {
                using (Stream writeStream = saveFile.OpenFile())
                {
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(writeStream))
                        {
                            List<string> temp = new List<string>();

                            foreach (ZestawienieKlas material in listZestawieniaKlas)
                            {
                                if (material.Jim.Length > 12 && material.Jim.Substring(4, 2).Equals("PL"))
                                {
                                    string line = material.Jim.Substring(6, 7);
                                    if (!temp.Contains(line))
                                    {
                                        temp.Add(line);
                                        writer.WriteLine(line);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        return string.Empty;
                    }
                }
            }

            return success;
        }
    }
}
