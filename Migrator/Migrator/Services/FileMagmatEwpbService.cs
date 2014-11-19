using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Windows;
using Migrator.Model;
using System.Data.OleDb;
using System.IO;
using Migrator.Helpers;
using System.Text;

namespace Migrator.Services
{
    public class FileMagmatEwpbService : IFileMagmatEwpbService
    {
        private List<MagmatEwpb> _listMagmatEwpb = new List<MagmatEwpb>();
        private List<MagmatEwpb> _listConversion = new List<MagmatEwpb>();

        public string OpenFileDialog()
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

        public List<MagmatEwpb> GetAll(string path)
        {
            Clean();

            using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding(1250)))
            {
                string line = null;
                string[] prevSubLines = null;
                int lp = 1;

                try
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        #region MAGMAT 305

                        if (path.Contains("305"))
                        {
                            if (line.Length > 4 && line[0].Equals('|'))
                            {
                                string[] subLines = line.Split('|');
                                if (!subLines[1].Trim().Equals("LP") && !string.IsNullOrWhiteSpace(subLines[1].Trim()) && subLines.Length >= 9)
                                {
                                    MagmatEwpb material = new MagmatEwpb();

                                    material.Lp = lp;
                                    material.Jim = subLines[3].Trim().Substring(0, 13);
                                    material.Material = KodowanieZnakow.PolskieZnaki(subLines[4].Trim(), Modul.MAGMAT_EWPB);
                                    material.Jm = subLines[5].Trim();
                                    material.Ilosc = Convert.ToInt32(Convert.ToDouble(subLines[6].Trim().Replace(".", String.Empty)));
                                    material.Kategoria = PrzypiszKategorieMagmat(subLines[3]);
                                    material.Wartosc = subLines[8].Trim().Replace(".", String.Empty);
                                    material.Cena = subLines[7].Trim().Replace(".", String.Empty);
                                    material.Kategoria = null;
                                    material.NrSeryjny = null;

                                    material.NrMagazynu = subLines[2].Trim();

                                    _listMagmatEwpb.Add(material);
                                    lp++;
                                }
                            }
                        }

                        #endregion
                        #region EWPB 319/320

                        else if (path.Contains("319") || path.Contains("320"))
                        {
                            if (line.Length > 2 && line[0].Equals('|') && line[7].Equals(':'))
                            {
                                string[] subLines = line.Split('|');

                                MagmatEwpb material = new MagmatEwpb();

                                material.Lp = lp;
                                material.Jim = subLines[1].Substring(13, 13);
                                material.Material = KodowanieZnakow.PolskieZnakiEWPB(subLines[2].Trim());
                                material.Jm = subLines[3].Trim();
                                material.Ilosc = Convert.ToInt32(Convert.ToDouble(subLines[4].Trim().Replace(".", String.Empty)));
                                material.Wartosc = "0";
                                material.Cena = "X";
                                material.Kategoria = PrzypiszKategorieEwpb319(subLines[1]);
                                material.NrSeryjny = string.Empty;

                                material.Uzytkownik = subLines[1].Trim().Substring(0, 5).Trim();

                                _listMagmatEwpb.Add(material);
                                lp++;
                            }
                        }
                        #endregion
                        #region EWPB 351

                        else if (path.Contains("351"))
                        {
                            if (line.Length > 0 && line[0].ToString().Equals("|") && line[7].ToString().Equals("|") && !line[6].ToString().Equals("=") && !line[3].ToString().Equals("L") && !line[9].ToString().Equals("I"))
                            {
                                if (string.IsNullOrWhiteSpace(line[6].ToString()))
                                {
                                    string[] subLines = line.Split('|');

                                    MagmatEwpb material = new MagmatEwpb();

                                    material.Lp = lp;
                                    material.Jim = subLines[2].Trim().Substring(0, 13);
                                    material.Material = KodowanieZnakow.PolskieZnakiEWPB(prevSubLines[3].Trim());
                                    material.Jm = prevSubLines[4].Trim();
                                    material.Ilosc = Convert.ToInt32(Convert.ToDouble(prevSubLines[5].Trim().Replace(".", String.Empty)));
                                    material.Wartosc = subLines[6].Trim();
                                    material.Cena = "X";
                                    material.Kategoria = PrzypiszKategorieEwpb351(subLines[2].Trim());
                                    material.NrSeryjny = string.Empty;
                                    material.Jednostka = prevSubLines[2].Trim();

                                    _listMagmatEwpb.Add(material);
                                    lp++;

                                }
                                else
                                {
                                    prevSubLines = line.Split('|');
                                }
                            }
                        }

                        #endregion
                    }
                }
                catch
                {
                    MessageBox.Show("Nie wszystkie dane zostały odczytane poprawnie. Zweryfikuj dane i ponownie wczytaj plik.", "Wykryto niepoprawną strukturę pliku!");
                }

                return _listMagmatEwpb;
            }
        }

        private string PrzypiszKategorieMagmat(string indeks)
        {
            if (indeks.Trim().Contains("@"))
            {
                return indeks.Trim().Split('@')[1];
            }

            return "1";
        }

        private string PrzypiszKategorieEwpb319(string indeks)
        {
            return "2";
        }

        private string PrzypiszKategorieEwpb351(string indeks)
        {
            if(indeks.Contains("@"))
            {
                return indeks.Split('@')[1];
            }

            return "2";
        }

        public void AddApp(MagmatEWPB typ)
        {
            _listMagmatEwpb.ForEach(x => x.App = typ.ToString());
        }

        public void AddZakladSklad(string zaklad, string sklad)
        {
            _listMagmatEwpb.ForEach(x =>
                {
                    x.Sklad = sklad;
                    x.Zaklad = zaklad;
                });
        }

        public List<MagmatEwpb> GetData()
        {
            return _listMagmatEwpb;
        }

        public void SetData(List<MagmatEwpb> listMagmatEwpb)
        {
            _listMagmatEwpb = listMagmatEwpb;
        }

        public List<MagmatEwpb> GetDictData(MagmatEWPB module)
        {
            switch (module)
            {
                case MagmatEWPB.Magmat305:
                    foreach (MagmatEwpb mat in _listMagmatEwpb)
                    {
                        if (!_listConversion.Exists(x => x.NrMagazynu == mat.NrMagazynu))
                            _listConversion.Add(mat);
                    }
                    break;

                case MagmatEWPB.Ewpb319_320:
                    foreach (MagmatEwpb mat in _listMagmatEwpb)
                    {
                        if (!_listConversion.Exists(x => x.Uzytkownik == mat.Uzytkownik))
                            _listConversion.Add(mat);
                    }
                    break;

                case MagmatEWPB.EWpb351:
                    foreach (MagmatEwpb mat in _listMagmatEwpb)
                    {
                        if (!_listConversion.Exists(x => x.Jednostka == mat.Jednostka))
                            _listConversion.Add(mat);
                    }
                    break;
            }

            return _listConversion;
        }

        public void Clean()
        {
            _listMagmatEwpb.Clear();
            _listConversion.Clear();
        }
    }
}
