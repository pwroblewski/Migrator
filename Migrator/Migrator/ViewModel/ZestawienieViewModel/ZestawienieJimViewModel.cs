using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using Migrator.Model;
using Migrator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Migrator.ViewModel.ZestawienieViewModel
{
    public class ZestawienieJimViewModel : MainWizardPageViewModelBase
    {
        #region Fields

        private IZestawienieService _fZestawienieService;

        #endregion //Fields

        #region Constructor

        public ZestawienieJimViewModel(IZestawienieService fZestawienieService)
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

        private string _jimPath;
        public string JimPath
        {
            get { return _jimPath; }
            set { _jimPath = value; RaisePropertyChanged(() => JimPath); }
        }

        #endregion

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

        #endregion

        #region Methods

        private void HandleMessage(Message msg)
        {
            if (msg.MessageText.Equals("synchronizuj dane"))
            {
                ListZestawienieKlas = _fZestawienieService.ZestawieniaKlas;
            }
            if (msg.MessageText.Equals("zapisz dane"))
            {
                _fZestawienieService.ZestawieniaKlas = ListZestawienieKlas;

                Messenger.Default.Send<Message, ZestawieniePlikWynikowyViewModel>(new Message("synchronizuj dane"));
            }
        }

        private void WczytajPlikJim()
        {
            JimPath = _fZestawienieService.OpenJimFile();
            if (!string.IsNullOrEmpty(JimPath))
            {
                try
                {
                    // Czytanie pliku
                    _fZestawienieService.AddJimData(JimPath);
                    ListZestawienieKlas = _fZestawienieService.ZestawieniaKlas;

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
            string msg = _fZestawienieService.SaveJimFile();

            Messenger.Default.Send<Message, MainWizardViewModel>(new Message(msg));
        }

        private void CallCleanUp(CleanUp cu)
        {
            if (ListZestawienieKlas != null) ListZestawienieKlas.Clear();
        }

        #endregion

        internal override bool IsValid()
        {
            return true;
        }

        internal override string GetPageName()
        {
            return ZestawieniePages.ZestawienieJimData.ToString();
        }

        internal override void LoadData()
        {
            throw new NotImplementedException();
        }
    }
}
