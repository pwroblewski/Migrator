using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using Migrator.Helpers;
using Migrator.Model.State;
using Migrator.Services;
using Migrator.ViewModel.MagmatViewModel;
using Migrator.ViewModel.SRTRViewModel;
using Migrator.ViewModel.ZestawienieViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Migrator.ViewModel
{
    public class MainWizardViewModel : ViewModelBase
    {
        #region Fields
        private ISRTRService _fSrtrToZwsironService;
        private IFileMagmatEwpbService _fMagmatSrtrService;
        private IFileSigmatService _fSigmatService;
        private IFileZestawienieService _fZestawienieService;
        #endregion //Fields

        #region Constructor

        public MainWizardViewModel(ISRTRService fSrtrToZwsironService, IFileMagmatEwpbService fMagmatEwpbService, IFileSigmatService fSigmatService, IFileZestawienieService fZestawienieService)
        {
            this.CurrentPage = this.Pages[0];
            _fSrtrToZwsironService = fSrtrToZwsironService;
            _fMagmatSrtrService = fMagmatEwpbService;
            _fSigmatService = fSigmatService;
            _fZestawienieService = fZestawienieService;

            Messenger.Default.Register<Modul>(this, CreatePages);
            Messenger.Default.Register<Message>(this, PokazKomunikat);
        }

        #endregion //Constructor

        #region Properties

        private MainWizardPageViewModelBase _currentPage;
        public MainWizardPageViewModelBase CurrentPage
        {
            get { return _currentPage; }
            private set
            {
                if (value == _currentPage)
                    return;

                if (_currentPage != null)
                    _currentPage.IsCurrentPage = false;

                _currentPage = value;

                if (_currentPage != null)
                    _currentPage.IsCurrentPage = true;

                RaisePropertyChanged("CurrentPage");
                RaisePropertyChanged("IsOnLastPage");
            }
        }

        // Zmienia opis buttona Next
        public string IsOnLastPage
        {
            get
            {
                if (this.CurrentPageIndex == this.Pages.Count - 1)
                    return "Zakończ";
                else
                    return "Dalej";
            }
        }

        private ReadOnlyCollection<MainWizardPageViewModelBase> _pages;
        public ReadOnlyCollection<MainWizardPageViewModelBase> Pages
        {
            get
            {
                if (_pages == null)
                {
                    var pages = new List<MainWizardPageViewModelBase>();
                    pages.Add(null);
                    _pages = new ReadOnlyCollection<MainWizardPageViewModelBase>(pages);
                }

                return _pages;
            }
        }

        // Pasek komunikatu
        private string _komunikat;
        public string Komunikat
        {
            get { return _komunikat; }
            set { _komunikat = value; RaisePropertyChanged(() => Komunikat); }
        }

        #endregion // Properties

        #region Commands

        #region Save/Load Project Command

        private RelayCommand _saveProjectCommand;
        public RelayCommand SaveProjectCommand
        {
            get
            {
                return _saveProjectCommand
                    ?? (_saveProjectCommand = new RelayCommand(
                        () => SaveProject(),
                        () => CanSaveProject()
                ));
            }
        }

        private RelayCommand _loadProjectCommand;
        public RelayCommand LoadProjectCommand
        {
            get
            {
                return _loadProjectCommand
                    ?? (_loadProjectCommand = new RelayCommand(
                        () => LoadProject(),
                        () => CanLoadProject()
                ));
            }
        }

        private void SaveProject()
        {
            // Mega chujowy kod, ale burdel w MON mnie do tego zmusił
            // Praca w MON jest do dupy
            SrtrState st;
            MagmatEwpbState magSt;
            ZestawienieState zestSt;
            switch (this.CurrentPage.GetPageName())
            {
                #region SRTR
                case "SrtrUserConversion":
                    st = new SrtrState
                    {
                        ListWynik = _fSrtrToZwsironService.GetSrtrToZwsiron(),
                        ListUzytkownik = ViewModelLocator.SrtrUsersConversionViewModel.ListUzytkownicy,
                        ListKartoteka = ViewModelLocator.SrtrLoadFilesViewModel.ListKartoteka,
                        ListNKartoteka = ViewModelLocator.SrtrLoadFilesViewModel.ListNZlikKartoteka,
                        ViewName = "SrtrUserConversion"
                    };
                    SaveSrtrProjectFile<SrtrState>(st);
                    break;
                case "SrtrGroupGus":
                    st = new SrtrState
                    {
                        ListWynik = _fSrtrToZwsironService.GetSrtrToZwsiron(),
                        ListGrupaGus = ViewModelLocator.SrtrGroupGusViewModel.ListGrGusSRTR,
                        ListUzytkownik = ViewModelLocator.SrtrUsersConversionViewModel.ListUzytkownicy,
                        ListKartoteka = ViewModelLocator.SrtrLoadFilesViewModel.ListKartoteka,
                        ListNKartoteka = ViewModelLocator.SrtrLoadFilesViewModel.ListNZlikKartoteka,
                        ViewName = "SrtrGroupGus"
                    };
                    SaveSrtrProjectFile<SrtrState>(st);
                    break;
                case "SrtrWykaz":
                    st = new SrtrState
                    {
                        ListWynik = _fSrtrToZwsironService.GetSrtrToZwsiron(),
                        ListWykazIlosciowy = ViewModelLocator.SrtrLoadWykazViewModel.ListWykazIlosciowySRTR,
                        ListGrupaGus = ViewModelLocator.SrtrGroupGusViewModel.ListGrGusSRTR,
                        ListUzytkownik = ViewModelLocator.SrtrUsersConversionViewModel.ListUzytkownicy,
                        ListKartoteka = ViewModelLocator.SrtrLoadFilesViewModel.ListKartoteka,
                        ListNKartoteka = ViewModelLocator.SrtrLoadFilesViewModel.ListNZlikKartoteka,
                        ViewName = "SrtrWykaz"
                    };
                    SaveSrtrProjectFile<SrtrState>(st);
                    break;
                case "SrtrJim":
                    st = new SrtrState
                    {
                        ListWynik = _fSrtrToZwsironService.GetSrtrToZwsiron(),
                        ListWykazIlosciowy = ViewModelLocator.SrtrJimViewModel.ListWykazIlosciowySRTR,
                        ListGrupaGus = ViewModelLocator.SrtrGroupGusViewModel.ListGrGusSRTR,
                        ListUzytkownik = ViewModelLocator.SrtrUsersConversionViewModel.ListUzytkownicy,
                        ListKartoteka = ViewModelLocator.SrtrLoadFilesViewModel.ListKartoteka,
                        ListNKartoteka = ViewModelLocator.SrtrLoadFilesViewModel.ListNZlikKartoteka,
                        ViewName = "SrtrJim"
                    };
                    SaveSrtrProjectFile<SrtrState>(st);
                    break;
                #endregion
                #region MAGMAT EWPB
                case "MagFillData":
                    magSt = new MagmatEwpbState
                    {
                        ViewName = "MagFillData",
                        MagmatEwpbService = _fMagmatSrtrService.GetData(),
                        ListData = ViewModelLocator.MagmatEWPBFillDataViewModel.ListMaterialy,
                        TypWydruku = ViewModelLocator.MagmatEWPBFillDataViewModel.TypWydruku,
                        Wydruk = ViewModelLocator.MagmatEWPBChooseTypeViewModel.Wydruk
                    };
                    SaveSrtrProjectFile<MagmatEwpbState>(magSt);
                    break;
                case "MagFillDictionary":
                    magSt = new MagmatEwpbState
                    {
                        ViewName = "MagFillDictionary",
                        MagmatEwpbService = _fMagmatSrtrService.GetData(),
                        ListSigmat = _fSigmatService.GetMaterialy(),
                        ListDictionary = ViewModelLocator.MagmatEWPBFillDictionaryViewModel.ListMaterialy,
                        ListData = ViewModelLocator.MagmatEWPBFillDataViewModel.ListMaterialy,
                        TypWydruku = ViewModelLocator.MagmatEWPBFillDataViewModel.TypWydruku,
                        Wydruk = ViewModelLocator.MagmatEWPBChooseTypeViewModel.Wydruk
                    };
                    SaveSrtrProjectFile<MagmatEwpbState>(magSt);
                    break;
                case "MagJimData":
                    magSt = new MagmatEwpbState
                    {
                        ViewName = "MagJimData",
                        MagmatEwpbService = _fMagmatSrtrService.GetData(),
                        ListSigmat = _fSigmatService.GetMaterialy(),
                        ListDictionary = ViewModelLocator.MagmatEWPBFillDictionaryViewModel.ListMaterialy,
                        ListData = ViewModelLocator.MagmatEWPBFillDataViewModel.ListMaterialy,
                        TypWydruku = ViewModelLocator.MagmatEWPBFillDataViewModel.TypWydruku,
                        Wydruk = ViewModelLocator.MagmatEWPBChooseTypeViewModel.Wydruk,
                        ListAmunicja = _fSigmatService.GetAmunicja(),
                        ListPaliwa = _fSigmatService.GetPaliwa(),
                        ListKat = _fSigmatService.GetKat(),
                        ListMund = _fSigmatService.GetMund(),
                        ListZywnosc = _fSigmatService.GetZywnosc(),
                        ListJim = ViewModelLocator.MagmatEWPBJimDataViewModel.ListMaterialy
                    };
                    SaveSrtrProjectFile<MagmatEwpbState>(magSt);
                    break;
                #endregion
                #region ZESTAWIENIE
                case "ZestawienieLoadFiles":
                    zestSt = new ZestawienieState
                    {
                        ViewName = "ZestawienieLoadFiles",
                        ListZestawienie = ViewModelLocator.ZestawienieLoadFilesViewModel.ListZestawienie,
                        ListZestService = _fZestawienieService.GetZestawienie(),
                        ListZestKlasService = _fZestawienieService.GetZestawienieKlas()
                    };
                    SaveSrtrProjectFile<ZestawienieState>(zestSt);
                    break;
                case "ZestawienieJimData":
                    zestSt = new ZestawienieState
                    {
                        ViewName = "ZestawienieJimData",
                        ListZestawienie = ViewModelLocator.ZestawienieLoadFilesViewModel.ListZestawienie,
                        ListZestService = _fZestawienieService.GetZestawienie(),
                        ListZestKlasService = _fZestawienieService.GetZestawienieKlas()
                    };
                    SaveSrtrProjectFile<ZestawienieState>(zestSt);
                    break;
                #endregion
            }
        }

        private void LoadProject()
        {
            if (this.CurrentPage != null)
            {
                switch (this.CurrentPage.GetPageName())
                {
                    case "SrtrLoadKartoteka":
                        var srtr = LoadProjectFile<SrtrState>();
                        LoadSrtr(srtr);
                        break;
                    case "MagChooseType":
                        var magmat = LoadProjectFile<MagmatEwpbState>();
                        LoadMagmatEwpb(magmat);
                        break;
                    case "ZestawienieLoadFiles":
                        var zestawienie = LoadProjectFile<ZestawienieState>();
                        LoadZestawienie(zestawienie);
                        break;
                }
            }


        }

        private void LoadSrtr(SrtrState stan)
        {
            if (stan != null)
            {
                switch (stan.ViewName)
                {
                    #region SRTR
                    case "SrtrUserConversion":
                        ViewModelLocator.SrtrUsersConversionViewModel.ListUzytkownicy = stan.ListUzytkownik;
                        ViewModelLocator.SrtrLoadFilesViewModel.ListKartoteka = stan.ListKartoteka;
                        ViewModelLocator.SrtrLoadFilesViewModel.ListNZlikKartoteka = stan.ListNKartoteka;
                        _fSrtrToZwsironService.SetSrtrToZwsiron(stan.ListWynik);
                        break;

                    case "SrtrGroupGus":
                        ViewModelLocator.SrtrGroupGusViewModel.ListGrGusSRTR = stan.ListGrupaGus;
                        ViewModelLocator.SrtrUsersConversionViewModel.ListUzytkownicy = stan.ListUzytkownik;
                        ViewModelLocator.SrtrLoadFilesViewModel.ListKartoteka = stan.ListKartoteka;
                        ViewModelLocator.SrtrLoadFilesViewModel.ListNZlikKartoteka = stan.ListNKartoteka;
                        _fSrtrToZwsironService.SetSrtrToZwsiron(stan.ListWynik);
                        break;
                    case "SrtrWykaz":
                        ViewModelLocator.SrtrLoadWykazViewModel.ListWykazIlosciowySRTR = stan.ListWykazIlosciowy;
                        ViewModelLocator.SrtrGroupGusViewModel.ListGrGusSRTR = stan.ListGrupaGus;
                        ViewModelLocator.SrtrUsersConversionViewModel.ListUzytkownicy = stan.ListUzytkownik;
                        ViewModelLocator.SrtrLoadFilesViewModel.ListKartoteka = stan.ListKartoteka;
                        ViewModelLocator.SrtrLoadFilesViewModel.ListNZlikKartoteka = stan.ListNKartoteka;
                        _fSrtrToZwsironService.SetSrtrToZwsiron(stan.ListWynik);
                        break;
                    case "SrtrJim":
                        ViewModelLocator.SrtrJimViewModel.ListWykazIlosciowySRTR = stan.ListWykazIlosciowy;
                        ViewModelLocator.SrtrLoadWykazViewModel.ListWykazIlosciowySRTR = stan.ListWykazIlosciowy;
                        ViewModelLocator.SrtrGroupGusViewModel.ListGrGusSRTR = stan.ListGrupaGus;
                        ViewModelLocator.SrtrUsersConversionViewModel.ListUzytkownicy = stan.ListUzytkownik;
                        ViewModelLocator.SrtrLoadFilesViewModel.ListKartoteka = stan.ListKartoteka;
                        ViewModelLocator.SrtrLoadFilesViewModel.ListNZlikKartoteka = stan.ListNKartoteka;
                        _fSrtrToZwsironService.SetSrtrToZwsiron(stan.ListWynik);
                        break;
                    #endregion
                }
            }
        }
        private void LoadMagmatEwpb(MagmatEwpbState stan)
        {
            if (stan != null)
            {
                switch (stan.ViewName)
                {
                    #region MAGMATEWPB
                    case "MagFillData":
                        ViewModelLocator.MagmatEWPBChooseTypeViewModel.PlikPath = " ";
                        ViewModelLocator.MagmatEWPBChooseTypeViewModel.Wydruk = stan.Wydruk;
                        ViewModelLocator.MagmatEWPBFillDataViewModel.TypWydruku = stan.TypWydruku;
                        ViewModelLocator.MagmatEWPBFillDataViewModel.ListMaterialy = stan.ListData;
                        _fMagmatSrtrService.SetData(stan.MagmatEwpbService);
                        break;
                    case "MagFillDictionary":
                        ViewModelLocator.MagmatEWPBChooseTypeViewModel.PlikPath = " ";
                        ViewModelLocator.MagmatEWPBChooseTypeViewModel.Wydruk = stan.Wydruk;
                        ViewModelLocator.MagmatEWPBFillDataViewModel.TypWydruku = stan.TypWydruku;
                        ViewModelLocator.MagmatEWPBFillDataViewModel.ListMaterialy = stan.ListData;
                        _fMagmatSrtrService.SetData(stan.MagmatEwpbService);
                        ViewModelLocator.MagmatEWPBFillDictionaryViewModel.ListMaterialy = stan.ListDictionary;
                        _fSigmatService.AddMaterial(stan.ListSigmat);
                        break;
                    case "MagJimData":
                        ViewModelLocator.MagmatEWPBChooseTypeViewModel.PlikPath = " ";
                        ViewModelLocator.MagmatEWPBChooseTypeViewModel.Wydruk = stan.Wydruk;
                        ViewModelLocator.MagmatEWPBFillDataViewModel.TypWydruku = stan.TypWydruku;
                        ViewModelLocator.MagmatEWPBFillDataViewModel.ListMaterialy = stan.ListData;
                        _fMagmatSrtrService.SetData(stan.MagmatEwpbService);
                        ViewModelLocator.MagmatEWPBFillDictionaryViewModel.ListMaterialy = stan.ListDictionary;
                        _fSigmatService.AddMaterial(stan.ListSigmat);
                        ViewModelLocator.MagmatEWPBJimDataViewModel.ListMaterialy = stan.ListJim;
                        _fSigmatService.SetKat(stan.ListKat);
                        _fSigmatService.SetPaliwa(stan.ListPaliwa);
                        _fSigmatService.SetZywnosc(stan.ListZywnosc);
                        _fSigmatService.SetMund(stan.ListMund);
                        _fSigmatService.SetAmunicja(stan.ListAmunicja);
                        break;
                    #endregion
                }
            }
        }
        private void LoadZestawienie(ZestawienieState stan)
        {
            if (stan != null)
            {
                switch (stan.ViewName)
                {
                    #region MAGMATEWPB
                    case "ZestawienieLoadFiles":
                        ViewModelLocator.ZestawienieLoadFilesViewModel.ListZestawienie = stan.ListZestawienie;
                        _fZestawienieService.SetZestawienie(stan.ListZestService);
                        _fZestawienieService.SetZestawienieKlas(stan.ListZestKlasService);
                        break;
                    case "ZestawienieJimData":
                        ViewModelLocator.ZestawienieLoadFilesViewModel.ListZestawienie = stan.ListZestawienie;
                        _fZestawienieService.SetZestawienie(stan.ListZestService);
                        _fZestawienieService.SetZestawienieKlas(stan.ListZestKlasService);
                        break;
                    #endregion
                }
            }
        }

        private bool CanSaveProject()
        {
            if (this.CurrentPage != null)
            {
                switch (this.CurrentPage.GetPageName())
                {
                    #region SRTR
                    case "SrtrUserConversion":
                        return true;
                    case "SrtrGroupGus":
                        return true;
                    case "SrtrWykaz":
                        return true;
                    case "SrtrJim":
                        return true;
                    #endregion
                    #region MAGMAT EWPB
                    case "MagFillData":
                        return true;

                    case "MagFillDictionary":
                        return true;

                    case "MagJimData":
                        return true;
                    #endregion
                    #region ZESTAWIENIE
                    case "ZestawienieLoadFiles":
                        return true;

                    case "ZestawienieJimData":
                        return true;
                    #endregion
                    default:
                        return false;
                }
            }
            else
                return false;
        }

        private bool CanLoadProject()
        {
            if (this.CurrentPage != null)
            {
                switch (this.CurrentPage.GetPageName())
                {
                    case "SrtrLoadKartoteka":
                        return true;
                    case "MagChooseType":
                        return true;
                    case "ZestawienieLoadFiles":
                        return true;
                    default:
                        return false;
                }
            }
            else
                return false;
        }
        #endregion

        #region MovePreviousCommand

        /// <summary>
        /// Returns the command which, when executed, causes the CurrentPage 
        /// property to reference the previous page in the workflow.
        /// </summary>
        /// 
        private RelayCommand _movePreviousCommand;
        public RelayCommand MovePreviousCommand
        {
            get
            {
                return _movePreviousCommand
                    ?? (_movePreviousCommand = new RelayCommand(
                        () => this.MoveToPreviousPage(),
                        () => this.CanMoveToPreviousPage
                ));
            }
        }

        bool CanMoveToPreviousPage
        {
            get { return 0 < this.CurrentPageIndex; }
        }

        void MoveToPreviousPage()
        {
            try
            {
                if (this.CanMoveToPreviousPage)
                    this.CurrentPage = this.Pages[this.CurrentPageIndex - 1];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion // MovePreviousCommand

        #region MoveNextCommand

        /// <summary>
        /// Returns the command which, when executed, causes the CurrentPage 
        /// property to reference the next page in the workflow.  If the user
        /// is viewing the last page in the workflow, this causes the Wizard
        /// to finish and be removed from the user interface.
        /// </summary>
        private RelayCommand _moveNextCommand;
        public RelayCommand MoveNextCommand
        {
            get
            {
                return _moveNextCommand
                    ?? (_moveNextCommand = new RelayCommand(
                        () => this.MoveToNextPage(),
                        () => this.CanMoveToNextPage
                ));
            }
        }

        bool CanMoveToNextPage
        {
            get { return this.CurrentPage != null && this.CurrentPage.IsValid(); }
        }

        void MoveToNextPage()
        {
            try
            {
                if (this.CanMoveToNextPage)
                {
                    if (this.CurrentPageIndex < this.Pages.Count - 1)
                    {
                        SendMessageToCurrentPage();
                        Komunikat = string.Empty;
                        this.CurrentPage = this.Pages[this.CurrentPageIndex + 1];
                    }
                    else
                        this.OnRequestClose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion // MoveNextCommand

        #region Cancel

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand
                    ?? (_cancelCommand = new RelayCommand(
                        () => ConfirmCancel()
                ));
            }
        }

        #endregion

        #endregion // Commands

        #region Private Helpers

        void CreatePages(Modul modul)
        {
            switch (modul)
            {
                case Modul.SRTR:
                    {
                        var pages = new List<MainWizardPageViewModelBase>();

                        pages.Add(ViewModelLocator.SrtrLoadFilesViewModel);
                        pages.Add(ViewModelLocator.SrtrUsersConversionViewModel);
                        pages.Add(ViewModelLocator.SrtrGroupGusViewModel);
                        pages.Add(ViewModelLocator.SrtrLoadWykazViewModel);
                        pages.Add(ViewModelLocator.SrtrJimViewModel);
                        pages.Add(ViewModelLocator.SrtrPlikWynikowyViewModel);

                        _pages = new ReadOnlyCollection<MainWizardPageViewModelBase>(pages);
                        break;
                    }
                case Modul.MAGMAT_EWPB:
                    {
                        var pages = new List<MainWizardPageViewModelBase>();

                        pages.Add(ViewModelLocator.MagmatEWPBChooseTypeViewModel);
                        pages.Add(ViewModelLocator.MagmatEWPBFillDataViewModel);
                        pages.Add(ViewModelLocator.MagmatEWPBFillDictionaryViewModel);
                        pages.Add(ViewModelLocator.MagmatEWPBJimDataViewModel);
                        pages.Add(ViewModelLocator.MagmatEWPBSigmatViewModel);

                        _pages = new ReadOnlyCollection<MainWizardPageViewModelBase>(pages);
                        break;
                    }
                case Modul.ZESTAWIENIE:
                    {
                        var pages = new List<MainWizardPageViewModelBase>();

                        pages.Add(ViewModelLocator.ZestawienieLoadFilesViewModel);
                        pages.Add(ViewModelLocator.ZestawienieJimViewModel);
                        pages.Add(ViewModelLocator.ZestawieniePlikWynikowyViewModel);

                        _pages = new ReadOnlyCollection<MainWizardPageViewModelBase>(pages);
                        break;
                    }
            }

            this.CurrentPage = this.Pages[0];
        }

        int CurrentPageIndex
        {
            get
            {
                return this.Pages.IndexOf(this.CurrentPage);
            }
        }

        void OnRequestClose()
        {
            Application.Current.Windows[Application.Current.Windows.Count - 1].Close();
            ViewModelLocator.Cleanup();
        }

        private void CallCleanUp(CleanUp cu)
        {
            this.CurrentPage = this.Pages[0];
        }

        private void ConfirmCancel()
        {
            MessageBoxResult result = MessageBox.Show("Anulowanie operacji spowoduje usunięcie obecnego postępu prac, jednak wszystkie wypełnione informacje pozostaną zapisane w bazie danych.", "Czy na pewno chcesz anulować operację?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                this.OnRequestClose();
        }

        private void PokazKomunikat(Message msg)
        {
            Komunikat = msg.MessageText;
        }

        private void SendMessageToCurrentPage()
        {
            switch (this.CurrentPage.GetPageName())
            {
                #region SRTR
                case "SrtrLoadKartoteka":
                    Messenger.Default.Send<Message, SrtrUsersConversionViewModel>(new Message("synchronizuj dane"));
                    break;
                case "SrtrUserConversion":
                    Messenger.Default.Send<Message, SrtrUsersConversionViewModel>(new Message("zapisz dane"));
                    break;
                case "SrtrGroupGus":
                    Messenger.Default.Send<Message, SrtrGroupGusViewModel>(new Message("zapisz dane"));
                    break;
                case "SrtrWykaz":
                    Messenger.Default.Send<Message, SrtrLoadWykazViewModel>(new Message("synchronizuj dane"));
                    break;
                case "SrtrJim":
                    Messenger.Default.Send<Message, SrtrJimViewModel>(new Message("zapisz dane"));
                    break;
                case "SrtrPlikWynikowy":
                    break;
                #endregion
                #region MAGMAT EWPB
                case "MagChooseType":
                    Messenger.Default.Send<Message, MagmatEWPBChooseTypeViewModel>(new Message("zapisz dane"));
                    break;
                case "MagFillData":
                    Messenger.Default.Send<Message, MagmatEWPBFillDataViewModel>(new Message("zapisz dane"));
                    Messenger.Default.Send<Message, MagmatEWPBFillDictionaryViewModel>(new Message("czyść dane"));
                    break;
                case "MagFillDictionary":
                    Messenger.Default.Send<Message, MagmatEWPBFillDictionaryViewModel>(new Message("zapisz dane"));
                    break;
                case "MagJimData":
                    Messenger.Default.Send<Message, MagmatEWPBJimDataViewModel>(new Message("zapisz dane"));
                    break;
                #endregion
                #region ZESTAWIENIE
                case "ZestawienieLoadFiles":
                    Messenger.Default.Send<Message, ZestawienieJimViewModel>(new Message("synchronizuj dane"));
                    break;
                case "ZestawienieJimData":
                    Messenger.Default.Send<Message, ZestawienieJimViewModel>(new Message("zapisz dane"));
                    Messenger.Default.Send<Message, ZestawieniePlikWynikowyViewModel>(new Message("synchronizuj dane"));
                    break;
                #endregion
            }
        }

        #endregion // Private Helpers

        private void SaveSrtrProjectFile<T>(T stan)
        {
            string _fileName = string.Format("Projekt_{0}", DateTime.Now.ToShortDateString());
            SaveFileDialog saveFile = new SaveFileDialog() { FileName = _fileName, DefaultExt = ".dat", Filter = "Plik z danymi (.dat)|*.dat" };

            if (saveFile.ShowDialog() == true)
            {
                using (StreamWriter stream = new StreamWriter(saveFile.FileName))
                {
                    try
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(stream, stan);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Nie można zapisać danych - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        stream.Close();
                    }
                }
            }
        }

        private T LoadProjectFile<T>()
        {
            OpenFileDialog accessDialog = new OpenFileDialog() { DefaultExt = ".dat", Filter = "Plik z danymi (.dat)|*.dat", AddExtension = true };

            if (accessDialog.ShowDialog() == true)
            {
                using (StreamReader stream = File.OpenText(accessDialog.FileName))
                {
                    try
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        T stan = (T)serializer.Deserialize(stream, typeof(T));
                        return stan;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Nie można odczytać danych - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            return default(T);
        }
    }
}
