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

        private IMAG_EWPBService _fMagEwpbService;

        #endregion //Fields

        #region Constructor

        public MagmatEWPBJimDataViewModel(IMAG_EWPBService fMagEwpbService)
        {
            _fMagEwpbService = fMagEwpbService;

            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion //Constructor

        #region Properties

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
                ListMaterialy = _fMagEwpbService.Materialy;

                MsgToVisibility(_fMagEwpbService.TypWydruku);
            }
            if (msg.MessageText.Equals("zapisz dane"))
            {
                _fMagEwpbService.Materialy = ListMaterialy;
                _fMagEwpbService.AddJim();
                
                Messenger.Default.Send<Message, MagmatEWPBSigmatViewModel>(new Message("synchronizuj dane"));
            }
        }

        private void MsgToVisibility(MagmatEWPB typ)
        {
            switch (typ)
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
        }

        private void WczytajPlikJim()
        {
            WynikJimPath = _fMagEwpbService.OpenJimFile();
            if (!string.IsNullOrEmpty(WynikJimPath))
            {
                try
                {
                    // Czytanie pliku
                    _fMagEwpbService.AddJimData(WynikJimPath);
                    ListMaterialy = _fMagEwpbService.Materialy;

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
            string msg = _fMagEwpbService.SaveJimFile();

            Messenger.Default.Send<Message, MainWizardViewModel>(new Message(msg));
        }

        private void CallCleanUp(CleanUp cu)
        {
            ListMaterialy = null;
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
