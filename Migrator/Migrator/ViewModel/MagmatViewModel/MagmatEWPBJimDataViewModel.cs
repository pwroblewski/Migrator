using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using Migrator.Model;
using Migrator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Migrator.ViewModel.MagmatViewModel
{
    public class MagmatEWPBJimDataViewModel : MainWizardPageViewModelBase
    {
        #region Fields

        private IMAG_EWPBService _fSigmatService;
        private IFileJimService _fJimService;

        #endregion //Fields

        #region Constructor

        public MagmatEWPBJimDataViewModel(IMAG_EWPBService fSigmatService, IFileJimService fJimService)
        {
            _fSigmatService = fSigmatService;
            _fJimService = fJimService;

            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion //Constructor

        #region Properties

        private MagmatEWPB _typWydruku;
        public MagmatEWPB TypWydruku
        {
            get { return _typWydruku; }
            set { _typWydruku = value; RaisePropertyChanged(() => TypWydruku); }
        }

        private List<MagmatEwpb> _listMaterialy;
        public List<MagmatEwpb> ListMaterialy
        {
            get { return _listMaterialy; }
            set { _listMaterialy = value; RaisePropertyChanged(() => ListMaterialy); }
        }

        private string _wynikJimPath;
        public string WynikJimPath
        {
            get { return _wynikJimPath; }
            set { _wynikJimPath = value; RaisePropertyChanged(() => WynikJimPath); }
        }

        private bool _wartoscVis;
        public bool WartoscVis
        {
            get { return _wartoscVis; }
            set { _wartoscVis = value; RaisePropertyChanged(() => WartoscVis); }
        }

        private bool _cenaVis;
        public bool CenaVis
        {
            get { return _cenaVis; }
            set { _cenaVis = value; RaisePropertyChanged(() => CenaVis); }
        }

        private bool _userVis;
        public bool UserVis
        {
            get { return _userVis; }
            set { _userVis = value; RaisePropertyChanged(() => UserVis); }
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
                TypWydruku = (MagmatEWPB)msg.MessageObject;

                switch (TypWydruku)
                {
                    case MagmatEWPB.Magmat_305:
                        WartoscVis = true;
                        CenaVis = true;
                        UserVis = false;
                        break;
                    case MagmatEWPB.EWPB_319_320:
                        WartoscVis = false;
                        CenaVis = false;
                        UserVis = true;
                        break;
                    case MagmatEWPB.EWPB_351:
                        WartoscVis = true;
                        UserVis = false;
                        CenaVis = false;
                        break;
                    default:
                        break;
                }

                if(ListMaterialy == null)
                    ListMaterialy = _fSigmatService.GetMaterialy();
            }
            if (msg.MessageText.Equals("zapisz dane"))
            {
                _fSigmatService.AddJim(ListMaterialy, TypWydruku);
                Messenger.Default.Send<Message, MagmatEWPBSigmatViewModel>(new Message("synchronizuj dane", TypWydruku));
            }
        }

        private void WczytajPlikJim()
        {
            WynikJimPath = _fJimService.OpenFileDialog();
            if (!string.IsNullOrEmpty(WynikJimPath))
            {
                try
                {
                    ListMaterialy = _fJimService.AddJimData(WynikJimPath, ListMaterialy);

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
            string msg = _fJimService.SaveFileDialog(ListMaterialy);

            Messenger.Default.Send<Message, MainWizardViewModel>(new Message(msg));
        }

        private void CallCleanUp(CleanUp cu)
        {
            if (ListMaterialy != null) ListMaterialy.Clear();
        }

        #endregion //Methods

        internal override bool IsValid()
        {
            return true;
        }

        internal override string GetPageName()
        {
            return MagmatEWPBPages.MagJimData.ToString();
        }

        internal override void LoadData()
        {
            throw new NotImplementedException();
        }
    }
}
