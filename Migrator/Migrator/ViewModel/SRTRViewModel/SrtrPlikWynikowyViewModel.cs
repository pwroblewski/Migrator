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
    public class SrtrPlikWynikowyViewModel : MainWizardPageViewModelBase
    {       
        #region Fields

        private readonly ISRTRService _fSrtrToZwsironService;
        
        #endregion //Fields

        #region Constructor

        public SrtrPlikWynikowyViewModel(ISRTRService fSrtrToZwsironService)
        {
            _fSrtrToZwsironService = fSrtrToZwsironService;

            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion //Constructor

        #region Properties

        private List<SrtrToZwsiron> _listSrtrToZwsiron;
        public List<SrtrToZwsiron> ListSrtrToZwsiron
        {
            get { return _listSrtrToZwsiron; }
            set { _listSrtrToZwsiron = value; RaisePropertyChanged(() => ListSrtrToZwsiron); }
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

        #endregion //Commands

        #region Private Methods

        private void HandleMessage(Message msg)
        {
            if (msg.MessageText.Equals("synchronizuj dane"))
            {
                ListSrtrToZwsiron = _fSrtrToZwsironService.SrtrToZwsiron;
            }
        }

        private void UtworzPlik()
        {
            string msg = _fSrtrToZwsironService.SaveFile();
            
            Messenger.Default.Send<Message, MainWizardViewModel>(new Message(msg));
        }
        internal override bool IsValid()
        {
            return true;
        }

        internal override string GetPageName()
        {
            return SRTRPages.SrtrPlikWynikowy.ToString();
        }

        private void CallCleanUp(CleanUp cu)
        {
            if (ListSrtrToZwsiron != null) ListSrtrToZwsiron.Clear();
            _fSrtrToZwsironService.Clean();
        }

        #endregion

        internal override void LoadData()
        {
            throw new NotImplementedException();
        }
    }
}
