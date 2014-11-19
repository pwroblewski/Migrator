using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using Migrator.Model;
using Migrator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.ViewModel.DictionariesViewModel
{
    public class DictionaryAmortyzacjaViewModel : DictionaryPageViewModelBase
    {
        #region Fields
        private readonly IDBAmorService _amorService; 
        #endregion

        #region Constructor

        public DictionaryAmortyzacjaViewModel(IDBAmorService amorService)
        {
            _amorService = amorService;

            Name = "Słownik amortyzacji";

            LoadAmortyzacjaData();

            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion

        #region Properties

        private Amortyzacja _amortyzacja;
        public Amortyzacja Amortyzacja
        {
            get { return _amortyzacja; }
            set { _amortyzacja = value; RaisePropertyChanged(() => Amortyzacja); }
        }

        private List<Amortyzacja> _amorList;
        public List<Amortyzacja> AmorList
        {
            get { return _amorList; }
            set { _amorList = value; RaisePropertyChanged(() => AmorList); }
        }

        #endregion

        #region Methods

        private void HandleMessage(Message msg)
        {
            if (AmorList==null || !AmorList.Any())
                LoadAmortyzacjaData();
        }

        private async void LoadAmortyzacjaData()
        {
            AmorList = await _amorService.GetAll();
        }

        internal override Helpers.Dictionaries GetPageName()
        {
            return Helpers.Dictionaries.Amortyzacja;
        }

        private void CallCleanUp(CleanUp cu)
        {
            if (Name != null) Name = string.Empty;
            if (Amortyzacja != null) Amortyzacja = null;
            if (AmorList != null) AmorList.Clear();
        }

        internal override bool IsValid()
        {
            throw new NotImplementedException();
        }

        internal override string Title()
        {
            return Name;
        }

        #endregion //Methods

        
    }
}
