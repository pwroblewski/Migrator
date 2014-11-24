using Migrator.Helpers;
using Migrator.Model;
using Migrator.Model.State;
using Migrator.Services.MAGMAT_EWPB;
using Migrator.Services.SRTR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.Services
{
    public class MAG_EWPBService : IMAG_EWPBService
    {
        private MagmatEwpbState stan = new MagmatEwpbState();

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
            else if (path.Contains("319"))
                TypWydruku = MagmatEWPB.EWPB_319_320;
            else
                TypWydruku = MagmatEWPB.EWPB_351;
        }
        #endregion

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
            Zywnosc = new List<SigmatZywnosc>();
            Amunicja = new List<SigmatAmunicja>();
            Kat = new List<SigmatKat>();
            Paliwa = new List<SigmatPaliwa>();
            Mund = new List<SigmatMund>();

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

        public string SaveFile()
        {
            throw new NotImplementedException();
        }

        public void Clean()
        {
            Materialy.Clear();
            Dictionaries.Clear();
            Kat.Clear();
            Amunicja.Clear();
            Mund.Clear();
            Paliwa.Clear();
            Zywnosc.Clear();
        }
    }
}
