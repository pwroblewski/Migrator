using GalaSoft.MvvmLight.Command;
using Migrator.Model;
using Migrator.Services;
using Migrator.View.SRTRView;
using GalaSoft.MvvmLight.Messaging;
using Migrator.ViewModel.SRTRViewModel.Windows;
using System.Collections.Generic;
using Migrator.Helpers;
using System;
using System.Windows;

namespace Migrator.ViewModel.SRTRViewModel
{
    public class SrtrLoadFilesViewModel : MainWizardPageViewModelBase
    {
        #region Fields

        private KartotekaWindow kartotekaWindow;
        private readonly ISRTRService _fSrtrToZwsironService;
        private readonly IMAG_EWPBService _fMagmatService;
        private readonly IDBJmService _dbJmService;
        private readonly IDBGrupaAktywowService _dbGrupaAktywowService;
        private readonly IDBAmorService _dbAmorService;

        string msgSucces = "Plik wczytano poprawnie.";

        #endregion //Fields

        #region Constructor

        public SrtrLoadFilesViewModel(ISRTRService fSrtrToZwsironService, IMAG_EWPBService fMagmatService, IDBJmService dbJmService, IDBGrupaAktywowService dbGrupaAktywowService, IDBAmorService dbAmorService)
        {
            _fSrtrToZwsironService = fSrtrToZwsironService;
            _fMagmatService = fMagmatService;
            _dbJmService = dbJmService;
            _dbGrupaAktywowService = dbGrupaAktywowService;
            _dbAmorService = dbAmorService;

            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion //Constructor

        #region Commands

        #region WczytajPlikKartotekiCommand

        private RelayCommand<string> _wczytajPlikKartotekiCommand;
        public RelayCommand<string> WczytajPlikKartotekiCommand
        {
            get
            {
                return _wczytajPlikKartotekiCommand
                    ?? (_wczytajPlikKartotekiCommand = new RelayCommand<string>(
                        title => WczytajKartoteke(),
                        title => !string.IsNullOrEmpty(JednostkaGosp)
                ));
            }
        }

        #endregion

        #region WczytajPlikSlJedCommand

        private RelayCommand<string> _wczytajPlikSlJedCommand;
        public RelayCommand<string> WczytajPlikSlJedCommand
        {
            get
            {
                return _wczytajPlikSlJedCommand
                    ?? (_wczytajPlikSlJedCommand = new RelayCommand<string>(
                        title => WczytajSlJed(),
                        title => !string.IsNullOrEmpty(KartotekaPath)
                ));
            }
        }

        #endregion

        #region PokazKartotekeCommand

        private RelayCommand<string> _pokazKartotekeCommand;
        public RelayCommand<string> PokazKartotekeCommand
        {
            get
            {
                return _pokazKartotekeCommand
                    ?? (_pokazKartotekeCommand = new RelayCommand<string>(
                        title =>
                        {
                            kartotekaWindow = new KartotekaWindow();
                            Messenger.Default.Send<List<KartotekaSRTR>, KartotekaWindowViewModel>(ListKartoteka);

                            if (kartotekaWindow.ShowDialog() == true)
                            { };
                        },
                        m => ListKartoteka!=null && ListKartoteka.Count > 0
                ));
            }
        }

        #endregion

        #region PokazNiezlikwidowaneSrodkiCommand

        private RelayCommand<string> _pokazNiezlikwidowaneSrodkiCommand;
        public RelayCommand<string> PokazNiezlikwidowaneSrodkiCommand
        {
            get
            {
                return _pokazNiezlikwidowaneSrodkiCommand
                    ?? (_pokazNiezlikwidowaneSrodkiCommand = new RelayCommand<string>(
                        title =>
                        {
                            kartotekaWindow = new KartotekaWindow();
                            kartotekaWindow.Title = "Niezlikwidowane środki trwałe będące w magazynie";
                            Messenger.Default.Send<List<KartotekaSRTR>, KartotekaWindowViewModel>(ListNZlikKartoteka);

                            if (kartotekaWindow.ShowDialog() == true)
                            { };
                        },
                        m => ListNZlikKartoteka!=null && ListNZlikKartoteka.Count > 0
                ));
            }
        }

        #endregion

        #region ZapiszNiezlikwidowaneSrodkiCommand

        private RelayCommand<string> _zapiszNiezlikwidowaneSrodkiCommand;
        public RelayCommand<string> ZapiszNiezlikwidowaneSrodkiCommand
        {
            get
            {
                return _zapiszNiezlikwidowaneSrodkiCommand
                    ?? (_zapiszNiezlikwidowaneSrodkiCommand = new RelayCommand<string>(
                        title =>
                        {
                            string msg = _fSrtrToZwsironService.SaveNSTFile();

                            Messenger.Default.Send<Message, MainWizardViewModel>(new Message(msg));
                        },
                        m => ListNZlikKartoteka != null && ListNZlikKartoteka.Count > 0
                ));
            }
        }

        #endregion

        #endregion //Commands

        #region Properties

        private string _kartotekaPath;
        public string KartotekaPath
        {
            get { return _kartotekaPath; }
            set { _kartotekaPath = value; RaisePropertyChanged(() => KartotekaPath); }
        }
        private string _slJedPath;
        public string SlJedPath
        {
            get { return _slJedPath; }
            set { _slJedPath = value; RaisePropertyChanged(() => SlJedPath); }
        }
        private string _jednostkaGosp;
        public string JednostkaGosp
        {
            get { return _jednostkaGosp; }
            set { _jednostkaGosp = value; RaisePropertyChanged(() => JednostkaGosp); }
        }

        private List<KartotekaSRTR> _listKartoteka;
        public List<KartotekaSRTR> ListKartoteka
        {
            get { return _listKartoteka; }
            set { _listKartoteka = value; RaisePropertyChanged(() => ListKartoteka); }
        }

        private List<KartotekaSRTR> _listNZlikKartoteka;
        public List<KartotekaSRTR> ListNZlikKartoteka
        {
            get { return _listNZlikKartoteka; }
            set { _listNZlikKartoteka = value; RaisePropertyChanged(() => ListNZlikKartoteka); }
        }

        #endregion //Properties

        #region Methods
        
        private void WczytajKartoteke()
        {
            try
            {
                KartotekaPath = _fSrtrToZwsironService.OpenKartotekaFile();
                if (!string.IsNullOrEmpty(KartotekaPath))
                {
                    _fSrtrToZwsironService.LoadKartotekaData(KartotekaPath);
                    ListKartoteka = _fSrtrToZwsironService.Kartoteka;
                    ListNZlikKartoteka = _fSrtrToZwsironService.KartotekaZlik;

                    if (ListKartoteka.Count > 0)
                    {
                        // komunikaty o statusie wczytania pliku
                        Messenger.Default.Send<Message, MainWizardViewModel>(new Message(msgSucces));

                        // uzupełnij główny obiekt SrtrToZwsiron
                        _fSrtrToZwsironService.AddKartotekaFile(ListKartoteka);
                        _fSrtrToZwsironService.AddJednosktaGospodarcza(JednostkaGosp);

                        ConvertJednostkiMiary();
                        ConvertGrupaAktywow();
                        ConvertAmortyzacja();
                    }
                }
            }
            catch (Exception ex)
            {
                 MessageBox.Show(ex.InnerException.Message, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void WczytajSlJed()
        {
            try
            {
                _fMagmatService.TypWydruku = MagmatEWPB.EWPB_351;
                SlJedPath = _fMagmatService.OpenDictionaryFile();
                if (!string.IsNullOrEmpty(SlJedPath))
                {
                    var jednostki = _fMagmatService.LoadSlJedData(SlJedPath);
                    _fSrtrToZwsironService.AddSlJed(jednostki);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void ConvertJednostkiMiary()
        {
            List<JednostkaMiary> listJM = await _dbJmService.GetAll();
            _fSrtrToZwsironService.AddJednostkiMiary(listJM);
        }

        private async void ConvertGrupaAktywow()
        {
            List<GrupaAktywow> listGrAkt = await _dbGrupaAktywowService.GetAll();
            _fSrtrToZwsironService.AddGrupaAktywow(listGrAkt);
        }

        private async void ConvertAmortyzacja()
        {
            List<Amortyzacja> listAmor = await _dbAmorService.GetAll();
            _fSrtrToZwsironService.AddAmortyzacja(listAmor);
        }

        internal override bool IsValid()
        {
            if (ListKartoteka != null)
                return ListKartoteka.Count > 0;
            else
                return false;
        }

        internal override string GetPageName()
        {
            return SRTRPages.SrtrLoadKartoteka.ToString();
        }

        internal override void LoadData()
        {
            ListKartoteka = _fSrtrToZwsironService.Kartoteka;
            ListNZlikKartoteka = _fSrtrToZwsironService.KartotekaZlik;
        }

        private void CallCleanUp(CleanUp cu)
        {
            if (KartotekaPath != null) KartotekaPath = string.Empty;
            if (SlJedPath != null) SlJedPath = string.Empty;
            if (JednostkaGosp != null) JednostkaGosp = string.Empty;
            ListKartoteka = null;
            ListNZlikKartoteka = null;
        }

        #endregion // Methods
    }
}
