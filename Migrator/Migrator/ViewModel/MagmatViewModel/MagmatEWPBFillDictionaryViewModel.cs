using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Migrator.Services;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using Migrator.Model;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Windows;

namespace Migrator.ViewModel.MagmatViewModel
{
    public class MagmatEWPBFillDictionaryViewModel : MainWizardPageViewModelBase
    {
        #region Fields

        private IMAG_EWPBService _fMagEwpbService;

        #endregion //Fields

        #region Constructor

        public MagmatEWPBFillDictionaryViewModel(IMAG_EWPBService fMagEwpbService)
        {
            _fMagEwpbService = fMagEwpbService;

            SelectedCells = new List<DataGridCellInfo>();

            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion //Constructor

        #region Properties
        private List<MagmatEwpb> _listMaterialy;
        public List<MagmatEwpb> ListMaterialy
        {
            get { return _listMaterialy; }
            set { _listMaterialy = value; RaisePropertyChanged(() => ListMaterialy); }
        }

        private List<DataGridCellInfo> _selectedCells;
        public List<DataGridCellInfo> SelectedCells
        {
            get { return _selectedCells; }
            set { _selectedCells = value; RaisePropertyChanged(() => SelectedCells); }
        }

        private string _slownikPath;
        public string SlownikPath
        {
            get { return _slownikPath; }
            set { _slownikPath = value; RaisePropertyChanged(() => SlownikPath); }
        }

        private bool _magazynVisibility;
        public bool MagazynVisibility
        {
            get { return _magazynVisibility; }
            set { _magazynVisibility = value; RaisePropertyChanged(() => MagazynVisibility); }
        }

        private bool _uzytkownikVisibility;
        public bool UzytkownikVisibility
        {
            get { return _uzytkownikVisibility; }
            set { _uzytkownikVisibility = value; RaisePropertyChanged(() => UzytkownikVisibility); }
        }

        private bool _jednostkaVisibility;
        public bool JednostkaVisibility
        {
            get { return _jednostkaVisibility; }
            set { _jednostkaVisibility = value; RaisePropertyChanged(() => JednostkaVisibility); }
        }

        #endregion

        #region Commands

        #region SelectedCellsChangedCommand

        private RelayCommand<SelectedCellsChangedEventArgs> _selectedCellsChangedCommand;
        public RelayCommand<SelectedCellsChangedEventArgs> SelectedCellsChangedCommand
        {
            get
            {
                return _selectedCellsChangedCommand
                    ?? (_selectedCellsChangedCommand = new RelayCommand<SelectedCellsChangedEventArgs>(
                        user =>
                        {
                            SelectedCells.AddRange(user.AddedCells);
                            SelectedCells.RemoveAll(x => user.RemovedCells.Contains(x));
                        }
                ));
            }
        }

        #endregion

        #region KeyDownCommand

        private RelayCommand<KeyEventArgs> _keyDownCommand;
        public RelayCommand<KeyEventArgs> KeyDownCommand
        {
            get
            {
                return _keyDownCommand
                    ?? (_keyDownCommand = new RelayCommand<KeyEventArgs>(
                        user =>
                        {
                            if (user.Key == Key.V && (Keyboard.Modifiers == ModifierKeys.Control)) // Paste
                            {
                                WklejDaneZeSchowkaSystemowego();
                            }
                        }
                ));
            }
        }

        #endregion

        #region WczytajPlikSlCommand

        private RelayCommand<string> _wczytajPlikSlCommand;
        public RelayCommand<string> WczytajPlikSlCommand
        {
            get
            {
                return _wczytajPlikSlCommand
                    ?? (_wczytajPlikSlCommand = new RelayCommand<string>(
                        user => WczytajPlikSl()
                ));
            }
        }

        #endregion

        #endregion

        #region Methods

        private void HandleMessage(Message msg)
        {
            if (msg.MessageText.Equals("synchronizuj dane"))
            {
                if (_fMagEwpbService.Dictionaries == null)
                    _fMagEwpbService.FillDictionaryData();

                ListMaterialy = _fMagEwpbService.Dictionaries;
                MsgToVisibility(_fMagEwpbService.TypWydruku);
            }
            if (msg.MessageText.Equals("zapisz dane"))
            {
                _fMagEwpbService.Dictionaries = ListMaterialy;
                _fMagEwpbService.AddDictionary();

                Messenger.Default.Send<Message, MagmatEWPBJimDataViewModel>(new Message("synchronizuj dane"));
            }
            if (msg.MessageText.Equals("czyść dane"))
            {
                SelectedCells.Clear();
            }
        }

        private void WczytajPlikSl()
        {
            SlownikPath = _fMagEwpbService.OpenDictionaryFile();
            if (!string.IsNullOrEmpty(SlownikPath))
            {
                try
                {
                    _fMagEwpbService.LoadDictionaryData(SlownikPath);
                    ListMaterialy = null;
                    ListMaterialy = _fMagEwpbService.Dictionaries;

                    // komunikaty o statusie wczytania pliku
                    Messenger.Default.Send<Message, MainWizardViewModel>(new Message("Poprawnie wczytano plik z bazą danych."));
                }
                catch (Exception ex)
                {
                    string msg = string.Format("BŁĄD! - {0}", ex.Message);
                    MessageBox.Show(msg, "Bład odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void WklejDaneZeSchowkaSystemowego()
        {
            string[] separator = new string[] { "\r\n" };
            string[] ClipboardContent = Clipboard.GetText().Split(separator, StringSplitOptions.RemoveEmptyEntries);

            if (SelectedCells.Exists(x => x.Column.Header.Equals("Zakład")) && SelectedCells.Exists(x => x.Column.Header.Equals("Skład")))
            {
                List<MagmatEwpb> list = new List<MagmatEwpb>();
                List<DataGridCellInfo> SelectedCellsTemp = SelectedCells.FindAll(x => x.Column.Header.Equals("Zakład"));

                for (int i = 0; i < SelectedCellsTemp.Count && i < ClipboardContent.Length; i++)
                {
                    string[] ClipboardLine = ClipboardContent[i].Split('\t');

                    MagmatEwpb material = (MagmatEwpb)SelectedCellsTemp[i].Item;
                    material.Zaklad = ClipboardLine[0];

                    if (ClipboardLine.Length > 1)
                        material.Sklad = ClipboardLine[1];

                    list.Add(material);
                }

                ListMaterialy = PrzypiszListe(list);
            }
            else if (SelectedCells.Exists(x => x.Column.Header.Equals("Zakład")) && !SelectedCells.Exists(x => x.Column.Header.Equals("Skład")))
            {
                List<MagmatEwpb> list = new List<MagmatEwpb>();

                for (int i = 0; i < SelectedCells.Count && i < ClipboardContent.Length; i++)
                {
                    string[] ClipboardLine = ClipboardContent[i].Split('\t');

                    MagmatEwpb material = (MagmatEwpb)SelectedCells[i].Item;
                    material.Zaklad = ClipboardLine[0];

                    list.Add(material);
                }

                ListMaterialy = PrzypiszListe(list);
            }
            else if (!SelectedCells.Exists(x => x.Column.Header.Equals("Zakład")) && SelectedCells.Exists(x => x.Column.Header.Equals("Skład")))
            {
                List<MagmatEwpb> list = new List<MagmatEwpb>();

                for (int i = 0; i < SelectedCells.Count && i < ClipboardContent.Length; i++)
                {
                    string[] ClipboardLine = ClipboardContent[i].Split('\t');

                    MagmatEwpb material = (MagmatEwpb)SelectedCells[i].Item;
                    material.Sklad = ClipboardLine[0];

                    list.Add(material);
                }

                ListMaterialy = PrzypiszListe(list);
            }
            else if (SelectedCells.Exists(x => x.Column.Header.Equals("ID użytkownika ZWSI RON")))
            {
                List<MagmatEwpb> list = new List<MagmatEwpb>();

                for (int i = 0; i < SelectedCells.Count && i < ClipboardContent.Length; i++)
                {
                    string[] ClipboardLine = ClipboardContent[i].Split('\t');

                    MagmatEwpb material = (MagmatEwpb)SelectedCells[i].Item;
                    material.UzytkownikZwsiron = ClipboardLine[0];

                    list.Add(material);
                }

                ListMaterialy = PrzypiszListe(list);
            }

        }

        private List<MagmatEwpb> PrzypiszListe(List<MagmatEwpb> list)
        {
            bool znalazl = false;
            List<MagmatEwpb> temp = new List<MagmatEwpb>();

            foreach (MagmatEwpb magEwpb in ListMaterialy)
            {
                znalazl = false;
                foreach (MagmatEwpb magEwpb2 in list)
                {
                    if (magEwpb.Lp.Equals(magEwpb2.Lp))
                    {
                        znalazl = true;
                        temp.Add(magEwpb2);
                    }
                }
                if (!znalazl)
                    temp.Add(magEwpb);
            }

            return temp;
        }

        private void MsgToVisibility(MagmatEWPB typ)
        {
            switch (typ)
            {
                case MagmatEWPB.Magmat_305:
                    MagazynVisibility = true;
                    UzytkownikVisibility = false;
                    JednostkaVisibility = false;
                    break;

                case MagmatEWPB.EWPB_319_320:
                    MagazynVisibility = false;
                    UzytkownikVisibility = true;
                    JednostkaVisibility = false;
                    break;

                case MagmatEWPB.EWPB_351:
                    MagazynVisibility = false;
                    UzytkownikVisibility = false;
                    JednostkaVisibility = true;
                    break;
            }
        }

        private void CallCleanUp(CleanUp cu)
        {
            ListMaterialy = null;
            if (SelectedCells != null) SelectedCells.Clear();
        }

        #endregion //Methods

        internal override bool IsValid()
        {
            if (ListMaterialy != null)
            {
                var idZwsiron = ListMaterialy.Exists(x => string.IsNullOrEmpty(x.UzytkownikZwsiron));
                var zakladSklad = ListMaterialy.Exists(x => string.IsNullOrEmpty(x.Zaklad) || string.IsNullOrEmpty(x.Sklad));

                if (_fMagEwpbService.TypWydruku == MagmatEWPB.EWPB_319_320)
                {
                    if (idZwsiron || zakladSklad) return false;
                    else return true;
                }
                else
                {
                    if (zakladSklad) return false;
                    else return true;
                }
            }
            return false;
        }

        internal override string GetPageName()
        {
            return Helpers.MagmatEWPBPages.MagFillDictionary.ToString();
        }

        internal override void LoadData()
        {
            throw new NotImplementedException();
        }
    }
}
