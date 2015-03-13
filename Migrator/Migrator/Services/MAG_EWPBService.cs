using Migrator.Helpers;
using Migrator.Model;
using Migrator.Model.State;
using Migrator.Services.MAGMAT_EWPB;
using Migrator.Services.SRTR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Migrator.Services
{
    public class MAG_EWPBService : IMAG_EWPBService
    {
        private MagmatEwpbState stan = new MagmatEwpbState();
        private Encoding enc = Encoding.GetEncoding("Windows-1250");

        #region Properties
        public MagmatEwpbState MagmatEwpbState
        {
            get { return stan; }
            set { stan = value; }
        }
        public string ViewName
        {
            get { return stan.ViewName; }
            set { stan.ViewName = value; }
        }
        public MagmatEWPB TypWydruku
        {
            get { return stan.TypWydruku; }
            set { stan.TypWydruku = value; }
        }
        public List<MagmatEwpb> Materialy
        {
            get { return stan.Materialy; }
            set { stan.Materialy = value; }
        }
        public List<MagmatEwpb> Dictionaries
        {
            get { return stan.Dictionaries; }
            set { stan.Dictionaries = value; }
        }
        public List<SigmatKat> Kat
        {
            get { return stan.Kat; }
            set { stan.Kat = value; }
        }
        public List<SigmatAmunicja> Amunicja
        {
            get { return stan.Amunicja; }
            set { stan.Amunicja = value; }
        }
        public List<SigmatMund> Mund
        {
            get { return stan.Mund; }
            set { stan.Mund = value; }
        }
        public List<SigmatPaliwa> Paliwa
        {
            get { return stan.Paliwa; }
            set { stan.Paliwa = value; }
        }
        public List<SigmatZywnosc> Zywnosc
        {
            get { return stan.Zywnosc; }
            set { stan.Zywnosc = value; }
        }
        #endregion

        #region MagmatEwpbFile
        public string OpenFile(string wydruk)
        {
            return MAG_EWPB_File.OpenFileDialog(wydruk);
        }
        public void LoadData(string path)
        {
            Materialy = MAG_EWPB_File.LoadData(path);
            UstawTypWydruku(path);
        }
        public void AddDomyslnyZakladSklad(string zaklad, string sklad)
        {
            if (zaklad != null && sklad != null)
            {
                Materialy.ForEach(x =>
                    {
                        x.Zaklad = zaklad;
                        x.Sklad = sklad;
                    });
            }
        }
        #endregion
        #region Dictionary
        public void FillDictionaryData()
        {
            Dictionaries = new List<MagmatEwpb>();

            switch (TypWydruku)
            {
                case MagmatEWPB.Magmat_305:
                    foreach (MagmatEwpb mat in Materialy)
                    {
                        if (!Dictionaries.Exists(x => x.NrMagazynu == mat.NrMagazynu))
                            Dictionaries.Add(mat);
                    }
                    break;

                case MagmatEWPB.EWPB_319_320:
                    foreach (MagmatEwpb mat in Materialy)
                    {
                        if (!Dictionaries.Exists(x => x.Uzytkownik == mat.Uzytkownik))
                            Dictionaries.Add(mat);
                    }
                    break;

                case MagmatEWPB.EWPB_351:
                    foreach (MagmatEwpb mat in Materialy)
                    {
                        if (!Dictionaries.Exists(x => x.Jednostka == mat.Jednostka))
                            Dictionaries.Add(mat);
                    }
                    break;
            }
        }
        public string OpenDictionaryFile()
        {
            switch (TypWydruku)
            {
                case MagmatEWPB.Magmat_305:
                    return MAG_EWPB_Magazyn.OpenFileDialog();

                case MagmatEWPB.EWPB_319_320:
                    return SRTR_Users.OpenFileDialog();

                default:
                    return MAG_EWPB_Jednostka.OpenFileDialog();
            }
        }
        public void LoadDictionaryData(string path)
        {
            switch (TypWydruku)
            {
                case MagmatEWPB.Magmat_305:
                    var magazyny = MAG_EWPB_Magazyn.LoadData(path);
                    SynchronizujDaneMagazynow(magazyny);
                    break;

                case MagmatEWPB.EWPB_319_320:
                    var users = SRTR_Users.LoadData(path);
                    SynchronizujDaneUzytkownikow(users);
                    break;

                default:
                    var jednostki = MAG_EWPB_Jednostka.LoadData(path);
                    SynchronizujDaneJednostek(jednostki);
                    break;
            }
        }
        public List<Jednostka> LoadSlJedData(string path)
        {
            return MAG_EWPB_Jednostka.LoadData(path);
        }
        private void SynchronizujDaneJednostek(List<Jednostka> jednostki)
        {
            Dictionaries.ForEach(x =>
                {
                    jednostki.ForEach(y =>
                        {
                            if (x.Jednostka.Equals(y.KodJednostki))
                            {
                                x.NazwaJednostki = y.NazwaJednostki;
                                x.Zaklad = y.Zaklad;
                                x.Sklad = y.Sklad;
                            }
                        });
                });
        }
        private void SynchronizujDaneUzytkownikow(List<Uzytkownik> users)
        {
            Dictionaries.ForEach(x =>
            {
                users.ForEach(y =>
                {
                    if (x.Uzytkownik.Equals(y.IdSrtr))
                    {
                        x.UzytkownikZwsiron = y.IdZwsiron;
                        x.NazwaUzytkownika = y.NazwaUzytkownika;
                        x.OsobaUpowazniona = y.OsobaUpowazniona;
                    }
                });
            });
        }
        private void SynchronizujDaneMagazynow(List<Magazyn> magazyny)
        {
            Dictionaries.ForEach(x =>
            {
                magazyny.ForEach(y =>
                {
                    if (x.NrMagazynu.Equals(y.NrMagazynu))
                    {
                        x.NazwaMagazynu = y.NazwaMagazynu;
                        x.Zaklad = y.Zaklad;
                        x.Sklad = y.Sklad;
                    }
                });
            });
        }
        #endregion
        #region Jim
        public string SaveJimFile()
        {
            return SRTR_Jim.SaveFileDialog(Materialy);
        }
        public string OpenJimFile()
        {
            return SRTR_Jim.OpenFileDialog();
        }
        public void AddJimData(string fileJimPath)
        {
            var materialy = SRTR_Jim.AddJimData(fileJimPath, Materialy);
            Materialy = materialy;
        }
        #endregion

        #region Helpers
        private void UstawTypWydruku(string path)
        {
            if (path.Contains("305"))
                TypWydruku = MagmatEWPB.Magmat_305;
            else if (path.Contains("319") || path.Contains("320"))
                TypWydruku = MagmatEWPB.EWPB_319_320;
            else
                TypWydruku = MagmatEWPB.EWPB_351;
        }
        public void AddDictionary()
        {
            Materialy.ForEach(x =>
            {
                switch (TypWydruku)
                {
                    case MagmatEWPB.Magmat_305:
                        var mag = Dictionaries.Find(y => y.NrMagazynu == x.NrMagazynu);
                        x.NazwaMagazynu = mag.NazwaMagazynu;
                        x.Zaklad = mag.Zaklad;
                        x.Sklad = mag.Sklad;
                        break;
                    case MagmatEWPB.EWPB_319_320:
                        var ewpb319 = Dictionaries.Find(y => y.Uzytkownik == x.Uzytkownik);
                        x.UzytkownikZwsiron = ewpb319.UzytkownikZwsiron;
                        x.NazwaUzytkownika = ewpb319.NazwaUzytkownika;
                        x.Zaklad = ewpb319.Zaklad;
                        x.Sklad = ewpb319.Sklad;
                        break;
                    case MagmatEWPB.EWPB_351:
                        var ewpb351 = Dictionaries.Find(y => y.Jednostka == x.Jednostka);
                        x.NazwaJednostki = ewpb351.NazwaJednostki;
                        x.Zaklad = ewpb351.Zaklad;
                        x.Sklad = ewpb351.Sklad;
                        break;
                }
            });
        }
        public void AddJim()
        {
            if (Zywnosc != null) Zywnosc.Clear(); else Zywnosc = new List<SigmatZywnosc>();
            if (Amunicja != null) Amunicja.Clear(); else Amunicja = new List<SigmatAmunicja>();
            if (Kat != null) Kat.Clear(); else Kat = new List<SigmatKat>();
            if (Paliwa != null) Paliwa.Clear(); else Paliwa = new List<SigmatPaliwa>();
            if (Mund != null) Mund.Clear(); else Mund = new List<SigmatMund>();

            Materialy.ForEach(x =>
            {
                switch (x.Klasyfikacja)
                {
                    case "ZYWNOSC":
                        // SIGMAT ZYWNOSC LEKARSTWA
                        SigmatZywnosc zywnosc = new SigmatZywnosc();
                        zywnosc.App = TypWydruku.ToString();
                        zywnosc.Lp = x.Lp;
                        zywnosc.Magazyn_ID = x.NrMagazynu;
                        zywnosc.Jim = x.Jim;
                        zywnosc.Material = x.Material;
                        zywnosc.Kategoria = x.Kategoria;
                        zywnosc.Ilosc = x.Ilosc;
                        zywnosc.Wartosc = x.Wartosc;
                        zywnosc.Zaklad = x.Zaklad;
                        zywnosc.Sklad = x.Sklad;
                        zywnosc.Uzytkownik_ID = x.UzytkownikZwsiron;

                        Zywnosc.Add(zywnosc);
                        break;

                    case "AMUNICJA":
                        // SIGMAT AMUNICJA
                        SigmatAmunicja amunicja = new SigmatAmunicja();
                        amunicja.App = TypWydruku.ToString();
                        amunicja.Lp = x.Lp;
                        amunicja.Magazyn_ID = x.NrMagazynu;
                        amunicja.Jim = x.Jim;
                        amunicja.Material = x.Material;
                        amunicja.Kategoria = x.Kategoria;
                        amunicja.NrSeryjny = x.NrSeryjny;
                        amunicja.Ilosc = x.Ilosc;
                        amunicja.Wartosc = x.Wartosc;
                        amunicja.Zaklad = x.Zaklad;
                        amunicja.Sklad = x.Sklad;
                        amunicja.Uzytkownik_ID = x.UzytkownikZwsiron;

                        Amunicja.Add(amunicja);
                        break;

                    case "KAT":
                        // SIGMAT KAT
                        SigmatKat kat = new SigmatKat();
                        kat.App = TypWydruku.ToString();
                        kat.Lp = x.Lp;
                        kat.Magazyn_ID = x.NrMagazynu;
                        kat.Jim = x.Jim;
                        kat.Material = x.Material;
                        kat.Kategoria = x.Kategoria;
                        kat.NrSeryjny = x.NrSeryjny;
                        kat.Ilosc = x.Ilosc;
                        kat.Wartosc = x.Wartosc;
                        kat.Zaklad = x.Zaklad;
                        kat.Sklad = x.Sklad;
                        kat.Uzytkownik_ID = x.UzytkownikZwsiron;

                        Kat.Add(kat);
                        break;

                    case "PALIWA":
                        // SIGMAT PALIWA
                        SigmatPaliwa paliwa = new SigmatPaliwa();
                        paliwa.App = TypWydruku.ToString();
                        paliwa.Lp = x.Lp;
                        paliwa.Magazyn_ID = x.NrMagazynu;
                        paliwa.Jim = x.Jim;
                        paliwa.Material = x.Material;
                        paliwa.Kategoria = x.Kategoria;
                        paliwa.Ilosc = x.Ilosc;
                        paliwa.Wartosc = x.Wartosc;
                        paliwa.Zaklad = x.Zaklad;
                        paliwa.Sklad = x.Sklad;
                        paliwa.Uzytkownik_ID = x.UzytkownikZwsiron;

                        Paliwa.Add(paliwa);
                        break;

                    case "MUND":
                        // SIGMAT MUNDUROWKA
                        SigmatMund mund = new SigmatMund();
                        mund.App = TypWydruku.ToString();
                        mund.Lp = x.Lp;
                        mund.Magazyn_ID = x.NrMagazynu;
                        mund.Jim = x.Jim;
                        mund.Material = x.Material;
                        mund.Kategoria = x.Kategoria;
                        mund.Ilosc = x.Ilosc;
                        mund.Wartosc = x.Wartosc;
                        mund.Zaklad = x.Zaklad;
                        mund.Sklad = x.Sklad;
                        mund.Uzytkownik_ID = x.UzytkownikZwsiron;

                        Mund.Add(mund);
                        break;

                    default:
                        break;
                }
            });

        }
        private void ZapiszZywnosc()
        {
            if (Zywnosc.Any())
            {
                var dlg = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = dlg.ShowDialog();

                var list_zyw = Zywnosc.GroupBy(x => new { x.Zaklad, x.Sklad })
                    .Select(x => x.ToList())
                    .ToList();

                foreach (var lista in list_zyw)
                {
                    string FileName = string.Format("{0}/{1}_{2}_ZYWNOSC.txt", dlg.SelectedPath, lista[0].Zaklad, lista[0].Sklad);

                    if (lista.Count > 499)
                    {
                        var temp_list = lista.Select((x, i) => new { Index = i, Value = x })
                            .GroupBy(x => x.Index / 499)
                            .Select(x => x.Select(v => v.Value).ToList())
                            .ToList();

                        for (int i = 1; i <= temp_list.Count; i++)
                        {
                            string FileName2 = string.Format("{0}/{1}_{2}_ZYWNOSC_{3}.txt", dlg.SelectedPath, lista[0].Zaklad, lista[0].Sklad, i);

                            try
                            {
                                using (StreamWriter writer = new StreamWriter(FileName2, false, enc))
                                {

                                    foreach (var zywnosc in temp_list[i - 1])
                                    {
                                        NullToStringEmptyConversion(zywnosc);
                                        if (TypWydruku == MagmatEWPB.EWPB_319_320)
                                            writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}", 
                                                zywnosc.Jim.PadRight(18).Substring(0,18), 
                                                zywnosc.Ilosc.ToString().PadRight(17).Substring(0,17),
                                                zywnosc.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                                zywnosc.DataWaznosci.ToString().PadRight(10).Substring(0, 10),
                                                zywnosc.NrPartiiProducenta.ToString().PadRight(15).Substring(0, 15),
                                                zywnosc.Opakowanie.ToString().PadRight(10).Substring(0, 10),
                                                zywnosc.Uzytkownik_ID.ToString().PadRight(10).Substring(0, 10),
                                                zywnosc.DataWydania.ToString().PadRight(10).Substring(0, 10),
                                                zywnosc.WyposazenieIndywidualne.ToString().PadRight(1).Substring(0, 1));
                                        else
                                            writer.WriteLine("{0}{1}{2}{3}{4}{5}",
                                                zywnosc.Jim.PadRight(18).Substring(0, 18),
                                                zywnosc.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                                zywnosc.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                                zywnosc.DataWaznosci.ToString().PadRight(10).Substring(0, 10),
                                                zywnosc.NrPartiiProducenta.ToString().PadRight(15).Substring(0, 15),
                                                zywnosc.Opakowanie.ToString().PadRight(10).Substring(0, 10));
                                    }
                                }
                            }
                            catch (IOException ex)
                            {
                                MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            using (StreamWriter writer = new StreamWriter(FileName, false, enc))
                            {

                                foreach (var zywnosc in lista)
                                {
                                    NullToStringEmptyConversion(zywnosc);
                                    if (TypWydruku == MagmatEWPB.EWPB_319_320)
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}",
                                            zywnosc.Jim.PadRight(18).Substring(0, 18),
                                            zywnosc.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                            zywnosc.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                            zywnosc.DataWaznosci.ToString().PadRight(10).Substring(0, 10),
                                            zywnosc.NrPartiiProducenta.ToString().PadRight(15).Substring(0, 15),
                                            zywnosc.Opakowanie.ToString().PadRight(10).Substring(0, 10),
                                            zywnosc.Uzytkownik_ID.ToString().PadRight(10).Substring(0, 10),
                                            zywnosc.DataWydania.ToString().PadRight(10).Substring(0, 10),
                                            zywnosc.WyposazenieIndywidualne.ToString().PadRight(1).Substring(0, 1));
                                    else
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}",
                                            zywnosc.Jim.PadRight(18).Substring(0, 18),
                                            zywnosc.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                            zywnosc.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                            zywnosc.DataWaznosci.ToString().PadRight(10).Substring(0, 10),
                                            zywnosc.NrPartiiProducenta.ToString().PadRight(15).Substring(0, 15),
                                            zywnosc.Opakowanie.ToString().PadRight(10).Substring(0, 10));
                                }
                            }
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Brak danych!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ZapiszAmunicja()
        {
            if (Amunicja.Any())
            {
                var dlg = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = dlg.ShowDialog();

                var lista_list = Amunicja.GroupBy(x => new { x.Zaklad, x.Sklad })
                    .Select(x => x.ToList())
                    .ToList();

                foreach (var lista in lista_list)
                {
                    string FileName = string.Format("{0}/{1}_{2}_AMUNICJA.txt", dlg.SelectedPath, lista[0].Zaklad, lista[0].Sklad);

                    if (lista.Count > 499)
                    {
                        var temp_list = lista.Select((x, i) => new { Index = i, Value = x })
                            .GroupBy(x => x.Index / 499)
                            .Select(x => x.Select(v => v.Value).ToList())
                            .ToList();

                        for (int i = 1; i <= temp_list.Count; i++)
                        {
                            string FileName2 = string.Format("{0}/{1}_{2}_AMUNICJA_{3}.txt", dlg.SelectedPath, lista[0].Zaklad, lista[0].Sklad, i);

                            try
                            {
                                using (StreamWriter writer = new StreamWriter(FileName2, false, enc))
                                {

                                    foreach (var amunicja in temp_list[i - 1])
                                    {
                                        NullToStringEmptyConversion(amunicja);
                                        if (TypWydruku == MagmatEWPB.EWPB_319_320)
                                            writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}",
                                                amunicja.Jim.PadRight(18).Substring(0, 18),
                                                amunicja.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                                amunicja.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                                amunicja.Kategoria.ToString().PadRight(1).Substring(0, 1),
                                                amunicja.Kategoria2.ToString().PadRight(1).Substring(0, 1),
                                                amunicja.NrPartii.ToString().PadRight(17).Substring(0, 17),
                                                amunicja.DataProdukcji.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.DataZablokowania.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.Wyroznik.ToString().PadRight(1).Substring(0, 1),
                                                amunicja.DataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.ZnacznikBlokowania.ToString().PadRight(1).Substring(0, 1),
                                                amunicja.NrSeryjny.ToString().PadRight(5).Substring(0, 5),
                                                amunicja.Uzytkownik_ID.ToString().PadRight(10).Substring(0, 10),
                                                amunicja.DataWydania.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.WyposazenieIndywidualne.ToString().PadRight(1).Substring(0, 1));
                                        else
                                            writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}{23}{24}{25}{26}{27}{28}{29}{30}",
                                                amunicja.Jim.PadRight(18).Substring(0, 18),
                                                amunicja.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                                amunicja.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                                amunicja.Kategoria.ToString().PadRight(1).Substring(0, 1),
                                                amunicja.Kategoria2.ToString().PadRight(1).Substring(0, 1),
                                                amunicja.NrPartii.ToString().PadRight(17).Substring(0, 17),
                                                amunicja.DataProdukcji.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.DataZablokowania.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.Wyroznik.ToString().PadRight(1).Substring(0, 1),
                                                amunicja.DataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.ZnacznikBlokowania.ToString().PadRight(1).Substring(0, 1),
                                                amunicja.NrSeryjny.ToString().PadRight(5).Substring(0, 5),
                                                amunicja.DataGwarancjiJBR.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.Zapalnik.ToString().PadRight(17).Substring(0, 17),
                                                amunicja.ZapalnikDataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.ZapalnikDataGwarancjiJBR.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.Zaplonnik.ToString().PadRight(17).Substring(0, 17),
                                                amunicja.ZaplonnikDataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.ZaplonnikDataGwarancjiJBR.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.Ladunek.ToString().PadRight(17).Substring(0, 17),
                                                amunicja.LadunekDataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.LadunekDataGwarancjiJBR.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.Pocisk.ToString().PadRight(17).Substring(0, 17),
                                                amunicja.PociskDataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.PociskDataGwarancjiJBR.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.Zrodlo.ToString().PadRight(17).Substring(0, 17),
                                                amunicja.ZrodloDataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.ZrodloDataGwarancjiJBR.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.Smugacz.ToString().PadRight(17).Substring(0, 17),
                                                amunicja.SmugaczDataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                                amunicja.SmugaczDataGwarancjiJBR.ToString().PadRight(8).Substring(0, 8));
                                    }
                                }
                            }
                            catch (IOException ex)
                            {
                                MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else
                    {
                        try
                        {

                            using (StreamWriter writer = new StreamWriter(FileName, false, enc))
                            {

                                foreach (var amunicja in lista)
                                {
                                    NullToStringEmptyConversion(amunicja);
                                    if (TypWydruku == MagmatEWPB.EWPB_319_320)
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}",
                                            amunicja.Jim.PadRight(18).Substring(0, 18),
                                            amunicja.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                            amunicja.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                            amunicja.Kategoria.ToString().PadRight(1).Substring(0, 1),
                                            amunicja.Kategoria2.ToString().PadRight(1).Substring(0, 1),
                                            amunicja.NrPartii.ToString().PadRight(17).Substring(0, 17),
                                            amunicja.DataProdukcji.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.DataZablokowania.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.Wyroznik.ToString().PadRight(1).Substring(0, 1),
                                            amunicja.DataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.ZnacznikBlokowania.ToString().PadRight(1).Substring(0, 1),
                                            amunicja.NrSeryjny.ToString().PadRight(5).Substring(0, 5),
                                            amunicja.Uzytkownik_ID.ToString().PadRight(10).Substring(0, 10),
                                            amunicja.DataWydania.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.WyposazenieIndywidualne.ToString().PadRight(1).Substring(0, 1));
                                    else
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}{23}{24}{25}{26}{27}{28}{29}{30}",
                                            amunicja.Jim.PadRight(18).Substring(0, 18),
                                            amunicja.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                            amunicja.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                            amunicja.Kategoria.ToString().PadRight(1).Substring(0, 1),
                                            amunicja.Kategoria2.ToString().PadRight(1).Substring(0, 1),
                                            amunicja.NrPartii.ToString().PadRight(17).Substring(0, 17),
                                            amunicja.DataProdukcji.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.DataZablokowania.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.Wyroznik.ToString().PadRight(1).Substring(0, 1),
                                            amunicja.DataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.ZnacznikBlokowania.ToString().PadRight(1).Substring(0, 1),
                                            amunicja.NrSeryjny.ToString().PadRight(5).Substring(0, 5),
                                            amunicja.DataGwarancjiJBR.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.Zapalnik.ToString().PadRight(17).Substring(0, 17),
                                            amunicja.ZapalnikDataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.ZapalnikDataGwarancjiJBR.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.Zaplonnik.ToString().PadRight(17).Substring(0, 17),
                                            amunicja.ZaplonnikDataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.ZaplonnikDataGwarancjiJBR.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.Ladunek.ToString().PadRight(17).Substring(0, 17),
                                            amunicja.LadunekDataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.LadunekDataGwarancjiJBR.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.Pocisk.ToString().PadRight(17).Substring(0, 17),
                                            amunicja.PociskDataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.PociskDataGwarancjiJBR.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.Zrodlo.ToString().PadRight(17).Substring(0, 17),
                                            amunicja.ZrodloDataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.ZrodloDataGwarancjiJBR.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.Smugacz.ToString().PadRight(17).Substring(0, 17),
                                            amunicja.SmugaczDataGwarancji.ToString().PadRight(8).Substring(0, 8),
                                            amunicja.SmugaczDataGwarancjiJBR.ToString().PadRight(8).Substring(0, 8));
                                }
                            }
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Brak danych!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ZapiszKat()
        {
            if (Kat.Any())
            {
                var dlg = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = dlg.ShowDialog();

                var lista_list = Kat.GroupBy(x => new { x.Zaklad, x.Sklad })
                    .Select(x => x.ToList())
                    .ToList();

                foreach (var lista in lista_list)
                {
                    string FileName = string.Format("{0}/{1}_{2}_KAT.txt", dlg.SelectedPath, lista[0].Zaklad, lista[0].Sklad);

                    if (lista.Count > 499)
                    {
                        var temp_list = lista.Select((x, i) => new { Index = i, Value = x })
                            .GroupBy(x => x.Index / 499)
                            .Select(x => x.Select(v => v.Value).ToList())
                            .ToList();

                        for (int i = 1; i <= temp_list.Count; i++)
                        {
                            string FileName2 = string.Format("{0}/{1}_{2}_KAT_{3}.txt", dlg.SelectedPath, lista[0].Zaklad, lista[0].Sklad, i);
                            try
                            {
                                using (StreamWriter writer = new StreamWriter(FileName2, false, enc))
                                {

                                    foreach (var kat in temp_list[i - 1])
                                    {
                                        NullToStringEmptyConversion(kat);
                                        if (TypWydruku == MagmatEWPB.EWPB_319_320)
                                            writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}",
                                                kat.Jim.PadRight(18).Substring(0, 18),
                                                kat.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                                kat.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                                kat.Kategoria.ToString().PadRight(1).Substring(0, 1),
                                                kat.NrSeryjny.ToString().PadRight(15).Substring(0, 15),
                                                kat.DataNabycia.ToString().PadRight(10).Substring(0, 10),
                                                kat.WartoscPoczatkowa.ToString().PadRight(11).Substring(0, 11),
                                                kat.WartoscUmorzenia.ToString().PadRight(11).Substring(0, 11),
                                                kat.StawkaAmortyzacji.ToString().PadRight(4).Substring(0, 4),
                                                kat.KlasSrodkowTrwalych.ToString().PadRight(3).Substring(0, 3),
                                                kat.DataProdukcji.ToString().PadRight(10).Substring(0, 10),
                                                kat.DataGwarancji.ToString().PadRight(10).Substring(0, 10),
                                                kat.Uzytkownik_ID.ToString().PadRight(10).Substring(0, 10),
                                                kat.DataWydania.ToString().PadRight(10).Substring(0, 10),
                                                kat.WyposazenieIndywidualne.ToString().PadRight(1).Substring(0, 1),
                                                kat.Pododdzial.ToString().PadRight(20).Substring(0, 20),
                                                kat.NrSeryjny.ToString().PadRight(20).Substring(0, 20));
                                        else
                                            writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", 
                                                kat.Jim.PadRight(18).Substring(0,18),
                                                kat.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                                kat.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                                kat.Kategoria.ToString().PadRight(1).Substring(0, 1),
                                                kat.NrSeryjny.ToString().PadRight(15).Substring(0, 15),
                                                kat.DataNabycia.ToString().PadRight(10).Substring(0, 10),
                                                kat.WartoscPoczatkowa.ToString().PadRight(11).Substring(0, 11),
                                                kat.WartoscUmorzenia.ToString().PadRight(11).Substring(0, 11),
                                                kat.StawkaAmortyzacji.ToString().PadRight(4).Substring(0, 4),
                                                kat.KlasSrodkowTrwalych.ToString().PadRight(3).Substring(0, 3),
                                                kat.DataProdukcji.ToString().PadRight(10).Substring(0, 10),
                                                kat.DataGwarancji.ToString().PadRight(10).Substring(0, 10),
                                                kat.KodStan.ToString().PadRight(1).Substring(0, 1));
                                    }
                                }
                            }
                            catch (IOException ex)
                            {
                                MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            using (StreamWriter writer = new StreamWriter(FileName, false, enc))
                            {

                                foreach (var kat in lista)
                                {
                                    NullToStringEmptyConversion(kat);
                                    if (TypWydruku == MagmatEWPB.EWPB_319_320)
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}",
                                            kat.Jim.PadRight(18).Substring(0, 18),
                                            kat.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                            kat.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                            kat.Kategoria.ToString().PadRight(1).Substring(0, 1),
                                            kat.NrSeryjny.ToString().PadRight(15).Substring(0, 15),
                                            kat.DataNabycia.ToString().PadRight(10).Substring(0, 10),
                                            kat.WartoscPoczatkowa.ToString().PadRight(11).Substring(0, 11),
                                            kat.WartoscUmorzenia.ToString().PadRight(11).Substring(0, 11),
                                            kat.StawkaAmortyzacji.ToString().PadRight(4).Substring(0, 4),
                                            kat.KlasSrodkowTrwalych.ToString().PadRight(3).Substring(0, 3),
                                            kat.DataProdukcji.ToString().PadRight(10).Substring(0, 10),
                                            kat.DataGwarancji.ToString().PadRight(10).Substring(0, 10),
                                            kat.Uzytkownik_ID.ToString().PadRight(10).Substring(0, 10),
                                            kat.DataWydania.ToString().PadRight(10).Substring(0, 10),
                                            kat.WyposazenieIndywidualne.ToString().PadRight(1).Substring(0, 1),
                                            kat.Pododdzial.ToString().PadRight(20).Substring(0, 20),
                                            kat.NrSeryjny.ToString().PadRight(20).Substring(0, 20));
                                    else
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}",
                                            kat.Jim.PadRight(18).Substring(0, 18),
                                            kat.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                            kat.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                            kat.Kategoria.ToString().PadRight(1).Substring(0, 1),
                                            kat.NrSeryjny.ToString().PadRight(15).Substring(0, 15),
                                            kat.DataNabycia.ToString().PadRight(10).Substring(0, 10),
                                            kat.WartoscPoczatkowa.ToString().PadRight(11).Substring(0, 11),
                                            kat.WartoscUmorzenia.ToString().PadRight(11).Substring(0, 11),
                                            kat.StawkaAmortyzacji.ToString().PadRight(4).Substring(0, 4),
                                            kat.KlasSrodkowTrwalych.ToString().PadRight(3).Substring(0, 3),
                                            kat.DataProdukcji.ToString().PadRight(10).Substring(0, 10),
                                            kat.DataGwarancji.ToString().PadRight(10).Substring(0, 10),
                                            kat.KodStan.ToString().PadRight(1).Substring(0, 1));
                                }
                            }
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Brak danych!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ZapiszPaliwa()
        {
            if (Paliwa.Any())
            {
                var dlg = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = dlg.ShowDialog();

                var lista_list = Paliwa.GroupBy(x => new { x.Zaklad, x.Sklad })
                    .Select(x => x.ToList())
                    .ToList();

                foreach (var lista in lista_list)
                {
                    string FileName = string.Format("{0}/{1}_{2}_PALIWA.txt", dlg.SelectedPath, lista[0].Zaklad, lista[0].Sklad);

                    if (lista.Count > 499)
                    {
                        var temp_list = lista.Select((x, i) => new { Index = i, Value = x })
                            .GroupBy(x => x.Index / 499)
                            .Select(x => x.Select(v => v.Value).ToList())
                            .ToList();

                        for (int i = 1; i <= temp_list.Count; i++)
                        {
                            string FileName2 = string.Format("{0}/{1}_{2}_PALIWA_{3}.txt", dlg.SelectedPath, lista[0].Zaklad, lista[0].Sklad, i);

                            try
                            {
                                using (StreamWriter writer = new StreamWriter(FileName2, false, enc))
                                {

                                    foreach (var pal in temp_list[i - 1])
                                    {
                                        NullToStringEmptyConversion(pal);
                                        if (TypWydruku == MagmatEWPB.EWPB_319_320)
                                            writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}",
                                                pal.Jim.PadRight(18).Substring(0, 18),
                                                pal.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                                pal.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                                pal.DataProdukcji.ToString().PadRight(10).Substring(0, 10),
                                                pal.TypOpakowania.ToString().PadRight(10).Substring(0, 10),
                                                pal.Wycena.ToString().PadRight(1).Substring(0, 1),
                                                pal.Orzeczenie.ToString().PadRight(30).Substring(0, 30),
                                                pal.Uzytkownik_ID.ToString().PadRight(10).Substring(0, 10),
                                                pal.DataWydania.ToString().PadRight(10).Substring(0, 10),
                                                pal.WyposazenieIndywidualne.ToString().PadRight(1).Substring(0, 1));
                                        else
                                            writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}",
                                                pal.Jim.PadRight(18).Substring(0, 18),
                                                pal.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                                pal.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                                pal.DataProdukcji.ToString().PadRight(10).Substring(0, 10),
                                                pal.TypOpakowania.ToString().PadRight(10).Substring(0, 10),
                                                pal.Wycena.ToString().PadRight(1).Substring(0, 1),
                                                pal.Orzeczenie.ToString().PadRight(30).Substring(0, 30));
                                    }
                                }
                            }
                            catch (IOException ex)
                            {
                                MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            using (StreamWriter writer = new StreamWriter(FileName, false, enc))
                            {

                                foreach (var pal in lista)
                                {
                                    NullToStringEmptyConversion(pal);
                                    if (TypWydruku == MagmatEWPB.EWPB_319_320)
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}",
                                            pal.Jim.PadRight(18).Substring(0, 18),
                                            pal.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                            pal.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                            pal.DataProdukcji.ToString().PadRight(10).Substring(0, 10),
                                            pal.TypOpakowania.ToString().PadRight(10).Substring(0, 10),
                                            pal.Wycena.ToString().PadRight(1).Substring(0, 1),
                                            pal.Orzeczenie.ToString().PadRight(30).Substring(0, 30),
                                            pal.Uzytkownik_ID.ToString().PadRight(10).Substring(0, 10),
                                            pal.DataWydania.ToString().PadRight(10).Substring(0, 10),
                                            pal.WyposazenieIndywidualne.ToString().PadRight(1).Substring(0, 1));
                                    else
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}",
                                            pal.Jim.PadRight(18).Substring(0, 18),
                                            pal.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                            pal.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                            pal.DataProdukcji.ToString().PadRight(10).Substring(0, 10),
                                            pal.TypOpakowania.ToString().PadRight(10).Substring(0, 10),
                                            pal.Wycena.ToString().PadRight(1).Substring(0, 1),
                                            pal.Orzeczenie.ToString().PadRight(30).Substring(0, 30));
                                }
                            }
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Brak danych!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ZapiszMund()
        {
            if (Mund.Any())
            {
                var dlg = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = dlg.ShowDialog();

                var lista_list = Mund.GroupBy(x => new { x.Zaklad, x.Sklad })
                    .Select(x => x.ToList())
                    .ToList();

                foreach (var lista in lista_list)
                {
                    string FileName = string.Format("{0}/{1}_{2}_MUND.txt", dlg.SelectedPath, lista[0].Zaklad, lista[0].Sklad);

                    if (lista.Count > 499)
                    {
                        var temp_list = lista.Select((x, i) => new { Index = i, Value = x })
                            .GroupBy(x => x.Index / 499)
                            .Select(x => x.Select(v => v.Value).ToList())
                            .ToList();

                        for (int i = 1; i <= temp_list.Count; i++)
                        {
                            string FileName2 = string.Format("{0}/{1}_{2}_MUND_{3}.txt", dlg.SelectedPath, lista[0].Zaklad, lista[0].Sklad, i);
                            try
                            {
                                using (StreamWriter writer = new StreamWriter(FileName2, false, enc))
                                {
                                    foreach (var mund in temp_list[i - 1])
                                    {
                                        NullToStringEmptyConversion(mund);
                                        if (TypWydruku == MagmatEWPB.EWPB_319_320)
                                            writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}",
                                                mund.Jim.PadRight(18).Substring(0, 18),
                                                mund.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                                mund.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                                mund.Kategoria.ToString().PadRight(1).Substring(0, 1),
                                                mund.Rozmiar.ToString().PadRight(11).Substring(0, 11),
                                                mund.RokProdukcji.ToString().PadRight(4).Substring(0, 4),
                                                mund.RokGwarancji.ToString().PadRight(4).Substring(0, 4),
                                                mund.Uzytkownik_ID.ToString().PadRight(10).Substring(0, 10),
                                                mund.DataWydania.ToString().PadRight(10).Substring(0, 10),
                                                mund.WyposazenieInduwidualne.ToString().PadRight(1).Substring(0, 1),
                                                mund.Pododdzial.ToString().PadRight(20).Substring(0, 20),
                                                mund.OkresUzywalnosci.ToString().PadRight(3).Substring(0, 3),
                                                mund.TypPozycjiPodzestawu.ToString().PadRight(1).Substring(0, 1));
                                        else
                                            writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}",
                                                mund.Jim.PadRight(18).Substring(0, 18),
                                                mund.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                                mund.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                                mund.Kategoria.ToString().PadRight(1).Substring(0, 1),
                                                mund.Rozmiar.ToString().PadRight(11).Substring(0, 11),
                                                mund.RokProdukcji.ToString().PadRight(4).Substring(0, 4),
                                                mund.RokGwarancji.ToString().PadRight(4).Substring(0, 4));
                                    }
                                }
                            }
                            catch (IOException ex)
                            {
                                MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            using (StreamWriter writer = new StreamWriter(FileName, false, enc))
                            {


                                foreach (var mund in lista)
                                {
                                    NullToStringEmptyConversion(mund);
                                    if (TypWydruku == MagmatEWPB.EWPB_319_320)
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}",
                                            mund.Jim.PadRight(18).Substring(0, 18),
                                            mund.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                            mund.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                            mund.Kategoria.ToString().PadRight(1).Substring(0, 1),
                                            mund.Rozmiar.ToString().PadRight(11).Substring(0, 11),
                                            mund.RokProdukcji.ToString().PadRight(4).Substring(0, 4),
                                            mund.RokGwarancji.ToString().PadRight(4).Substring(0, 4),
                                            mund.Uzytkownik_ID.ToString().PadRight(10).Substring(0, 10),
                                            mund.DataWydania.ToString().PadRight(10).Substring(0, 10),
                                            mund.WyposazenieInduwidualne.ToString().PadRight(1).Substring(0, 1),
                                            mund.Pododdzial.ToString().PadRight(20).Substring(0, 20),
                                            mund.OkresUzywalnosci.ToString().PadRight(3).Substring(0, 3),
                                            mund.TypPozycjiPodzestawu.ToString().PadRight(1).Substring(0, 1));
                                    else
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}",
                                            mund.Jim.PadRight(18).Substring(0, 18),
                                            mund.Ilosc.ToString().PadRight(17).Substring(0, 17),
                                            mund.Wartosc.ToString().PadRight(16).Substring(0, 16),
                                            mund.Kategoria.ToString().PadRight(1).Substring(0, 1),
                                            mund.Rozmiar.ToString().PadRight(11).Substring(0, 11),
                                            mund.RokProdukcji.ToString().PadRight(4).Substring(0, 4),
                                            mund.RokGwarancji.ToString().PadRight(4).Substring(0, 4));
                                }
                            }
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Brak danych!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void NullToStringEmptyConversion(object myObject)
        {
            foreach (var propertyInfo in myObject.GetType().GetProperties())
            {
                if (propertyInfo.PropertyType == typeof(string))
                {
                    if (propertyInfo.GetValue(myObject, null) == null)
                    {
                        propertyInfo.SetValue(myObject, string.Empty, null);
                    }
                }
            }
        }
        #endregion

        public void SaveFile(string header)
        {
            switch (header)
            {
                case "ŻYWNOŚC (LEKARSTWA)":
                    ZapiszZywnosc();
                    break;
                case "AMUNICJA":
                    ZapiszAmunicja();
                    break;
                case "KAT":
                    ZapiszKat();
                    break;
                case "PALIWA":
                    ZapiszPaliwa();
                    break;
                case "MUND":
                    ZapiszMund();
                    break;
            }
        }

        public void Clean()
        {
            Materialy = null;
            Dictionaries = null;
            Kat = null;
            Amunicja = null;
            Mund = null;
            Paliwa = null;
            Zywnosc = null;
        }
    }
}
