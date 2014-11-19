using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.ViewModel.DictionariesViewModel
{
    public class DictionaryMagazynyViewModel : DictionaryPageViewModelBase
    {
        #region Fields

        #endregion

        #region Constructor

        public DictionaryMagazynyViewModel()
        {
            Name = "Słownik magazynów";
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
            return Helpers.Dictionaries.Magazyny;
        }
    }
}
