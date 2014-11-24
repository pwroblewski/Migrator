using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using Migrator.Model;
using Migrator.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Migrator.ViewModel.MagmatViewModel
{
    public class MagmatEWPBSigmatViewModel : MainWizardPageViewModelBase
    {
        #region Fields

        private IMAG_EWPBService _fMagEwpbService;

        #endregion //Fields

        #region Constructor

        public MagmatEWPBSigmatViewModel(IMAG_EWPBService fMagEwpbService)
        {
            _fMagEwpbService = fMagEwpbService;

            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion //Constructor

        #region Properties
        private TabItem _selectedItem;
        public TabItem SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; RaisePropertyChanged(() => SelectedItem); }
        }

        private List<SigmatKat> _listKat;
        public List<SigmatKat> ListKat
        {
            get { return _listKat; }
            set { _listKat = value; RaisePropertyChanged(() => ListKat); }
        }

        private List<SigmatAmunicja> _listAmunicja;
        public List<SigmatAmunicja> ListAmunicja
        {
            get { return _listAmunicja; }
            set { _listAmunicja = value; RaisePropertyChanged(() => ListAmunicja); }
        }

        private List<SigmatMund> _listMund;
        public List<SigmatMund> ListMund
        {
            get { return _listMund; }
            set { _listMund = value; RaisePropertyChanged(() => ListMund); }
        }

        private List<SigmatPaliwa> _listPaliwa;
        public List<SigmatPaliwa> ListPaliwa
        {
            get { return _listPaliwa; }
            set { _listPaliwa = value; RaisePropertyChanged(() => ListPaliwa); }
        }

        private List<SigmatZywnosc> _listZywnosc;
        public List<SigmatZywnosc> ListZywnosc
        {
            get { return _listZywnosc; }
            set { _listZywnosc = value; RaisePropertyChanged(() => ListZywnosc); }
        }

        private bool _userVis;
        public bool UserVis
        {
            get { return _userVis; }
            set { _userVis = value; RaisePropertyChanged(() => UserVis); }
        }
        
        private bool _userNotVis;
        public bool UserNotVis
        {
            get { return _userNotVis; }
            set { _userNotVis = value; RaisePropertyChanged(() => UserNotVis); }
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

        #endregion //Commands

        #region Methods

        private void HandleMessage(Message msg)
        {
            if (msg.MessageText.Equals("synchronizuj dane"))
            {
                MsgToVisible(_fMagEwpbService.TypWydruku);

                ListAmunicja = _fMagEwpbService.Amunicja;
                ListKat = _fMagEwpbService.Kat;
                ListPaliwa = _fMagEwpbService.Paliwa;
                ListMund = _fMagEwpbService.Mund;
                ListZywnosc = _fMagEwpbService.Zywnosc;
            }
            if(msg.MessageText.Equals("zapisz dane"))
            {
                _fMagEwpbService.Amunicja = ListAmunicja;
                _fMagEwpbService.Kat = ListKat;
                _fMagEwpbService.Paliwa = ListPaliwa;
                _fMagEwpbService.Mund = ListMund;
                _fMagEwpbService.Zywnosc = ListZywnosc;
            }
        }

        private void MsgToVisible(MagmatEWPB typ)
        {
            switch (typ)
            {
                case MagmatEWPB.Magmat_305:
                    UserVis = false;
                    UserNotVis = true;
                    break;
                case MagmatEWPB.EWPB_319_320:
                    UserVis = true;
                    UserNotVis = false;
                    break;
                case MagmatEWPB.EWPB_351:
                    UserVis = false;
                    UserNotVis = true;
                    break;
                default:
                    break;
            }
        }

        private void UtworzPlik()
        {
            try
            {
                _fMagEwpbService.SaveFile(SelectedItem.Header.ToString());

                Messenger.Default.Send<Message, MainWizardViewModel>(new Message("Plik zapisano poprawnie."));
            }
            catch(Exception ex)
            {
                MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {

            }
        }

        private void CallCleanUp(CleanUp cu)
        {
            if (ListAmunicja != null) ListAmunicja.Clear();
            if (ListKat != null) ListKat.Clear();
            if (ListMund != null) ListMund.Clear();
            if (ListPaliwa != null) ListPaliwa.Clear();
            if (ListZywnosc != null) ListZywnosc.Clear();
        }

        #endregion //Methods

        internal override bool IsValid()
        {
            return true;
        }

        internal override string GetPageName()
        {
            return MagmatEWPBPages.MagSigmat.ToString();
        }

        internal override void LoadData()
        {
            throw new NotImplementedException();
        }
    }
}
