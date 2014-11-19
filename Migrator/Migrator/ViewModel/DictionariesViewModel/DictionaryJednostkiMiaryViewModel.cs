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
    public class DictionaryJednostkiMiaryViewModel : DictionaryPageViewModelBase
    {
        #region Fields
        private readonly IDBJmService _jmService;
        #endregion

        #region Constructor

        public DictionaryJednostkiMiaryViewModel(IDBJmService jmService)
        {
            _jmService = jmService;

            Name = "Słownik jednostek miar";

            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion

        #region Properties

        private JednostkaMiary _jm;
        public JednostkaMiary Jm
        {
            get { return _jm; }
            set { _jm = value; RaisePropertyChanged(() => Jm); }
        }

        private List<JednostkaMiary> _jmList;
        public List<JednostkaMiary> JmList
        {
            get { return _jmList; }
            set { _jmList = value; RaisePropertyChanged(() => JmList); }
        }

        #endregion

        #region Methods

        private async void LoadJmData()
        {
            JmList = await _jmService.GetAll();
        }

        internal override bool IsValid()
        {
            throw new NotImplementedException();
        }

        internal override string Title()
        {
            return Name;
        }

        internal override Helpers.Dictionaries GetPageName()
        {
            return Helpers.Dictionaries.JednstkaMiary;
        }

        private void HandleMessage(Message msg)
        {
            if (JmList == null || !JmList.Any())
            {
                LoadJmData();
            }
        }

        private void CallCleanUp(CleanUp cu)
        {
            if (Name != null) Name = string.Empty;
            if (Jm != null) Jm = null;
            if (JmList != null) JmList.Clear();
        }

        #endregion //Methods
    }
}
