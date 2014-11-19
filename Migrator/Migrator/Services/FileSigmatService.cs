using Migrator.Helpers;
using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.Services
{
    public class FileSigmatService : IFileSigmatService
    {
        List<MagmatEwpb> _listMaterialy = new List<MagmatEwpb>();
        List<SigmatKat> _listKat = new List<SigmatKat>();
        List<SigmatAmunicja> _listAmunicja = new List<SigmatAmunicja>();
        List<SigmatMund> _listMund = new List<SigmatMund>();
        List<SigmatPaliwa> _listPaliwa = new List<SigmatPaliwa>();
        List<SigmatZywnosc> _listZywnosc = new List<SigmatZywnosc>();

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
                    case MagmatEWPB.Magmat305:
                        var mag = listSelMaterialy.Find(y => y.NrMagazynu == x.NrMagazynu);
                        x.NazwaMagazynu = mag.NazwaMagazynu;
                        x.Zaklad = mag.Zaklad;
                        x.Sklad = mag.Sklad;
                        break;
                    case MagmatEWPB.Ewpb319_320:
                        var ewpb319 = listSelMaterialy.Find(y => y.Uzytkownik == x.Uzytkownik);
                        x.UzytkownikZwsiron = ewpb319.UzytkownikZwsiron;
                        x.NazwaUzytkownika = ewpb319.NazwaUzytkownika;
                        x.Zaklad = ewpb319.Zaklad;
                        x.Sklad = ewpb319.Sklad;
                        break;
                    case MagmatEWPB.EWpb351:
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
