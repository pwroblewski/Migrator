using Microsoft.Win32;
using Migrator.Helpers;
using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Windows;

namespace Migrator.Services
{
    public class FileZestawienieService : IFileZestawienieService
    {
        public List<Zestawienie> _listZestawienie = new List<Zestawienie>();
        public List<ZestawienieKlas> _listZestawienieKlas = new List<ZestawienieKlas>();

        public string[] OpenFileDialog()
        {
            OpenFileDialog accessDialog = new OpenFileDialog() { DefaultExt = "txt", Filter = "Database files (*.txt)|*.txt|All Files (*.*)|*.*", AddExtension = true, Multiselect = true };

            if (accessDialog.ShowDialog() == true)
            {
                string safeFileName = accessDialog.SafeFileName;

                return accessDialog.FileNames;
            }
            else
                return null;
        }

        public List<Zestawienie> GetAll(string[] paths)
        {
            List<string> list_jim = new List<string>();

            for (int i = 0; i < paths.Length; i++)
            {
                using (StreamReader sr = new StreamReader(paths[i], Encoding.GetEncoding(1250)))
                {
                    string fileName = Path.GetFileName(paths[i]);
                    string line = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length > 17)
                        {
                            string jim = line.Substring(0, 18).Trim();
                            if (!list_jim.Contains(line.Substring(0, 18).Trim()))
                            {
                                list_jim.Add(jim);
                                string zaklad = fileName.Substring(0, 4);
                                string sklad = fileName.Substring(5, 4);
                                // TO DO obsługa czytania z pliku uzytkownika
                                string uzytkownik = string.Empty;

                                if (fileName[3].Equals('K'))
                                {
                                    // Materiał z EWPB 319/320
                                    uzytkownik = line.Split('\t')[line.Split('\t').Length - 1].Trim();
                                }

                                _listZestawienie.Add(new Zestawienie(jim, zaklad, sklad, uzytkownik));
                                _listZestawienieKlas.Add(new ZestawienieKlas() { Jim = jim });
                            }
                        }
                    }
                }
            }

            return _listZestawienie;
        }

        public List<ZestawienieKlas> AddJimData(string path)
        {
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                string line = null;

                while ((line = sr.ReadLine()) != null)
                {
                    foreach(var zestawienie in _listZestawienieKlas)
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

                    _listZestawienie.ForEach(x =>
                    {
                        if (x.Jim.Equals(line.Substring(0, 18).Trim()))
                            x.Material = line.Substring(19, 40);
                    });
                }
            }

            List<ZestawienieKlas> temp = new List<ZestawienieKlas>();
            temp.AddRange(_listZestawienieKlas);

            return temp;
        }

        public List<Zestawienie> GetZestawienie()
        {
            return _listZestawienie;
        }

        public List<ZestawienieKlas> GetZestawienieKlas()
        {
            return _listZestawienieKlas;
        }

        public void Clean()
        {
            _listZestawienie.Clear();
            _listZestawienieKlas.Clear();
        }


        public void ZapiszZestawienieKlas(List<ZestawienieKlas> listZestawienieKlas)
        {
            _listZestawienieKlas = listZestawienieKlas;
        }

        public void ZapiszPliki()
        {
            if (_listZestawienie != null && _listZestawienie.Count > 0 && _listZestawienieKlas != null && _listZestawienieKlas.Count > 0)
            {
                SaveFileDialog saveFile = new SaveFileDialog() { FileName = "MATERIAL_", DefaultExt = ".text", Filter = "Dokumenty tekstowe (.txt)|*.txt"};

                if (saveFile.ShowDialog() == true)
                {
                    using (Stream writeStream = saveFile.OpenFile())
                    {
                        try
                        {
                            StreamWriter writer = new StreamWriter(writeStream);
                            foreach (Zestawienie zestawienie in _listZestawienie)
                            {
                                foreach (ZestawienieKlas zestawienieKlas in _listZestawienieKlas)
                                {
                                    if (zestawienie.Jim.Equals(zestawienieKlas.Jim.Trim()) && zestawienieKlas.KlasaZaop != null)
                                    {
                                        writer.Write("{0}\t", zestawienieKlas.Jim.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.KlasaZaop.Trim());
                                        writer.Write("{0}\t", zestawienie.Zaklad.Trim());
                                        writer.Write("{0}\t", zestawienie.Sklad.Trim());
                                        writer.Write("{0}\t", zestawienie.Dzial.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.Nazwa.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.Jm.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.StaryNrInd.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.KlasyfikatorHier.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.GrupaKlasaKwo.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.WagaBrutto.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.WagaNetto.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.JednWagi.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.Objetosc.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.JednObj.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.Gestor.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.Norma.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.WyroznikProdNiebezp.Trim());
                                        writer.Write("{0}\t", zestawienie.SymbolKat.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.KodCpv.Trim());
                                        writer.Write("{0}\t", zestawienie.Wskaznik.Trim());
                                        writer.Write("{0}\t", zestawienie.ZapasBezp.Trim());
                                        writer.Write("{0}\t", zestawienie.TypWyceny.Trim());
                                        writer.Write("{0}\t", zestawienie.Rodzaj.Trim());
                                        writer.Write("{0}\t", zestawienie.MaterialKonto.Trim());
                                        writer.Write("{0}\t", zestawienie.StarCena.Trim());
                                        writer.Write("{0}\t", zestawienie.JednCena.Trim());
                                        writer.Write("{0}\t", zestawienie.CenaSrednia.Trim());
                                        writer.Write("{0}\t", zestawienie.CenaStand.Trim());
                                        writer.Write("{0}\t", zestawienie.ZakladDost.Trim());
                                        writer.Write("{0}\t", zestawienie.MaterialProf.Trim());
                                        writer.Write("{0}\t", zestawienieKlas.WyroznikCpv.Trim());
                                        writer.Write(writer.NewLine);
                                        writer.Flush();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            writeStream.Flush();
                            writeStream.Close();
                        }
                    }
                }

                SaveFileDialog saveFile2 = new SaveFileDialog() { FileName = "MATERIAL_KLAS_", DefaultExt = ".text", Filter = "Dokumenty tekstowe (.txt)|*.txt" };

                if (saveFile2.ShowDialog() == true)
                {
                    using (Stream writeStream = saveFile2.OpenFile())
                    {
                        try
                        {
                            StreamWriter writer = new StreamWriter(writeStream);
                            foreach (Zestawienie zestawienie in _listZestawienie)
                            {
                                foreach (ZestawienieKlas zestawienieKlas in _listZestawienieKlas)
                                {
                                    if (zestawienie.Jim.Equals(zestawienieKlas.Jim.Trim()))
                                    {
                                        writer.WriteLine("{0}\t022\t{1}", zestawienieKlas.Jim, zestawienieKlas.KlasyfikacjaPartii);
                                        writer.Flush();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            writeStream.Flush();
                            writeStream.Close();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Brak danych!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void SetZestawienie(List<Zestawienie> listZestawienie)
        {
            _listZestawienie = listZestawienie;
        }

        public void SetZestawienieKlas(List<ZestawienieKlas> listZestawienieKlas)
        {
            _listZestawienieKlas = listZestawienieKlas;
        }
    }
}
