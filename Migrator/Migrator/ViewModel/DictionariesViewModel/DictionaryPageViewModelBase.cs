using GalaSoft.MvvmLight;
using Migrator.Helpers;

namespace Migrator.ViewModel.DictionariesViewModel
{
    public abstract class DictionaryPageViewModelBase : ViewModelBase
    {
        //public readonly IDBFService _kartotekaSRTRService;

        #region Constructor

        public DictionaryPageViewModelBase()
        {
        }

        #endregion //Constructor

        #region Properties

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(() => Name); }
        }

        private bool _isCurrentPage;
        public bool IsCurrentPage
        {
            get { return _isCurrentPage; }
            set { _isCurrentPage = value; RaisePropertyChanged(() => IsCurrentPage); }
        }

        #endregion //Properties

        #region Methods

        internal abstract bool IsValid();
        internal abstract string Title();
        internal abstract Dictionaries GetPageName();

        #endregion //Methods
    }
}
