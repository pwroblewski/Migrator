using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Model;
using Migrator.View;
using Migrator.Helpers;
using Migrator.View.DictionariesView;
using System.IO;
using System.Windows;

namespace Migrator.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private string _dbPath = string.Format("{0}\\MigratorDB.s3db", Directory.GetCurrentDirectory());

        private const string SrtrTitle = "Migracja z SRTR";
        private const string MagmatTitle = "Migracja z MAGMAT/EWPB";
        private const string ZestawienieTitle = "Zestawienie";

        private MainWizardWindow srtrWizardWindow;
        private DictionaryManagementWindow dictionaryManagementWindow;

        #endregion

        #region RelayCommand

        private RelayCommand<Modul> _openSrtrWizardCommad;
        public RelayCommand<Modul> OpenSrtrWizardCommad
        {
            get
            {
                return _openSrtrWizardCommad ??
                (
                    _openSrtrWizardCommad = new RelayCommand<Modul>
                    (
                        modul =>
                        {
                            
                            srtrWizardWindow = new MainWizardWindow();
                            Messenger.Default.Send<Modul>(modul);

                            switch (modul)
                            {
                                case Modul.SRTR:
                                    {
                                        srtrWizardWindow.Title = SrtrTitle;
                                        break;
                                    }
                                case Modul.MAGMAT_EWPB:
                                    {
                                        srtrWizardWindow.Title = MagmatTitle;
                                        break;
                                    }
                                case Modul.ZESTAWIENIE:
                                    {
                                        srtrWizardWindow.Title = ZestawienieTitle;
                                        break;
                                    }
                            }

                            if (srtrWizardWindow.ShowDialog() == true) { };
                        }
                    )
                );
            }
        }

        private RelayCommand _openDictionariesManagementCommand;
        public RelayCommand OpenDictionariesManagementCommand
        {
            get
            {
                return _openDictionariesManagementCommand ??
                (
                    _openDictionariesManagementCommand = new RelayCommand
                    (
                        () =>
                        {
                            dictionaryManagementWindow = new DictionaryManagementWindow();
                            if (dictionaryManagementWindow.ShowDialog() == true) { };
                           
                        }
                    )
                );
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            App.Connection = new SQLiteAsyncConnection(_dbPath);
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed
        ////    base.Cleanup();
        ////}
    }
}