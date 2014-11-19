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

namespace Migrator.ViewModel.SRTRViewModel
{
    public class SrtrJimViewModel : MainWizardPageViewModelBase
    {
        #region Fields

        private readonly IFileJimService _fJimService;
        private readonly ISRTRService _fSrtrToZwsironService;

        #endregion //Fields

        #region Constructor

        public SrtrJimViewModel(IFileJimService fJimService, ISRTRService fSrtrToZwsironService)
        {
            _fJimService = fJimService;
            _fSrtrToZwsironService = fSrtrToZwsironService;
            
            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion //Constructor

        #region Properties

        private List<WykazIlosciowy> _listWykazIlosciowySRTR;
        public List<WykazIlosciowy> ListWykazIlosciowySRTR
        {
            get { return _listWykazIlosciowySRTR; }
            set { _listWykazIlosciowySRTR = value; RaisePropertyChanged(() => ListWykazIlosciowySRTR); }
        }

        private string _wynikJimPath;
        public string WynikJimPath
        {
            get { return _wynikJimPath; }
            set { _wynikJimPath = value; RaisePropertyChanged(() => WynikJimPath); }
        }

        #endregion //Properties

        #region Commands

        #region UtworzPlikCommand

        private RelayCommand<string> _utworzPlikCommand;
        public RelayCommand<string> UtworzPlikCommand
        {
            get
            {
                return _utworzPlikCommand
                    ?? (_utworzPlikCommand = new RelayCommand<string>(
                        file => UtworzPlik()
                ));
            }
        }

        #endregion

        #region WczytajPlikJimCommand

        private RelayCommand<string> _wczytajPlikJimCommand;
        public RelayCommand<string> WczytajPlikJimCommand
        {
            get
            {
                return _wczytajPlikJimCommand
                    ?? (_wczytajPlikJimCommand = new RelayCommand<string>(
                        file => WczytajPlikJim()
                ));
            }
        }

        #endregion

        #endregion //Commands

        #region Private Methods

        private void HandleMessage(Message msg)
        {
            if (msg.MessageText.Equals("synchronizuj dane"))
            {
                ListWykazIlosciowySRTR = (List<WykazIlosciowy>)msg.MessageObject;
            }
            if (msg.MessageText.Equals("zapisz dane"))
            {
                _fSrtrToZwsironService.AddWykaz(ListWykazIlosciowySRTR);
                Messenger.Default.Send<Message, SrtrPlikWynikowyViewModel>(new Message("synchronizuj dane"));
            }
        }

        private void WczytajPlikJim()
        {
            WynikJimPath = _fJimService.OpenFileDialog();
            if (!string.IsNullOrEmpty(WynikJimPath))
            {
                try
                {
                    // Czytanie pliku
                    var list = _fJimService.AddJimData(WynikJimPath, ListWykazIlosciowySRTR);
                    ListWykazIlosciowySRTR = null;
                    ListWykazIlosciowySRTR = list;

                    Messenger.Default.Send<Message, MainWizardViewModel>(new Message("Plik wczytano poprawnie."));
                }
                catch (Exception ex)
                {
                    string msg = string.Format("BŁĄD! - {0}", ex.Message);
                    MessageBox.Show(msg, "Bład odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UtworzPlik()
        {
            string msg = _fJimService.SaveFileDialog(ListWykazIlosciowySRTR);
            
            Messenger.Default.Send<Message, MainWizardViewModel>(new Message(msg));
        }

        internal override bool IsValid()
        {
            // sprawdzenie wypełnienia wszystkich Mpk oraz Id ZWSI RON
            if (ListWykazIlosciowySRTR != null)
            {
                bool isValid = ListWykazIlosciowySRTR.Exists(x => string.IsNullOrEmpty(x.Zaklad));
                return isValid ? false : true;
            }
            else
            {
                return false;
            }
        }

        internal override string GetPageName()
        {
            return SRTRPages.SrtrJim.ToString();
        } 

        private void CallCleanUp(CleanUp cu)
        {
            if (WynikJimPath != null) WynikJimPath = string.Empty;
            if (ListWykazIlosciowySRTR != null) ListWykazIlosciowySRTR.Clear();
        }

        #endregion //Private Methods

        
    }
}
