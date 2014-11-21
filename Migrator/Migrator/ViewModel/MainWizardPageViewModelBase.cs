using GalaSoft.MvvmLight;
using Migrator.Helpers;
using Migrator.Services;
using System;

namespace Migrator.ViewModel
{
    public abstract class MainWizardPageViewModelBase : ViewModelBase
    {
        #region Constructor
        public MainWizardPageViewModelBase()
        {
        }

        #endregion //Constructor

        #region Properties

        private bool _isCurrentPage;
        public bool IsCurrentPage
        {
            get { return _isCurrentPage; }
            set { _isCurrentPage = value; RaisePropertyChanged(() => IsCurrentPage); }
        }

        #endregion //Properties

        #region Methods

        internal abstract bool IsValid();
        internal abstract string GetPageName();
        internal abstract void LoadData();

        #endregion //Methods
    }
}