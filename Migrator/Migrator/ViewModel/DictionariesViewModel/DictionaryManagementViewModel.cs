using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Migrator.ViewModel.DictionariesViewModel
{
    public class DictionaryManagementViewModel : ViewModelBase
    {
        #region Constructor

        public DictionaryManagementViewModel()
        {
            CreateDictionaryPages();
        }

        #endregion //Constructor

        #region Properties

        private DictionaryPageViewModelBase _currentPage;
        public DictionaryPageViewModelBase CurrentPage
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
            }
        }

        private List<DictionaryPageViewModelBase> _pages;
        public List<DictionaryPageViewModelBase> Pages
        {
            get
            {
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

        private RelayCommand<string> _showDictionaryCommand;
        public RelayCommand<string> ShowDictionaryCommand
        {
            get
            {
                return _showDictionaryCommand
                    ?? (_showDictionaryCommand = new RelayCommand<string>(name =>
                    {
                        var pageIndex = Pages.FindIndex( x => x.Name.Equals(name) );

                        CurrentPage = Pages[pageIndex];
                        SendMessageToCurrentPage();
                    }
                ));
            }
        }

        #endregion //Commands

        #region Methods

        private void CreateDictionaryPages()
        {
            var pages = new List<DictionaryPageViewModelBase>();

            pages.Add(ViewModelLocator.DictionaryAmortyzacjaViewModel);
            pages.Add(ViewModelLocator.DictionaryJednostkiMiaryViewModel);
            pages.Add(ViewModelLocator.DictionaryGrupyGusViewModel);
            pages.Add(ViewModelLocator.DictionaryUzytkownicyViewModel);
            pages.Add(ViewModelLocator.DictionaryJednostkiViewModel);
            pages.Add(ViewModelLocator.DictionaryMagazynyViewModel);
            
            pages.ForEach(x => x.Name = x.Title());

            _pages = new List<DictionaryPageViewModelBase>(pages);

            CurrentPage = Pages[0];
        }

        private void SendMessageToCurrentPage()
        {
            switch (this.CurrentPage.GetPageName())
            {
                case Dictionaries.Amortyzacja:
                    Messenger.Default.Send<Message, DictionaryAmortyzacjaViewModel>(new Message("wczytaj dane"));
                    break;

                case Dictionaries.GrupaGUS:
                    Messenger.Default.Send<Message, DictionaryGrupyGusViewModel>(new Message("wczytaj dane"));
                    break;

                case Dictionaries.Jednostki:
                    Messenger.Default.Send<Message, DictionaryJednostkiViewModel>(new Message("wczytaj dane"));
                    break;

                case Dictionaries.JednstkaMiary:
                    Messenger.Default.Send<Message, DictionaryJednostkiMiaryViewModel>(new Message("wczytaj dane"));
                    break;

                case Dictionaries.Magazyny:
                    Messenger.Default.Send<Message, DictionaryMagazynyViewModel>(new Message("wczytaj dane"));
                    break;

                case Dictionaries.Uzytkownicy:
                    Messenger.Default.Send<Message, DictionaryUzytkownicyViewModel>(new Message("wczytaj dane"));
                    break;
            }
        }

        #endregion //Methods
    }
}
