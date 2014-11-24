/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:Migrator.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using Migrator.Helpers;
using Migrator.Model;
using Migrator.Services;
using Migrator.ViewModel.DictionariesViewModel;
using Migrator.ViewModel.MagmatViewModel;
using Migrator.ViewModel.SRTRViewModel;
using Migrator.ViewModel.SRTRViewModel.Windows;
using Migrator.ViewModel.ZestawienieViewModel;

namespace Migrator.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {

            }
            else
            {
                //SimpleIoc.Default.Register<IFileUserService, FileUserService>();
                //SimpleIoc.Default.Register<IFileMagazynService, FileMagazynService>();
                //SimpleIoc.Default.Register<IFileJednostkaService, FileJednostkaService>();
                //SimpleIoc.Default.Register<IFileJimService, FileJimService>();
                SimpleIoc.Default.Register<ISRTRService, SRTRService>();
                //SimpleIoc.Default.Register<IFileMagmatEwpbService, FileMagmatEwpbService>();
                SimpleIoc.Default.Register<IMAG_EWPBService, MAG_EWPBService>();
                SimpleIoc.Default.Register<IFileZestawienieService, FileZestawienieService>();
                SimpleIoc.Default.Register<IDBUserService, DBUserService>();
                SimpleIoc.Default.Register<IDBAmorService, DBAmorService>();
                SimpleIoc.Default.Register<IDBJmService, DBJmService>();
                SimpleIoc.Default.Register<IDBGrupaAktywowService, DBGrupaAktywowService>();
                SimpleIoc.Default.Register<IDBGrRodzGusSRTRService, DBGrRodzGusSRTRService>();
                SimpleIoc.Default.Register<IDBGrRodzGusZWSIRONService, DBGrRodzGusZWSIRONService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();

            SimpleIoc.Default.Register<MainWizardViewModel>();
            SimpleIoc.Default.Register<KartotekaWindowViewModel>();

            //SRTR Pages
            SimpleIoc.Default.Register<SrtrLoadFilesViewModel>();
            SimpleIoc.Default.Register<SrtrUsersConversionViewModel>();
            SimpleIoc.Default.Register<SrtrGroupGusViewModel>();
            SimpleIoc.Default.Register<SrtrLoadWykazViewModel>();
            SimpleIoc.Default.Register<SrtrJimViewModel>();
            SimpleIoc.Default.Register<SrtrPlikWynikowyViewModel>();

            //MAGMAT-EWPB Pages
            SimpleIoc.Default.Register<MagmatEWPBChooseTypeViewModel>();
            SimpleIoc.Default.Register<MagmatEWPBFillDataViewModel>();
            SimpleIoc.Default.Register<MagmatEWPBFillDictionaryViewModel>();
            SimpleIoc.Default.Register<MagmatEWPBJimDataViewModel>();
            SimpleIoc.Default.Register<MagmatEWPBSigmatViewModel>();

            //ZESTAWIENIE Pages
            SimpleIoc.Default.Register<ZestawienieLoadFilesViewModel>();
            SimpleIoc.Default.Register<ZestawienieJimViewModel>();
            SimpleIoc.Default.Register<ZestawieniePlikWynikowyViewModel>();

            //Dictionary Pages
            SimpleIoc.Default.Register<DictionaryManagementViewModel>();
            SimpleIoc.Default.Register<DictionaryUzytkownicyViewModel>();
            SimpleIoc.Default.Register<DictionaryJednostkiViewModel>();
            SimpleIoc.Default.Register<DictionaryMagazynyViewModel>();
            SimpleIoc.Default.Register<DictionaryJednostkiMiaryViewModel>();
            SimpleIoc.Default.Register<DictionaryGrupyGusViewModel>();
            SimpleIoc.Default.Register<DictionaryAmortyzacjaViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public MainWizardViewModel MainWizardViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainWizardViewModel>();
            }
        }

        #region SRTR
        public static SrtrLoadFilesViewModel SrtrLoadFilesViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SrtrLoadFilesViewModel>();
            }
        }

        public static SrtrUsersConversionViewModel SrtrUsersConversionViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SrtrUsersConversionViewModel>();
            }
        }

        public static SrtrGroupGusViewModel SrtrGroupGusViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SrtrGroupGusViewModel>();
            }
        }

        public static SrtrLoadWykazViewModel SrtrLoadWykazViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SrtrLoadWykazViewModel>();
            }
        }

        public static SrtrJimViewModel SrtrJimViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SrtrJimViewModel>();
            }
        }

        public static SrtrPlikWynikowyViewModel SrtrPlikWynikowyViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SrtrPlikWynikowyViewModel>();
            }
        }

        public KartotekaWindowViewModel KartotekaWindowViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<KartotekaWindowViewModel>();
            }
        }
        #endregion

        #region MAGMAT
        public static MagmatEWPBChooseTypeViewModel MagmatEWPBChooseTypeViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MagmatEWPBChooseTypeViewModel>();
            }
        }
        public static MagmatEWPBFillDataViewModel MagmatEWPBFillDataViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MagmatEWPBFillDataViewModel>();
            }
        }
        public static MagmatEWPBFillDictionaryViewModel MagmatEWPBFillDictionaryViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MagmatEWPBFillDictionaryViewModel>();
            }
        }
        public static MagmatEWPBJimDataViewModel MagmatEWPBJimDataViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MagmatEWPBJimDataViewModel>();
            }
        }
        public static MagmatEWPBSigmatViewModel MagmatEWPBSigmatViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MagmatEWPBSigmatViewModel>();
            }
        }
        #endregion

        #region ZESTAWIENIE
        public static ZestawienieLoadFilesViewModel ZestawienieLoadFilesViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ZestawienieLoadFilesViewModel>();
            }
        }
        public static ZestawienieJimViewModel ZestawienieJimViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ZestawienieJimViewModel>();
            }
        }
        public static ZestawieniePlikWynikowyViewModel ZestawieniePlikWynikowyViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ZestawieniePlikWynikowyViewModel>();
            }
        }
        #endregion

        #region Dictionary
        public static DictionaryManagementViewModel DictionaryManagementViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DictionaryManagementViewModel>();
            }
        }

        public static DictionaryUzytkownicyViewModel DictionaryUzytkownicyViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DictionaryUzytkownicyViewModel>();
            }
        }

        public static DictionaryJednostkiViewModel DictionaryJednostkiViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DictionaryJednostkiViewModel>();
            }
        }

        public static DictionaryMagazynyViewModel DictionaryMagazynyViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DictionaryMagazynyViewModel>();
            }
        }

        public static DictionaryGrupyGusViewModel DictionaryGrupyGusViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DictionaryGrupyGusViewModel>();
            }
        }

        public static DictionaryJednostkiMiaryViewModel DictionaryJednostkiMiaryViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DictionaryJednostkiMiaryViewModel>();
            }
        }

        public static DictionaryAmortyzacjaViewModel DictionaryAmortyzacjaViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DictionaryAmortyzacjaViewModel>();
            }
        }

        #endregion
        
        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            Messenger.Default.Send<CleanUp>(new CleanUp());
        }
    }
}