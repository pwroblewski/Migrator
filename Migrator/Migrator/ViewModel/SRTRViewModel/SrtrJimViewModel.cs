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

        private readonly ISRTRService _fSrtrToZwsironService;

        #endregion //Fields

        #region Constructor

        public SrtrJimViewModel(ISRTRService fSrtrToZwsironService)
        {
            //_fJimService = fJimService;
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
                ListWykazIlosciowySRTR = _fSrtrToZwsironService.Wykaz;
            }
            if (msg.MessageText.Equals("zapisz dane"))
            {
                _fSrtrToZwsironService.Wykaz = ListWykazIlosciowySRTR;
                _fSrtrToZwsironService.AddWykaz();

                Messenger.Default.Send<Message, SrtrPlikWynikowyViewModel>(new Message("synchronizuj dane"));
            }
        }

        private void WczytajPlikJim()
        {
            WynikJimPath = _fSrtrToZwsironService.OpenJimFile();
            if (!string.IsNullOrEmpty(WynikJimPath))
            {
                try
                {
                    // Czytanie pliku
                    _fSrtrToZwsironService.AddJimData(WynikJimPath);
                    ListWykazIlosciowySRTR = null;
                    ListWykazIlosciowySRTR = _fSrtrToZwsironService.Wykaz;

                    Messenger.Default.Send<Message, MainWizardViewModel>(new Message("Plik wczytano poprawnie."));
                }
                catch (ArgumentOutOfRangeException)
                {
                    string msg = "Wykryto niepoprawną strukturę pliku.";
                    MessageBox.Show(msg, "Bład odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception)
                {
                    string msg = string.Format("Nieznany błąd. Sprawdź poprawność danych lub skontaktuj się z twórcą programu.");
                    MessageBox.Show(msg, "Bład odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UtworzPlik()
        {
            string msg = _fSrtrToZwsironService.SaveJimFile();

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
            ListWykazIlosciowySRTR = null;
        }

        #endregion //Private Methods



        internal override void LoadData()
        {
            throw new NotImplementedException();
        }
    }
}
