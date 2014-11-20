using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using Migrator.Model;
using Migrator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Migrator.ViewModel.DictionariesViewModel
{
    public class DictionaryGrupyGusViewModel : DictionaryPageViewModelBase
    {
        #region Fields

        private readonly IDBGrRodzGusSRTRService _dbGrRodzGusSRTRService;
        private readonly IDBGrRodzGusZWSIRONService _dbGrRodzGusZWSIRONService;

        #endregion //Fields

        #region Constructor

        public DictionaryGrupyGusViewModel(IDBGrRodzGusSRTRService dbGrRodzGusSRTRService, IDBGrRodzGusZWSIRONService dbGrRodzGusZWSIRONService)
        {
            _dbGrRodzGusSRTRService = dbGrRodzGusSRTRService;
            _dbGrRodzGusZWSIRONService = dbGrRodzGusZWSIRONService;

            Name = "Słownik grup GUS";
            
            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion

        #region Properties

        private List<GrupaRodzajowaGusSRTR> _listGrGusSRTR;
        public List<GrupaRodzajowaGusSRTR> ListGrGusSRTR
        {
            get { return _listGrGusSRTR; }
            set { _listGrGusSRTR = value; RaisePropertyChanged(() => ListGrGusSRTR); }
        }

        private List<GrupaRodzajowaGusZWSIRON> _listGrGusZWSIRON;
        public List<GrupaRodzajowaGusZWSIRON> ListGrGusZWSIRON
        {
            get { return _listGrGusZWSIRON; }
            set { _listGrGusZWSIRON = value; RaisePropertyChanged(() => ListGrGusZWSIRON); }
        }

        private string _grupaGusPath;
        public string GrupaGusPath
        {
            get { return _grupaGusPath; }
            set { _grupaGusPath = value; RaisePropertyChanged(() => GrupaGusPath); }
        }

        #endregion //Properties

        #region Commands

        #region UpdateCommand

        private RelayCommand<RoutedEventArgs> _updateCommand;
        public RelayCommand<RoutedEventArgs> UpdateCommand
        {
            get
            {
                return _updateCommand
                    ?? (_updateCommand = new RelayCommand<RoutedEventArgs>(title =>
                    {
                        _dbGrRodzGusSRTRService.Update((GrupaRodzajowaGusSRTR)((System.Windows.Controls.DataGrid)(title.Source)).CurrentItem);
                    }
                ));
            }
        }

        #endregion

        #region WczytajPlikGrupGusCommand

        private RelayCommand<string> _wczytajPlikGrupGusCommand;
        public RelayCommand<string> WczytajPlikGrupGusCommand
        {
            get
            {
                return _wczytajPlikGrupGusCommand
                    ?? (_wczytajPlikGrupGusCommand = new RelayCommand<string>(
                        title =>
                        {
                            WczytajSlownikGrupGus();
                        }
                ));
            }
        }

        #endregion

        #endregion //Commands

        #region Private Methods

        private async void LoadGUSData()
        {
            ListGrGusSRTR = await _dbGrRodzGusSRTRService.GetAll();
        }

        private async void LoadGUSZwsironData()
        {
            ListGrGusZWSIRON = await _dbGrRodzGusZWSIRONService.GetAll();
        }

        private void WczytajSlownikGrupGus()
        {
            //GrupaGusPath = _fGrupaGusService.OpenFileDialog();
            if (!string.IsNullOrEmpty(GrupaGusPath))
            {
                try
                {
                    // Czytanie pliku
                    //var fileData = _fGrupaGusService.GetAll(GrupaGusPath);
                    // Synchronizowanie pliku z bazą danych
                    //await _dbGrRodzGusSRTRService.SyncFileGusSRTRData(fileData);

                    LoadGUSData();

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
            return Helpers.Dictionaries.GrupaGUS;
        }

        private void HandleMessage(Message msg)
        {
            if (ListGrGusSRTR == null || !ListGrGusSRTR.Any())
            {
                LoadGUSData();
                LoadGUSZwsironData();
            }
        }

        private void CallCleanUp(CleanUp cu)
        {
            if (GrupaGusPath != null) GrupaGusPath = string.Empty;
            if (ListGrGusSRTR != null) ListGrGusSRTR.Clear();
            if (ListGrGusZWSIRON != null) ListGrGusZWSIRON.Clear();
           // _fGrupaGusService.Clean();
        }

        #endregion //Private Methods
    }
}
