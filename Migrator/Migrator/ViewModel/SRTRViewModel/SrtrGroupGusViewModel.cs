﻿using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using Migrator.Model;
using Migrator.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace Migrator.ViewModel.SRTRViewModel
{
    public class SrtrGroupGusViewModel : MainWizardPageViewModelBase
    {
        #region Fields

        private readonly ISRTRService _fSrtrToZwsironService;
        private readonly IDBGrRodzGusZWSIRONService _dbGrRodzGusZWSIRONService;

        #endregion //Fields

        #region Constructor

        public SrtrGroupGusViewModel(ISRTRService fSrtrToZwsironService, IDBGrRodzGusZWSIRONService dbGrRodzGusZWSIRONService)
        {
            _fSrtrToZwsironService = fSrtrToZwsironService;
            _dbGrRodzGusZWSIRONService = dbGrRodzGusZWSIRONService;

            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion //Constructor

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

        private async void HandleMessage(Message msg)
        {
            if (msg.MessageText.Equals("synchronizuj dane"))
            {
                ListGrGusZWSIRON = await _dbGrRodzGusZWSIRONService.GetAll(); // pobranie danych z bazy o grupach rodzajowych ZWSI RON

                if (_fSrtrToZwsironService.GrGus == null)
                {
                    _fSrtrToZwsironService.GetGrupaGus();   // pobranie danych grup GUS z kartoteki
                    ListGrGusSRTR = _fSrtrToZwsironService.GrGus;
                }
                else
                    ListGrGusSRTR = _fSrtrToZwsironService.GrGus;

                // automatyczne przypisywanie GUS
                foreach (var item in ListGrGusSRTR)
                {
                    item.KodGrRodzZWSIRON = ListGrGusZWSIRON
                                                .Where(y => y.KodGrRodzZWSIRON.Trim() == item.KodGrRodzSRTR.PadRight(4, '0'))
                                                .Select(x => x.KodGrRodzZWSIRON)
                                                .FirstOrDefault();
                }
            }
            if (msg.MessageText.Equals("zapisz dane"))
            {
                _fSrtrToZwsironService.GrGus = ListGrGusSRTR;
                _fSrtrToZwsironService.AddGrupaGus();

                Messenger.Default.Send<Message, SrtrLoadWykazViewModel>(new Message("synchronizuj dane"));
            }
        }

        private void WczytajSlownikGrupGus()
        {
            GrupaGusPath = _fSrtrToZwsironService.OpenGrGusFile();
            if (!string.IsNullOrEmpty(GrupaGusPath))
            {
                try
                {
                    _fSrtrToZwsironService.LoadGrGusData(GrupaGusPath);      // Czytanie pliku
                    ListGrGusSRTR = null;
                    ListGrGusSRTR = _fSrtrToZwsironService.GrGus;

                    Messenger.Default.Send<Message, MainWizardViewModel>(new Message("Poprawnie zsynchronizowano plik z bazą danych."));    // komunikaty o statusie wczytania pliku
                }
                catch (Exception ex)
                {
                    string msg = string.Format("BŁĄD! - {0}", ex.Message);
                    MessageBox.Show(msg, "Bład odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void WyczyscDaneUzytkownikow()
        {
            List<GrupaRodzajowaGusSRTR> temp = new List<GrupaRodzajowaGusSRTR>();

            for (int i = 0; i < ListGrGusSRTR.Count; i++)
            {
                temp.Add(new GrupaRodzajowaGusSRTR
                {
                    KodGrRodzSRTR = ListGrGusSRTR[i].KodGrRodzSRTR,
                    NazwaGrRodzSRTR = ListGrGusSRTR[i].NazwaGrRodzSRTR
                });
            }

            ListGrGusSRTR.Clear();
            ListGrGusSRTR = temp;
        }

        internal override bool IsValid()
        {
            // sprawdzenie wypełnienia wszystkich Mpk oraz Id ZWSI RON
            if (ListGrGusSRTR != null)
            {
                bool isValid = ListGrGusSRTR.Exists(x => string.IsNullOrEmpty(x.KodGrRodzZWSIRON));
                return isValid ? false : true;
            }
            return false;
        }

        internal override string GetPageName()
        {
            return SRTRPages.SrtrGroupGus.ToString();
        }

        private void CallCleanUp(CleanUp cu)
        {
            if(GrupaGusPath != null) GrupaGusPath = string.Empty;
            ListGrGusSRTR = null;
            ListGrGusZWSIRON = null;
        }

        #endregion //Private Methods

        internal override void LoadData()
        {
            throw new NotImplementedException();
        }
    }
}
