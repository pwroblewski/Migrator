using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using Migrator.Model;
using Migrator.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Migrator.ViewModel.MagmatViewModel
{
    public class MagmatEWPBSigmatViewModel : MainWizardPageViewModelBase
    {
        #region Fields

        private IFileSigmatService _fSigmatService;

        #endregion //Fields

        #region Constructor

        public MagmatEWPBSigmatViewModel(IFileSigmatService fSigmatService)
        {
            _fSigmatService = fSigmatService;

            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion //Constructor

        #region Properties

        private MagmatEWPB _typWydruku;
        public MagmatEWPB TypWydruku
        {
            get { return _typWydruku; }
            set { _typWydruku = value; RaisePropertyChanged(() => TypWydruku); }
        }

        private TabItem _selectedItem;
        public TabItem SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; RaisePropertyChanged(() => SelectedItem); }
        }

        private List<SigmatKat> _listKat;
        public List<SigmatKat> ListKat
        {
            get { return _listKat; }
            set { _listKat = value; RaisePropertyChanged(() => ListKat); }
        }

        private List<SigmatAmunicja> _listAmunicja;
        public List<SigmatAmunicja> ListAmunicja
        {
            get { return _listAmunicja; }
            set { _listAmunicja = value; RaisePropertyChanged(() => ListAmunicja); }
        }

        private List<SigmatMund> _listMund;
        public List<SigmatMund> ListMund
        {
            get { return _listMund; }
            set { _listMund = value; RaisePropertyChanged(() => ListMund); }
        }

        private List<SigmatPaliwa> _listPaliwa;
        public List<SigmatPaliwa> ListPaliwa
        {
            get { return _listPaliwa; }
            set { _listPaliwa = value; RaisePropertyChanged(() => ListPaliwa); }
        }

        private List<SigmatZywnosc> _listZywnosc;
        public List<SigmatZywnosc> ListZywnosc
        {
            get { return _listZywnosc; }
            set { _listZywnosc = value; RaisePropertyChanged(() => ListZywnosc); }
        }

        private bool _userVis;
        public bool UserVis
        {
            get { return _userVis; }
            set { _userVis = value; RaisePropertyChanged(() => UserVis); }
        }
        
        private bool _userNotVis;
        public bool UserNotVis
        {
            get { return _userNotVis; }
            set { _userNotVis = value; RaisePropertyChanged(() => UserNotVis); }
        }
        #endregion

        #region Commands

        #region UtworzPlikCommand

        private RelayCommand<string> _utworzPlikCommand;
        public RelayCommand<string> UtworzPlikCommand
        {
            get
            {
                return _utworzPlikCommand
                    ?? (_utworzPlikCommand = new RelayCommand<string>(
                        file => UtworzPlik(TypWydruku)
                ));
            }
        }

        #endregion

        #endregion //Commands

        #region Methods

        private void HandleMessage(Message msg)
        {
            if (msg.MessageText.Equals("synchronizuj dane"))
            {
                TypWydruku = (MagmatEWPB)msg.MessageObject;

                switch (TypWydruku)
                {
                    case MagmatEWPB.Magmat305:
                        UserVis = false;
                        UserNotVis = true;
                        break;
                    case MagmatEWPB.Ewpb319_320:
                        UserVis = true;
                        UserNotVis = false;
                        break;
                    case MagmatEWPB.EWpb351:
                        UserVis = false;
                        UserNotVis = true;
                        break;
                    default:
                        break;
                }

                ListAmunicja = _fSigmatService.GetAmunicja();
                ListKat = _fSigmatService.GetKat();
                ListPaliwa = _fSigmatService.GetPaliwa();
                ListMund = _fSigmatService.GetMund();
                ListZywnosc = _fSigmatService.GetZywnosc();
            }
        }

        private void UtworzPlik(MagmatEWPB TypWydruku)
        {
            //string msg = _fJimService.SaveFileDialog(ListMaterialy);
            switch (SelectedItem.Header.ToString())
            {
                case "ŻYWNOŚC (LEKARSTWA)":
                    ZapiszZywnosc(TypWydruku);
                    break;
                case "AMUNICJA":
                    ZapiszAmunicja(TypWydruku);
                    break;
                case "KAT":
                    ZapiszKat(TypWydruku);
                    break;
                case "PALIWA":
                    ZapiszPaliwa(TypWydruku);
                    break;
                case "MUND":
                    ZapiszMund(TypWydruku);
                    break;
            }

            Messenger.Default.Send<Message, MainWizardViewModel>(new Message("Plik zapisano poprawnie."));
        }

        private void ZapiszZywnosc(MagmatEWPB TypWydruku)
        {
            if (ListZywnosc != null && ListZywnosc.Any())
            {
                var dlg = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = dlg.ShowDialog();

                var list_zyw = ListZywnosc.GroupBy(x => new { x.Zaklad, x.Sklad })
                    .Select(x => x.ToList())
                    .ToList();

                try
                {
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
                                StreamWriter writer = new StreamWriter(FileName2);

                                foreach (var zywnosc in temp_list[i - 1])
                                {
                                    NullToStringEmptyConversion(zywnosc);
                                    if(TypWydruku == MagmatEWPB.Ewpb319_320)
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}", zywnosc.Jim.PadRight(18), zywnosc.Ilosc.ToString().PadRight(17), zywnosc.Wartosc.ToString().PadRight(16), zywnosc.DataWaznosci.ToString().PadRight(10), zywnosc.NrPartiiProducenta.ToString().PadRight(15), zywnosc.Opakowanie.ToString().PadRight(10), zywnosc.Uzytkownik_ID.ToString().PadRight(10), zywnosc.DataWydania.ToString().PadRight(10), zywnosc.WyposazenieIndywidualne.ToString().PadRight(1));
                                    else
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}", zywnosc.Jim.PadRight(18), zywnosc.Ilosc.ToString().PadRight(17), zywnosc.Wartosc.ToString().PadRight(16), zywnosc.DataWaznosci.ToString().PadRight(10), zywnosc.NrPartiiProducenta.ToString().PadRight(15), zywnosc.Opakowanie.ToString().PadRight(10));
                                    writer.Flush();
                                }
                            }
                        }
                        else
                        {
                            StreamWriter writer = new StreamWriter(FileName);

                            foreach (var zywnosc in lista)
                            {
                                NullToStringEmptyConversion(zywnosc);
                                if (TypWydruku == MagmatEWPB.Ewpb319_320)
                                    writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}", zywnosc.Jim.PadRight(18), zywnosc.Ilosc.ToString().PadRight(17), zywnosc.Wartosc.ToString().PadRight(16), zywnosc.DataWaznosci.ToString().PadRight(10), zywnosc.NrPartiiProducenta.ToString().PadRight(15), zywnosc.Opakowanie.ToString().PadRight(10), zywnosc.Uzytkownik_ID.ToString().PadRight(10), zywnosc.DataWydania.ToString().PadRight(10), zywnosc.WyposazenieIndywidualne.ToString().PadRight(1));
                                else
                                    writer.WriteLine("{0}{1}{2}{3}{4}{5}", zywnosc.Jim.PadRight(18), zywnosc.Ilosc.ToString().PadRight(17), zywnosc.Wartosc.ToString().PadRight(16), zywnosc.DataWaznosci.ToString().PadRight(10), zywnosc.NrPartiiProducenta.ToString().PadRight(15), zywnosc.Opakowanie.ToString().PadRight(10));
                                writer.Flush();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Brak danych!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ZapiszAmunicja(MagmatEWPB TypWydruku)
        {
            if (ListAmunicja != null && ListAmunicja.Any())
            {
                var dlg = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = dlg.ShowDialog();

                var lista_list = ListAmunicja.GroupBy(x => new { x.Zaklad, x.Sklad })
                    .Select(x => x.ToList())
                    .ToList();

                try
                {
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
                                StreamWriter writer = new StreamWriter(FileName2);

                                foreach (var amunicja in temp_list[i - 1])
                                {
                                    NullToStringEmptyConversion(amunicja);
                                    if (TypWydruku == MagmatEWPB.Ewpb319_320)
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}",
                                            amunicja.Jim.PadRight(18),
                                            amunicja.Ilosc.ToString().PadRight(17),
                                            amunicja.Wartosc.ToString().PadRight(16),
                                            amunicja.Kategoria.ToString().PadRight(1),
                                            amunicja.Kategoria2.ToString().PadRight(1),
                                            amunicja.NrPartii.ToString().PadRight(17),
                                            amunicja.DataProdukcji.ToString().PadRight(8),
                                            amunicja.DataZablokowania.ToString().PadRight(8),
                                            amunicja.Wyroznik.ToString().PadRight(1),
                                            amunicja.DataGwarancji.ToString().PadRight(8),
                                            amunicja.ZnacznikBlokowania.ToString().PadRight(1),
                                            amunicja.NrSeryjny.ToString().PadRight(5),
                                            amunicja.Uzytkownik_ID.ToString().PadRight(10),
                                            amunicja.DataWydania.ToString().PadRight(8),
                                            amunicja.WyposazenieIndywidualne.ToString().PadRight(1));
                                    else
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}{23}{24}{25}{26}{27}{28}{29}{30}",
                                            amunicja.Jim.PadRight(18),
                                            amunicja.Ilosc.ToString().PadRight(17),
                                            amunicja.Wartosc.ToString().PadRight(16),
                                            amunicja.Kategoria.ToString().PadRight(1),
                                            amunicja.Kategoria2.ToString().PadRight(1),
                                            amunicja.NrPartii.ToString().PadRight(17),
                                            amunicja.DataProdukcji.ToString().PadRight(8),
                                            amunicja.DataZablokowania.ToString().PadRight(8),
                                            amunicja.Wyroznik.ToString().PadRight(1),
                                            amunicja.DataGwarancji.ToString().PadRight(8),
                                            amunicja.ZnacznikBlokowania.ToString().PadRight(1),
                                            amunicja.NrSeryjny.ToString().PadRight(5),
                                            amunicja.DataGwarancjiJBR.ToString().PadRight(8),
                                            amunicja.Zapalnik.ToString().PadRight(17),
                                            amunicja.ZapalnikDataGwarancji.ToString().PadRight(8),
                                            amunicja.ZapalnikDataGwarancjiJBR.ToString().PadRight(8),
                                            amunicja.Zaplonnik.ToString().PadRight(17),
                                            amunicja.ZaplonnikDataGwarancji.ToString().PadRight(8),
                                            amunicja.ZaplonnikDataGwarancjiJBR.ToString().PadRight(8),
                                            amunicja.Ladunek.ToString().PadRight(17),
                                            amunicja.LadunekDataGwarancji.ToString().PadRight(8),
                                            amunicja.LadunekDataGwarancjiJBR.ToString().PadRight(8),
                                            amunicja.Pocisk.ToString().PadRight(17),
                                            amunicja.PociskDataGwarancji.ToString().PadRight(8),
                                            amunicja.PociskDataGwarancjiJBR.ToString().PadRight(8),
                                            amunicja.Zrodlo.ToString().PadRight(17),
                                            amunicja.ZrodloDataGwarancji.ToString().PadRight(8),
                                            amunicja.ZrodloDataGwarancjiJBR.ToString().PadRight(8),
                                            amunicja.Smugacz.ToString().PadRight(17),
                                            amunicja.SmugaczDataGwarancji.ToString().PadRight(8),
                                            amunicja.SmugaczDataGwarancjiJBR.ToString().PadRight(8));
                                    writer.Flush();
                                }
                            }
                        }
                        else
                        {
                            StreamWriter writer = new StreamWriter(FileName);

                            foreach (var amunicja in lista)
                            {
                                NullToStringEmptyConversion(amunicja);
                                if (TypWydruku == MagmatEWPB.Ewpb319_320)
                                    writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}",
                                        amunicja.Jim.PadRight(18),
                                        amunicja.Ilosc.ToString().PadRight(17),
                                        amunicja.Wartosc.ToString().PadRight(16),
                                        amunicja.Kategoria.ToString().PadRight(1),
                                        amunicja.Kategoria2.ToString().PadRight(1),
                                        amunicja.NrPartii.ToString().PadRight(17),
                                        amunicja.DataProdukcji.ToString().PadRight(8),
                                        amunicja.DataZablokowania.ToString().PadRight(8),
                                        amunicja.Wyroznik.ToString().PadRight(1),
                                        amunicja.DataGwarancji.ToString().PadRight(8),
                                        amunicja.ZnacznikBlokowania.ToString().PadRight(1),
                                        amunicja.NrSeryjny.ToString().PadRight(5),
                                        amunicja.Uzytkownik_ID.ToString().PadRight(10),
                                        amunicja.DataWydania.ToString().PadRight(8),
                                        amunicja.WyposazenieIndywidualne.ToString().PadRight(1));
                                else
                                    writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}{23}{24}{25}{26}{27}{28}{29}{30}",
                                        amunicja.Jim.PadRight(18),
                                        amunicja.Ilosc.ToString().PadRight(17),
                                        amunicja.Wartosc.ToString().PadRight(16),
                                        amunicja.Kategoria.ToString().PadRight(1),
                                        amunicja.Kategoria2.ToString().PadRight(1),
                                        amunicja.NrPartii.ToString().PadRight(17),
                                        amunicja.DataProdukcji.ToString().PadRight(8),
                                        amunicja.DataZablokowania.ToString().PadRight(8),
                                        amunicja.Wyroznik.ToString().PadRight(1),
                                        amunicja.DataGwarancji.ToString().PadRight(8),
                                        amunicja.ZnacznikBlokowania.ToString().PadRight(1),
                                        amunicja.NrSeryjny.ToString().PadRight(5),
                                        amunicja.DataGwarancjiJBR.ToString().PadRight(8),
                                        amunicja.Zapalnik.ToString().PadRight(17),
                                        amunicja.ZapalnikDataGwarancji.ToString().PadRight(8),
                                        amunicja.ZapalnikDataGwarancjiJBR.ToString().PadRight(8),
                                        amunicja.Zaplonnik.ToString().PadRight(17),
                                        amunicja.ZaplonnikDataGwarancji.ToString().PadRight(8),
                                        amunicja.ZaplonnikDataGwarancjiJBR.ToString().PadRight(8),
                                        amunicja.Ladunek.ToString().PadRight(17),
                                        amunicja.LadunekDataGwarancji.ToString().PadRight(8),
                                        amunicja.LadunekDataGwarancjiJBR.ToString().PadRight(8),
                                        amunicja.Pocisk.ToString().PadRight(17),
                                        amunicja.PociskDataGwarancji.ToString().PadRight(8),
                                        amunicja.PociskDataGwarancjiJBR.ToString().PadRight(8),
                                        amunicja.Zrodlo.ToString().PadRight(17),
                                        amunicja.ZrodloDataGwarancji.ToString().PadRight(8),
                                        amunicja.ZrodloDataGwarancjiJBR.ToString().PadRight(8),
                                        amunicja.Smugacz.ToString().PadRight(17),
                                        amunicja.SmugaczDataGwarancji.ToString().PadRight(8),
                                        amunicja.SmugaczDataGwarancjiJBR.ToString().PadRight(8)); 
                                writer.Flush();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Brak danych!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ZapiszKat(MagmatEWPB TypWydruku)
        {
            if (ListKat != null && ListKat.Any())
            {
                var dlg = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = dlg.ShowDialog();

                var lista_list = ListKat.GroupBy(x => new { x.Zaklad, x.Sklad })
                    .Select(x => x.ToList())
                    .ToList();

                try
                {
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
                                StreamWriter writer = new StreamWriter(FileName2);

                                foreach (var kat in temp_list[i - 1])
                                {
                                    NullToStringEmptyConversion(kat);
                                    if (TypWydruku == MagmatEWPB.Ewpb319_320)
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}", kat.Jim.PadRight(18), kat.Ilosc.ToString().PadRight(17), kat.Wartosc.ToString().PadRight(16), kat.Kategoria.ToString().PadRight(1), kat.NrSeryjny.ToString().PadRight(15), kat.DataNabycia.ToString().PadRight(10), kat.WartoscPoczatkowa.ToString().PadRight(11), kat.WartoscUmorzenia.ToString().PadRight(11), kat.StawkaAmortyzacji.ToString().PadRight(4), kat.KlasSrodkowTrwalych.ToString().PadRight(3), kat.DataProdukcji.ToString().PadRight(10), kat.DataGwarancji.ToString().PadRight(10), kat.Uzytkownik_ID.ToString().PadRight(10), kat.DataWydania.ToString().PadRight(10), kat.WyposazenieIndywidualne.ToString().PadRight(1), kat.Pododdzial.ToString().PadRight(20));
                                    else
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", kat.Jim.PadRight(18), kat.Ilosc.ToString().PadRight(17), kat.Wartosc.ToString().PadRight(16), kat.Kategoria.ToString().PadRight(1), kat.NrSeryjny.ToString().PadRight(15), kat.DataNabycia.ToString().PadRight(10), kat.WartoscPoczatkowa.ToString().PadRight(11), kat.WartoscUmorzenia.ToString().PadRight(11), kat.StawkaAmortyzacji.ToString().PadRight(4), kat.KlasSrodkowTrwalych.ToString().PadRight(3), kat.DataProdukcji.ToString().PadRight(10), kat.DataGwarancji.ToString().PadRight(10), kat.KodStan.ToString().PadRight(1));
                                    writer.Flush();
                                }
                            }
                        }
                        else
                        {
                            StreamWriter writer = new StreamWriter(FileName);

                            foreach (var kat in lista)
                            {
                                NullToStringEmptyConversion(kat);
                                if (TypWydruku == MagmatEWPB.Ewpb319_320)
                                    writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}", kat.Jim.PadRight(18), kat.Ilosc.ToString().PadRight(17), kat.Wartosc.ToString().PadRight(16), kat.Kategoria.ToString().PadRight(1), kat.NrSeryjny.ToString().PadRight(15), kat.DataNabycia.ToString().PadRight(10), kat.WartoscPoczatkowa.ToString().PadRight(11), kat.WartoscUmorzenia.ToString().PadRight(11), kat.StawkaAmortyzacji.ToString().PadRight(4), kat.KlasSrodkowTrwalych.ToString().PadRight(3), kat.DataProdukcji.ToString().PadRight(10), kat.DataGwarancji.ToString().PadRight(10), kat.Uzytkownik_ID.ToString().PadRight(10), kat.DataWydania.ToString().PadRight(10), kat.WyposazenieIndywidualne.ToString().PadRight(1), kat.Pododdzial.ToString().PadRight(20));
                                else
                                    writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", kat.Jim.PadRight(18), kat.Ilosc.ToString().PadRight(17), kat.Wartosc.ToString().PadRight(16), kat.Kategoria.ToString().PadRight(1), kat.NrSeryjny.ToString().PadRight(15), kat.DataNabycia.ToString().PadRight(10), kat.WartoscPoczatkowa.ToString().PadRight(11), kat.WartoscUmorzenia.ToString().PadRight(11), kat.StawkaAmortyzacji.ToString().PadRight(4), kat.KlasSrodkowTrwalych.ToString().PadRight(3), kat.DataProdukcji.ToString().PadRight(10), kat.DataGwarancji.ToString().PadRight(10), kat.KodStan.ToString().PadRight(1));
                                writer.Flush();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Brak danych!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ZapiszPaliwa(MagmatEWPB TypWydruku)
        {
            if (ListPaliwa != null && ListPaliwa.Any())
            {
                var dlg = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = dlg.ShowDialog();

                var lista_list = ListPaliwa.GroupBy(x => new { x.Zaklad, x.Sklad })
                    .Select(x => x.ToList())
                    .ToList();

                try
                {
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
                                StreamWriter writer = new StreamWriter(FileName2);

                                foreach (var pal in temp_list[i - 1])
                                {
                                    NullToStringEmptyConversion(pal);
                                    if (TypWydruku == MagmatEWPB.Ewpb319_320)
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", pal.Jim.PadRight(18), pal.Ilosc.ToString().PadRight(17), pal.Wartosc.ToString().PadRight(16), pal.DataProdukcji.ToString().PadRight(10), pal.TypOpakowania.ToString().PadRight(10), pal.Wycena.ToString().PadRight(1), pal.Orzeczenie.ToString().PadRight(30), pal.Uzytkownik_ID.ToString().PadRight(10), pal.DataWydania.ToString().PadRight(10), pal.WyposazenieIndywidualne.ToString().PadRight(1));
                                    else
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}", pal.Jim.PadRight(18), pal.Ilosc.ToString().PadRight(17), pal.Wartosc.ToString().PadRight(16), pal.DataProdukcji.ToString().PadRight(10), pal.TypOpakowania.ToString().PadRight(10), pal.Wycena.ToString().PadRight(1), pal.Orzeczenie.ToString().PadRight(30));
                                    writer.Flush();
                                }
                            }
                        }
                        else
                        {
                            StreamWriter writer = new StreamWriter(FileName);

                            foreach (var pal in lista)
                            {
                                NullToStringEmptyConversion(pal);
                                if (TypWydruku == MagmatEWPB.Ewpb319_320)
                                    writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", pal.Jim.PadRight(18), pal.Ilosc.ToString().PadRight(17), pal.Wartosc.ToString().PadRight(16), pal.DataProdukcji.ToString().PadRight(10), pal.TypOpakowania.ToString().PadRight(10), pal.Wycena.ToString().PadRight(1), pal.Orzeczenie.ToString().PadRight(30), pal.Uzytkownik_ID.ToString().PadRight(10), pal.DataWydania.ToString().PadRight(10), pal.WyposazenieIndywidualne.ToString().PadRight(1));
                                else
                                    writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}", pal.Jim.PadRight(18), pal.Ilosc.ToString().PadRight(17), pal.Wartosc.ToString().PadRight(16), pal.DataProdukcji.ToString().PadRight(10), pal.TypOpakowania.ToString().PadRight(10), pal.Wycena.ToString().PadRight(1), pal.Orzeczenie.ToString().PadRight(30));
                                writer.Flush();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Brak danych!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ZapiszMund(MagmatEWPB TypWydruku)
        {
            if (ListMund != null && ListMund.Any())
            {
                var dlg = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = dlg.ShowDialog();

                var lista_list = ListMund.GroupBy(x => new { x.Zaklad, x.Sklad })
                    .Select(x => x.ToList())
                    .ToList();

                try
                {
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
                                StreamWriter writer = new StreamWriter(FileName2);

                                foreach (var mund in temp_list[i - 1])
                                {
                                    NullToStringEmptyConversion(mund);
                                    if (TypWydruku == MagmatEWPB.Ewpb319_320)
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", mund.Jim.PadRight(18), mund.Ilosc.ToString().PadRight(17), mund.Wartosc.ToString().PadRight(16), mund.Kategoria.ToString().PadRight(1), mund.Rozmiar.ToString().PadRight(11), mund.RokProdukcji.ToString().PadRight(4), mund.RokGwarancji.ToString().PadRight(4), mund.Uzytkownik_ID.ToString().PadRight(10), mund.DataWydania.ToString().PadRight(10), mund.WyposazenieInduwidualne.ToString().PadRight(1), mund.Pododdzial.ToString().PadRight(20), mund.OkresUzywalnosci.ToString().PadRight(3), mund.TypPozycjiPodzestawu.ToString().PadRight(1));
                                    else
                                        writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}", mund.Jim.PadRight(18), mund.Ilosc.ToString().PadRight(17), mund.Wartosc.ToString().PadRight(16), mund.Kategoria.ToString().PadRight(1), mund.Rozmiar.ToString().PadRight(11), mund.RokProdukcji.ToString().PadRight(4), mund.RokGwarancji.ToString().PadRight(4));
                                    writer.Flush();
                                }
                            }
                        }
                        else
                        {
                            StreamWriter writer = new StreamWriter(FileName);

                            foreach (var mund in lista)
                            {
                                NullToStringEmptyConversion(mund);
                                if (TypWydruku == MagmatEWPB.Ewpb319_320)
                                    writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", mund.Jim.PadRight(18), mund.Ilosc.ToString().PadRight(17), mund.Wartosc.ToString().PadRight(16), mund.Kategoria.ToString().PadRight(1), mund.Rozmiar.ToString().PadRight(11), mund.RokProdukcji.ToString().PadRight(4), mund.RokGwarancji.ToString().PadRight(4), mund.Uzytkownik_ID.ToString().PadRight(10), mund.DataWydania.ToString().PadRight(10), mund.WyposazenieInduwidualne.ToString().PadRight(1), mund.Pododdzial.ToString().PadRight(20), mund.OkresUzywalnosci.ToString().PadRight(3), mund.TypPozycjiPodzestawu.ToString().PadRight(1));
                                else
                                    writer.WriteLine("{0}{1}{2}{3}{4}{5}{6}", mund.Jim.PadRight(18), mund.Ilosc.ToString().PadRight(17), mund.Wartosc.ToString().PadRight(16), mund.Kategoria.ToString().PadRight(1), mund.Rozmiar.ToString().PadRight(11), mund.RokProdukcji.ToString().PadRight(4), mund.RokGwarancji.ToString().PadRight(4));
                                writer.Flush();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void CallCleanUp(CleanUp cu)
        {
            if (ListAmunicja != null) ListAmunicja.Clear();
            if (ListKat != null) ListKat.Clear();
            if (ListMund != null) ListMund.Clear();
            if (ListPaliwa != null) ListPaliwa.Clear();
            if (ListZywnosc != null) ListZywnosc.Clear();
        }

        #endregion //Methods

        internal override bool IsValid()
        {
            return true;
        }

        internal override string GetPageName()
        {
            return MagmatEWPBPages.MagSigmat.ToString();
        }

        internal override void LoadData()
        {
            throw new NotImplementedException();
        }
    }
}
