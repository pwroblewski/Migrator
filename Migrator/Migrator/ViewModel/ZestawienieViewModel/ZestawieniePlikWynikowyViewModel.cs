using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using Migrator.Model;
using Migrator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrator.ViewModel.ZestawienieViewModel
{
    public class ZestawieniePlikWynikowyViewModel : MainWizardPageViewModelBase
    {
        #region Fields

        private IFileZestawienieService _fZestawienieService;

        #endregion //Fields

        #region Constructor

        public ZestawieniePlikWynikowyViewModel(IFileZestawienieService fZestawienieService)
        {
            _fZestawienieService = fZestawienieService;

            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
            Messenger.Default.Register<Message>(this, HandleMessage);
        }

        #endregion //Constructor

        #region Properties

        private List<ZestawienieKlas> _listZestawienieKlas;
        public List<ZestawienieKlas> ListZestawienieKlas
        {
            get { return _listZestawienieKlas; }
            set { _listZestawienieKlas = value; RaisePropertyChanged(() => ListZestawienieKlas); }
        }

        #endregion

        #region Commands

        #region UtworzPlikCommand

        private RelayCommand<string> _zapiszPlikiCommand;
        public RelayCommand<string> ZapiszPlikiCommand
        {
            get
            {
                return _zapiszPlikiCommand
                    ?? (_zapiszPlikiCommand = new RelayCommand<string>(
                        file => _fZestawienieService.ZapiszPliki()
                ));
            }
        }

        #endregion

        #endregion

        #region Methods

        private void HandleMessage(Message msg)
        {
            if (msg.MessageText.Equals("synchronizuj dane"))
                ListZestawienieKlas = _fZestawienieService.GetZestawienieKlas();
        }

        private void CallCleanUp(CleanUp cu)
        {
            if (ListZestawienieKlas != null) ListZestawienieKlas.Clear();
        }

        #endregion

        internal override bool IsValid()
        {
            return true; ;
        }

        internal override string GetPageName()
        {
            return ZestawieniePages.ZestawieniePlikWynikowy.ToString();
        }
    }
}
