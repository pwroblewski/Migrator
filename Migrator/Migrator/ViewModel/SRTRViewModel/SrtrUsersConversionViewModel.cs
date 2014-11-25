using GalaSoft.MvvmLight.Command;
using Migrator.Model;
using Migrator.Services;
using Migrator.Helpers;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Input;

namespace Migrator.ViewModel.SRTRViewModel
{
    public class SrtrUsersConversionViewModel : MainWizardPageViewModelBase
    {
        #region Fields

        private readonly ISRTRService _fSrtrToZwsironService;

        #endregion //Fields

        #region Constructor

        public SrtrUsersConversionViewModel(ISRTRService fSrtrToZwsironService)
        {
            _fSrtrToZwsironService = fSrtrToZwsironService;

            SelectedCells = new List<DataGridCellInfo>();

            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion //Constructor

        #region Properties

        private List<Uzytkownik> _listUzytkownicy;
        public List<Uzytkownik> ListUzytkownicy
        {
            get { return _listUzytkownicy; }
            set { _listUzytkownicy = value; RaisePropertyChanged(() => ListUzytkownicy); }
        }

        private string _slUserPath;
        public string SlUserPath
        {
            get { return _slUserPath; }
            set { _slUserPath = value; RaisePropertyChanged(() => SlUserPath); }
        }

        private List<DataGridCellInfo> _selectedCells;
        public List<DataGridCellInfo> SelectedCells
        {
            get { return _selectedCells; }
            set { _selectedCells = value; RaisePropertyChanged(() => SelectedCells); }
        }

        #endregion //Properties

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

        #region WczytajPlikSlUserCommand

        private RelayCommand<string> _wczytajPlikSlUserCommand;
        public RelayCommand<string> WczytajPlikSlUserCommand
        {
            get
            {
                return _wczytajPlikSlUserCommand
                    ?? (_wczytajPlikSlUserCommand = new RelayCommand<string>(title =>
                        {
                            WczytajSlownikUzytkownikow();
                        }
                ));
            }
        }

        #endregion

        #region Czyszczenie

        private RelayCommand<string> _wyczyscDaneCommand;
        public RelayCommand<string> WyczyscDaneCommand
        {
            get
            {
                return _wyczyscDaneCommand
                    ?? (_wyczyscDaneCommand = new RelayCommand<string>(title =>
                        {
                            WyczyscDaneUzytkownikow();
                        }
                ));
            }
        }

        #endregion

        #endregion //Commands

        #region Private Methods

        private void HandleMessage(Message msg)
        {
            try
            {
                if (msg.MessageText.Equals("synchronizuj dane"))
                    if (_fSrtrToZwsironService.Users == null)
                    {
                        _fSrtrToZwsironService.GetUsersID();
                        ListUzytkownicy = _fSrtrToZwsironService.Users;
                    }
                    else
                    {
                        ListUzytkownicy = _fSrtrToZwsironService.Users;
                    }

                if (msg.MessageText.Equals("zapisz dane"))
                {
                    _fSrtrToZwsironService.Users = ListUzytkownicy;
                    _fSrtrToZwsironService.AddUzytkownicy();

                    Messenger.Default.Send<Message, SrtrGroupGusViewModel>(new Message("synchronizuj dane"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Bład konstruktora", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WczytajSlownikUzytkownikow()
        {
            SlUserPath = _fSrtrToZwsironService.OpenUserFile();
            if (!string.IsNullOrEmpty(SlUserPath))
            {
                try
                {
                    _fSrtrToZwsironService.LoadUserData(SlUserPath);
                    ListUzytkownicy = null;
                    ListUzytkownicy = _fSrtrToZwsironService.Users;

                    Messenger.Default.Send<Message, MainWizardViewModel>(new Message("Poprawnie zsynchronizowano plik z bazą danych."));    // komunikaty o statusie wczytania pliku
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

            if (SelectedCells.Exists(x => x.Column.Header.Equals("MPK")) && SelectedCells.Exists(x => x.Column.Header.Equals("ID użytkownika ZWSI RON")))
            {
                List<Uzytkownik> list = new List<Uzytkownik>();
                List<DataGridCellInfo> SelectedCellsTemp = SelectedCells.FindAll(x => x.Column.Header.Equals("MPK"));

                for (int i = 0; i < SelectedCellsTemp.Count && i < ClipboardContent.Length; i++)
                {
                    string[] ClipboardLine = ClipboardContent[i].Split('\t');

                    Uzytkownik uzytkownik = (Uzytkownik)SelectedCellsTemp[i].Item;
                    uzytkownik.Mpk = ClipboardLine[0];

                    if(ClipboardLine.Length > 1)
                        uzytkownik.IdZwsiron = ClipboardLine[1];

                    list.Add(uzytkownik);
                }

                ListUzytkownicy = PrzypiszListe(list);
            }
            else if (SelectedCells.Exists(x => x.Column.Header.Equals("MPK")) && !SelectedCells.Exists(x => x.Column.Header.Equals("ID użytkownika ZWSI RON")))
            {
                List<Uzytkownik> list = new List<Uzytkownik>();

                for (int i = 0; i < SelectedCells.Count && i < ClipboardContent.Length; i++)
                {
                    string[] ClipboardLine = ClipboardContent[i].Split('\t');

                    Uzytkownik uzytkownik = (Uzytkownik)SelectedCells[i].Item;
                    uzytkownik.Mpk = ClipboardLine[0];

                    list.Add(uzytkownik);
                }

                ListUzytkownicy = PrzypiszListe(list);
            }
            else if (!SelectedCells.Exists(x => x.Column.Header.Equals("MPK")) && SelectedCells.Exists(x => x.Column.Header.Equals("ID użytkownika ZWSI RON")))
            {
                List<Uzytkownik> list = new List<Uzytkownik>();

                for (int i = 0; i < SelectedCells.Count && i < ClipboardContent.Length; i++)
                {
                    string[] ClipboardLine = ClipboardContent[i].Split('\t');

                    Uzytkownik uzytkownik = (Uzytkownik)SelectedCells[i].Item;
                    uzytkownik.IdZwsiron = ClipboardLine[0];

                    list.Add(uzytkownik);
                }

                ListUzytkownicy = PrzypiszListe(list);
            }
        }

        private List<Uzytkownik> PrzypiszListe(List<Uzytkownik> list)
        {
            bool znalazl = false;
            List<Uzytkownik> temp = new List<Uzytkownik>();

            foreach (Uzytkownik user in ListUzytkownicy)
            {
                znalazl = false;
                foreach (Uzytkownik user2 in list)
                {
                    if (user.IdSrtr.Equals(user2.IdSrtr))
                    {
                        znalazl = true;
                        temp.Add(user2);

                        //_dbUserService.Update(user2);
                    }
                }
                if (!znalazl)
                    temp.Add(user);
            }

            return temp;
        }

        private void WyczyscDaneUzytkownikow()
        {
            List<Uzytkownik> temp = new List<Uzytkownik>();

            for (int i = 0; i < ListUzytkownicy.Count; i++)
            {
                temp.Add(new Uzytkownik
                {
                    IdSrtr = ListUzytkownicy[i].IdSrtr
                });
            }

            ListUzytkownicy.Clear();
            ListUzytkownicy = temp;
        }

        internal override bool IsValid()
        {
            // sprawdzenie wypełnienia wszystkich Mpk oraz Id ZWSI RON
            if (ListUzytkownicy != null)
            {
                bool isValid = ListUzytkownicy.Exists(x => string.IsNullOrEmpty(x.Mpk));
                return isValid ? false : true;
            }
            return false;
        }

        internal override string GetPageName()
        {
            return SRTRPages.SrtrUserConversion.ToString();
        }

        private void CallCleanUp(CleanUp cu)
        {
            if (SlUserPath != null) SlUserPath = string.Empty;
            ListUzytkownicy = null;
        }

        #endregion //Private Methods

        internal override void LoadData()
        {
            throw new NotImplementedException();
        }
    }
}
