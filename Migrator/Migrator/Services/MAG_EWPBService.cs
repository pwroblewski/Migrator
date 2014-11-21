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

        List<MagmatEwpb> _listMaterialy = new List<MagmatEwpb>();
        List<SigmatKat> _listKat = new List<SigmatKat>();
        List<SigmatAmunicja> _listAmunicja = new List<SigmatAmunicja>();
        List<SigmatMund> _listMund = new List<SigmatMund>();
        List<SigmatPaliwa> _listPaliwa = new List<SigmatPaliwa>();
        List<SigmatZywnosc> _listZywnosc = new List<SigmatZywnosc>();

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

        #region Helpers

        #endregion
        public string SaveFile()
        {
            throw new NotImplementedException();
        }

        public void AddMaterial(List<MagmatEwpb> listMaterialy)
        {
            Clean();

            _listMaterialy.AddRange(listMaterialy);
        }

        public void AddSlownik(List<MagmatEwpb> listSelMaterialy, MagmatEWPB typWydruku)
        {
            _listMaterialy.ForEach(x =>
            {
                switch (typWydruku)
                {
                    case MagmatEWPB.Magmat_305:
                        var mag = listSelMaterialy.Find(y => y.NrMagazynu == x.NrMagazynu);
                        x.NazwaMagazynu = mag.NazwaMagazynu;
                        x.Zaklad = mag.Zaklad;
                        x.Sklad = mag.Sklad;
                        break;
                    case MagmatEWPB.EWPB_319_320:
                        var ewpb319 = listSelMaterialy.Find(y => y.Uzytkownik == x.Uzytkownik);
                        x.UzytkownikZwsiron = ewpb319.UzytkownikZwsiron;
                        x.NazwaUzytkownika = ewpb319.NazwaUzytkownika;
                        x.Zaklad = ewpb319.Zaklad;
                        x.Sklad = ewpb319.Sklad;
                        break;
                    case MagmatEWPB.EWPB_351:
                        var ewpb351 = listSelMaterialy.Find(y => y.Jednostka == x.Jednostka);
                        x.NazwaJednostki = ewpb351.NazwaJednostki;
                        x.Zaklad = ewpb351.Zaklad;
                        x.Sklad = ewpb351.Sklad;
                        break;
                }
            });
        }

        public void AddJim(List<MagmatEwpb> listMaterialy, MagmatEWPB typWydruku)
        {
            listMaterialy.ForEach(x =>
            {
                switch (x.Klasyfikacja)
                {
                    case "ZYWNOSC":
                        // SIGMAT ZYWNOSC LEKARSTWA
                        SigmatZywnosc zywnosc = new SigmatZywnosc();
                        zywnosc.App = typWydruku.ToString();
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

                        _listZywnosc.Add(zywnosc);
                        break;

                    case "AMUNICJA":
                        // SIGMAT AMUNICJA
                        SigmatAmunicja amunicja = new SigmatAmunicja();
                        amunicja.App = typWydruku.ToString();
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

                        _listAmunicja.Add(amunicja);
                        break;

                    case "KAT":
                        // SIGMAT KAT
                        SigmatKat kat = new SigmatKat();
                        kat.App = typWydruku.ToString();
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

                        _listKat.Add(kat);
                        break;

                    case "PALIWA":
                        // SIGMAT PALIWA
                        SigmatPaliwa paliwa = new SigmatPaliwa();
                        paliwa.App = typWydruku.ToString();
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

                        _listPaliwa.Add(paliwa);
                        break;

                    case "MUND":
                        // SIGMAT MUNDUROWKA
                        SigmatMund mund = new SigmatMund();
                        mund.App = typWydruku.ToString();
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

                        _listMund.Add(mund);
                        break;
                    default:
                        break;
                }
            });

        }

        public List<Sigmat> GetSigmat()
        {
            throw new NotImplementedException();
        }

        public List<MagmatEwpb> GetMaterialy()
        {
            return _listMaterialy;
        }

        public List<SigmatKat> GetKat()
        {
            return _listKat;
        }

        public List<SigmatAmunicja> GetAmunicja()
        {
            return _listAmunicja;
        }

        public List<SigmatMund> GetMund()
        {
            return _listMund;
        }

        public List<SigmatPaliwa> GetPaliwa()
        {
            return _listPaliwa;
        }

        public List<SigmatZywnosc> GetZywnosc()
        {
            return _listZywnosc;
        }

        public void Clean()
        {
            _listMaterialy.Clear();
            _listKat.Clear();
            _listAmunicja.Clear();
            _listMund.Clear();
            _listPaliwa.Clear();
            _listZywnosc.Clear();
        }


        public void SetKat(List<SigmatKat> listKat)
        {
            _listKat = listKat;
        }

        public void SetAmunicja(List<SigmatAmunicja> listAmunicja)
        {
            _listAmunicja = listAmunicja;
        }

        public void SetMund(List<SigmatMund> listMund)
        {
            _listMund = listMund;
        }

        public void SetPaliwa(List<SigmatPaliwa> listPaliwa)
        {
            _listPaliwa = listPaliwa;
        }

        public void SetZywnosc(List<SigmatZywnosc> listZywnosc)
        {
            _listZywnosc = listZywnosc;
        }
    }
}
