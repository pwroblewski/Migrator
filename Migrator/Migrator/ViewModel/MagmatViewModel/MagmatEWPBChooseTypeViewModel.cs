using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Migrator.Services;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using System.Windows;
using Migrator.Model;

namespace Migrator.ViewModel.MagmatViewModel
{
    public class MagmatEWPBChooseTypeViewModel : MainWizardPageViewModelBase
    {
        #region Fields

        //private IFileMagmatEwpbService _fMagmatEwpbService;
        private IMAG_EWPBService _fMagEwpbService;

        string msgSucces = "Plik wczytano poprawnie.";

        #endregion

        #region Constructor

        public MagmatEWPBChooseTypeViewModel(IFileMagmatEwpbService fMagmatEwpbService, IMAG_EWPBService fMagEwpbService)
        {
             _fMagEwpbService = fMagEwpbService;

            WydrukLista = Enum.GetNames(typeof(MagmatEWPB)).Cast<string>().ToList();

            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion

        #region Properties
        private string _wydruk;
        public string Wydruk
        {
            get { return _wydruk; }
            set { _wydruk = value; RaisePropertyChanged(() => Wydruk); }
        }

        private List<string> _wydrukLista;
        public List<string> WydrukLista
        {
            get { return _wydrukLista; }
            set { _wydrukLista = value; RaisePropertyChanged(() => WydrukLista); }
        }

        private string _plikPath;
        public string PlikPath
        {
            get { return _plikPath; }
            set { _plikPath = value; RaisePropertyChanged(() => PlikPath); }
        }

        private string _domyslnyZaklad;
        public string DomyslnyZaklad
        {
            get { return _domyslnyZaklad; }
            set { _domyslnyZaklad = value; RaisePropertyChanged(() => DomyslnyZaklad); }
        }

        private string _domyslnySklad;
        public string DomyslnySklad
        {
            get { return _domyslnySklad; }
            set { _domyslnySklad = value; RaisePropertyChanged(() => DomyslnySklad); }
        }

        private List<MagmatEwpb> _listMaterialy;
        public List<MagmatEwpb> ListMaterialy
        {
            get { return _listMaterialy; }
            set { _listMaterialy = value; RaisePropertyChanged(() => ListMaterialy); }
        }
        #endregion

        #region Commands

        #region WczytajPlikCommand

        private RelayCommand<string> _wczytajPlikWydrukuCommand;
        public RelayCommand<string> WczytajPlikWydrukuCommand
        {
            get
            {
                return _wczytajPlikWydrukuCommand
                    ?? (_wczytajPlikWydrukuCommand = new RelayCommand<string>(
                        path => WczytajPlik(),
                        path => !string.IsNullOrEmpty(Wydruk)
                ));
            }
        }

        #endregion

        #endregion //Commands

        #region Methods

        private void HandleMessage(Message msg)
        {
            //if(msg.MessageText.Equals("zapisz dane"))
            //{
            //    //_fMagmatEwpbService.AddZakladSklad(DomyslnyZaklad, DomyslnySklad);
            //    Messenger.Default.Send<Message, MagmatEWPBFillDataViewModel>(new Message("synchronizuj dane"));
            //}
        }

        private void WczytajPlik()
        {
            try
            {
                PlikPath = _fMagEwpbService.OpenFile(Wydruk);
                if (!string.IsNullOrEmpty(PlikPath))
                {
                    _fMagEwpbService.LoadData(PlikPath);
                    ListMaterialy = _fMagEwpbService.Materialy;

                    Messenger.Default.Send<Message, MainWizardViewModel>(new Message(msgSucces));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Bład odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion //Methods

        private void CallCleanUp(CleanUp cu)
        {
            if (PlikPath != null) PlikPath = string.Empty;
            if (Wydruk != null) Wydruk = string.Empty;
            if (DomyslnyZaklad != null) DomyslnyZaklad = string.Empty;
            if (DomyslnySklad != null) DomyslnySklad = string.Empty;
            //_fMagmatEwpbService.Clean();
        }

        internal override bool IsValid()
        {
            if (!string.IsNullOrEmpty(PlikPath) && Wydruk!=null)
            {
                if (Wydruk.Equals("EWPB - 319/320") && (string.IsNullOrEmpty(DomyslnyZaklad) || string.IsNullOrEmpty(DomyslnySklad)))
                    return false;

                return true;
            }
            else
                return false;
        }

        internal override string GetPageName()
        {
            return Helpers.MagmatEWPBPages.MagChooseType.ToString();
        }

        internal override void LoadData()
        {
            throw new NotImplementedException();
        }
    }
}
