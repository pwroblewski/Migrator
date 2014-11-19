using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using Migrator.Model;
using Migrator.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Migrator.ViewModel.DictionariesViewModel
{
    public class DictionaryUzytkownicyViewModel : DictionaryPageViewModelBase
    {
        #region Fields

        private readonly IFileUserService _fUserService;
        private readonly IDBUserService _dbUserService;

        #endregion //Fields

        #region Constructor

        public DictionaryUzytkownicyViewModel(IFileUserService fUserService, IDBUserService dbUserService)
        {
            _fUserService = fUserService;
            _dbUserService = dbUserService;

            Name = "Słownik użytkowników";

            SelectedCells = new List<DataGridCellInfo>();

            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion

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

        #region UpdateCommand

        private RelayCommand<DataGridRowEditEndingEventArgs> _updateCommand;
        public RelayCommand<DataGridRowEditEndingEventArgs> UpdateCommand
        {
            get
            {
                return _updateCommand
                    ?? (_updateCommand = new RelayCommand<DataGridRowEditEndingEventArgs>(user =>
                    {
                        _dbUserService.Update((Uzytkownik)user.Row.Item);
                    }
                ));
            }
        }

        #endregion

        #region WczytajPlikSlUserCommand

        private RelayCommand<string> _wczytajSlownikUzytkownikowCommand;
        public RelayCommand<string> WczytajSlownikUzytkownikowCommand
        {
            get
            {
                return _wczytajSlownikUzytkownikowCommand
                    ?? (_wczytajSlownikUzytkownikowCommand = new RelayCommand<string>(title =>
                    {
                        WczytajSlownikUzytkownikow();
                    }
                ));
            }
        }

        #endregion

        #endregion //Commands

        #region Private Methods

        private async void WczytajSlownikUzytkownikow()
        {
            SlUserPath = _fUserService.OpenFileDialog();
            if (!string.IsNullOrEmpty(SlUserPath))
            {
                try
                {
                    // Czytanie pliku
                    var fileData = _fUserService.GetAll(SlUserPath);
                    // Synchronizowanie pliku z bazą danych
                    await _dbUserService.SyncFileUserData(fileData);

                    LoadUsersData();

                    // komunikaty o statusie wczytania pliku
                    Messenger.Default.Send<Message, MainWizardViewModel>(new Message("Poprawnie zsynchronizowano plik z bazą danych."));
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

                    if (ClipboardLine.Length > 1)
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

                        _dbUserService.Update(user2);
                    }
                }
                if (!znalazl)
                    temp.Add(user);
            }

            return temp;
        }

        private async void LoadUsersData()
        {
            ListUzytkownicy = await _dbUserService.GetAll();
        }

        internal override bool IsValid()
        {
            throw new NotImplementedException();
        }

        internal override string Title()
        {
            return Name;
        }

        internal override Helpers.Dictionaries GetPageName()
        {
            return Helpers.Dictionaries.Uzytkownicy;
        }

        private void HandleMessage(Message msg)
        {
            if (ListUzytkownicy == null || ListUzytkownicy.Count == 0)
            {
                LoadUsersData();
            }
        }

        private void CallCleanUp(CleanUp cu)
        {
            if (SlUserPath != null) SlUserPath = string.Empty;
            if (ListUzytkownicy != null) ListUzytkownicy.Clear();
            _fUserService.Clean();
        }

        #endregion //Private Methods
    }
}
