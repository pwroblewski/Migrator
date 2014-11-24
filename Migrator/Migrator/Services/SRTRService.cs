using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using Migrator.Helpers;
using System.Linq;
using Migrator.Model.State;
using Migrator.Services.SRTR;

namespace Migrator.Services
{
    public class SRTRService : ISRTRService
    {
        private SrtrState stan = new SrtrState();

        #region Properties
        public SrtrState SrtrState
        {
            get { return stan; }
            set { stan = value; }
        }
        public string ViewName
        {
            get { return stan.ViewName; }
            set { stan.ViewName = value; }
        }
        public List<SrtrToZwsiron> SrtrToZwsiron
        {
            get { return stan.ListWynik; }
            set { stan.ListWynik = value; }
        }
        public List<KartotekaSRTR> Kartoteka
        {
            get { return stan.ListKartoteka; }
            set { stan.ListKartoteka = value; }
        }
        public List<KartotekaSRTR> KartotekaZlik
        {
            get { return stan.ListKartotekaZlik; }
            set { stan.ListKartotekaZlik = value; }
        }
        public List<Uzytkownik> Users
        {
            get { return stan.ListUzytkownik; }
            set { stan.ListUzytkownik = value; }
        }
        public List<GrupaRodzajowaGusSRTR> GrGus
        {
            get { return stan.ListGrupaGus; }
            set { stan.ListGrupaGus = value; }
        }
        public List<WykazIlosciowy> Wykaz
        {
            get { return stan.ListWykazIlosciowy; }
            set { stan.ListWykazIlosciowy = value; }
        }
        #endregion

        #region Kartoteka
        public string OpenKartotekaFile()
        {
            return SRTR_Kartoteka.OpenFileDialog();
        }
        public void LoadKartotekaData(string path)
        {
            var ret = SRTR_Kartoteka.LoadData(path);
            Kartoteka = ret[0];
            KartotekaZlik = ret[1];
        }
        #endregion
        #region Uzytkownicy
        public string OpenUserFile()
        {
            return SRTR_Users.OpenFileDialog();
        }
        public void LoadUserData(string path)
        {
            var slownik = SRTR_Users.LoadData(path);
            SynchronizujDaneUzytkownika(slownik);
        }
        private void SynchronizujDaneUzytkownika(List<Uzytkownik> slownik)
        {
            Users.ForEach(x => 
                {
                    slownik.ForEach(y =>
                        {
                            if (x.IdSrtr.Equals(y.IdSrtr))
                            {
                                //x.IdZwsiron = y.IdZwsiron;
                                //x.Mpk = y.Mpk;
                                x.KodJednostki = y.KodJednostki;
                                x.NazwaUzytkownika = y.NazwaUzytkownika;
                                x.OsobaUpowazniona = y.OsobaUpowazniona;
                                x.Poczta = y.Poczta;
                                x.Telefax = y.Telefax;
                                x.Telefon = y.Telefon;
                                x.TypUzytkownika = y.TypUzytkownika;
                                x.Ulica = y.Ulica;
                            }
                        });
                });
        }
        #endregion
        #region GrGus
        public string OpenGrGusFile()
        {
            return SRTR_GrGus.OpenFileDialog();
        }
        public void LoadGrGusData(string path)
        {
            var slownik = SRTR_GrGus.LoadData(path);
            SynchronizujDaneGus(slownik);
        }
        private void SynchronizujDaneGus(List<GrupaRodzajowaGusSRTR> slownik)
        {
            GrGus.ForEach(x =>
            {
                slownik.ForEach(y =>
                {
                    if (x.KodGrRodzSRTR.Equals(y.KodGrRodzSRTR))
                    {
                        x.NazwaGrRodzSRTR = y.NazwaGrRodzSRTR;
                    }
                });
            });
        }
        #endregion
        #region WykazIlosciowy
        public string OpenWykazFile()
        {
            return SRTR_WykazIlosciowy.OpenFileDialog();
        }
        public void LoadWykazData(string path)
        {
            Wykaz = SRTR_WykazIlosciowy.LoadData(path);
        }
        #endregion
        #region Jim
        public string OpenJimFile()
        {
            return SRTR_Jim.OpenFileDialog();
        }
        public void AddJimData(string fileJimPath)
        {
            var wykaz = SRTR_Jim.AddJimData(fileJimPath, Wykaz);
            Wykaz = wykaz;
        }
        public string SaveJimFile()
        {
            return SRTR_Jim.SaveFileDialog(Wykaz);
        }
        #endregion

        #region Uzupelnianie
        public void AddKartotekaFile(List<KartotekaSRTR> listKartoteka)
        {
            SrtrToZwsiron = new List<SrtrToZwsiron>();

            foreach (var kartoteka in listKartoteka)
            {
                SrtrToZwsiron.Add(new SrtrToZwsiron
                {
                    Nazwa = kartoteka.Nazwa_sr,
                    Jeden = "1",
                    KatSprzetu = kartoteka.Kat_sprz.Trim(),
                    NrSeryjny = kartoteka.Nr_seryjny.Trim(),
                    NrInwentarzowy = String.Format("{0}\\{1}\\{2}", kartoteka.Nr_kolejny.Trim().PadLeft(6, '0'), kartoteka.Kod_jed.Trim().PadLeft(4, '0'), kartoteka.Gr_gus.Trim()),
                    DataNabycia2 = Convert.ToDateTime(kartoteka.Data_nab2).ToString("ddMMyyyy").PadLeft(8, '0'),
                    DataNabycia = Convert.ToDateTime(kartoteka.Data_nab).ToString("ddMMyyyy").PadLeft(8, '0'),
                    IdUzytSrtr = kartoteka.Kod_uzyt,
                    Zero = "0",
                    Kwo = UzupelnijKWO(kartoteka.Indeks_m),
                    // helpers
                    GrupaAktywowSRTR = kartoteka.Konto_wpc.Substring(0, 7),
                    JednsotkaMiarySRTR = kartoteka.Jed_miary,
                    WspolczynnikAmortyzacjiSRTR = kartoteka.Wsp_am_1,
                    WartoscPoczatkowaSRTR = kartoteka.War_pocz,
                    GrupaGusSRTR = kartoteka.Gr_gus
                });
            }
        }

        public void AddJednosktaGospodarcza(string jednostka)
        {
            SrtrToZwsiron.ForEach(x => x.JednostkaGospodarcza = jednostka);
        }

        public void AddJednostkiMiary(List<JednostkaMiary> listJM)
        {
            SrtrToZwsiron.ForEach(x =>
            {
                listJM.ForEach(y =>
                {
                    if (x.JednsotkaMiarySRTR.Equals(y.KodJmSrtr))
                        x.JednostkaMiary = y.KodJmZwsiron;
                });
            });
        }

        public void AddGrupaAktywow(List<GrupaAktywow> listGrupaAktywow)
        {
            SrtrToZwsiron.ForEach(x =>
            {
                listGrupaAktywow.ForEach(y =>
                {
                    if (x.GrupaAktywowSRTR.Equals(y.KontoWartPoczSrtr))
                        x.GrupaAktywow = y.GrupaSkladAktywowTrwalych;
                });
            });
        }

        public void AddAmortyzacja(List<Amortyzacja> listAmortyzacja)
        {
            SrtrToZwsiron.ForEach(x =>
            {
                if (string.IsNullOrEmpty(x.WartoscPoczatkowaSRTR))
                {
                    x.StawkaAmor = "0000";
                    x.AmorCzasLata = "0";
                    x.AmorCzasMisiace = "0";
                }
                else
                {
                    listAmortyzacja.ForEach(y =>
                    {
                        if (float.Parse(x.WspolczynnikAmortyzacjiSRTR) == y.KodStawkiAmorSrtr)
                        {
                            x.StawkaAmor = y.KodStawkiAmorZwsiron;
                            x.AmorCzasLata = y.CzasLata.ToString();
                            x.AmorCzasMisiace = y.CzasMiesiace.ToString();
                        }
                    });
                }
            });
        }

        public void AddUzytkownicy()
        {
            SrtrToZwsiron.ForEach(x =>
            {
                Users.ForEach(y =>
                {
                    if (x.IdUzytSrtr.Equals(y.IdSrtr))
                    {
                        x.Mpk = y.Mpk;
                        x.IdUzytZwsiron = y.IdZwsiron;
                    }
                });
            });
        }

        public void AddGrupaGus()
        {
            SrtrToZwsiron.ForEach(x =>
            {
                GrGus.ForEach(y =>
                {
                    if (x.GrupaGusSRTR.Equals(y.KodGrRodzSRTR))
                        x.GrupaGus = y.KodGrRodzZWSIRON;
                });
            });
        }

        public void AddWykaz()
        {
            SrtrToZwsiron.ForEach(x =>
            {
                Wykaz.ForEach(y =>
                {
                    if (x.NrInwentarzowy.Equals(y.NrInwentarzowy))
                    {
                        x.WartoscPoczatkowa = y.WartoscPoczatkowa;
                        x.Umorzenie = y.Umorzenie;
                        x.Zaklad = y.Zaklad;

                        if (!string.IsNullOrEmpty(y.NazwaMaterialu))
                            x.IndeksMaterialowy = y.IndeksMaterialowy;
                    }
                });
            });
        }
        #endregion

        #region Helpers
        public string UzupelnijKWO(string indeks)
        {
            // jeżeli indeks materiałowy jest poprawny
            if (indeks.Length > 7 && indeks.Substring(4, 2).Equals("PL"))
                return indeks.Substring(0, 4);
            else
                return "9999";
        }
        public void GetUsersID()
        {
            Users = new List<Uzytkownik>();

            var q = (from p in SrtrToZwsiron
                     select
                         new Uzytkownik
                         {
                             IdSrtr = p.IdUzytSrtr,

                         }
                    ).Distinct(new UsersComparer()).ToList<Uzytkownik>();

            Users = q;
        }
        public void GetGrupaGus()
        {
            GrGus = new List<GrupaRodzajowaGusSRTR>();

            var q = (from p in SrtrToZwsiron
                     select
                         new GrupaRodzajowaGusSRTR
                         {
                             KodGrRodzSRTR = p.GrupaGusSRTR
                         }
                    ).Distinct(new GrupaGusComparer()).ToList<GrupaRodzajowaGusSRTR>();

            GrGus = q;
        }
        #endregion

        #region SaveFile
        public string SaveFile()
        {
            string success = "Plik zapisano poprawnie.";

            SaveFileDialog saveFile = new SaveFileDialog() { FileName = "dane_SRTR", DefaultExt = ".text", Filter = "Dokumenty tekstowe (.txt)|*.txt" };

            if (saveFile.ShowDialog() == true)
            {
                using (Stream writeStream = saveFile.OpenFile())
                {
                    try
                    {
                        StreamWriter writer = new StreamWriter(writeStream);

                        foreach (SrtrToZwsiron srtrToZwsiron in SrtrToZwsiron)
                        {
                            if (CzyJimPoprawny(srtrToZwsiron))
                            {
                                writer.Write("{0}\t", srtrToZwsiron.GrupaAktywow.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.JednostkaGospodarcza.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.IndeksMaterialowy.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.Nazwa.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.JednostkaMiary.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.Jeden.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.KatSprzetu.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.NrSeryjny.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.NrInwentarzowy.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.DataNabycia2.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.WartoscPoczatkowa.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.Umorzenie.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.Zero.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.StawkaAmor.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.GrupaGus.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.DataNabycia.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.DataNabycia2.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.Mpk.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.AmorCzasLata.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.AmorCzasMisiace.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.Kwo.Trim());
                                writer.Write("{0}\t", srtrToZwsiron.IdUzytZwsiron.Trim());
                                writer.WriteLine("{0}\t", srtrToZwsiron.Zaklad.Trim());
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
        private bool CzyJimPoprawny(SrtrToZwsiron srtrToZwsiron)
        {
            if (srtrToZwsiron.IndeksMaterialowy == null || srtrToZwsiron.IndeksMaterialowy.Length != 13 || !srtrToZwsiron.IndeksMaterialowy.Substring(4, 2).Equals("PL"))
                return false;

            if (string.IsNullOrEmpty(srtrToZwsiron.GrupaAktywow))
                return false;

            return true;
        }
        #endregion

        public void Clean()
        {
            SrtrToZwsiron.Clear();
            Kartoteka.Clear();
            KartotekaZlik.Clear();
            if(Users != null) Users.Clear();
            if (GrGus != null) GrGus.Clear();
            if (Wykaz != null) Wykaz.Clear();
        }
    }
}