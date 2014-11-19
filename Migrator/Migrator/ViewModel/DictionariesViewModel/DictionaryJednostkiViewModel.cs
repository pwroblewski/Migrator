using System;

namespace Migrator.ViewModel.DictionariesViewModel
{
    public class DictionaryJednostkiViewModel : DictionaryPageViewModelBase
    {
        #region Fields

        #endregion

        #region Constructor

        public DictionaryJednostkiViewModel()
        {
            Name = "Słownik jednostek";
        }

        #endregion

        #region Properties

        #endregion

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
            return Helpers.Dictionaries.Jednostki;
        }
    }
}