using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using Migrator.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Migrator.ViewModel.SRTRViewModel.Windows
{
    public class KartotekaWindowViewModel : ViewModelBase
    {
        #region Constructor

        public  KartotekaWindowViewModel()
        {
            Messenger.Default.Register<List<KartotekaSRTR>>(this, WypelnijKartoteke);
        }

        #endregion //Constructor

        #region Properties

        private ObservableCollection<KartotekaSRTR> _kartotekaSRTRList;
        public ObservableCollection<KartotekaSRTR> KartotekaSRTRList
        {
            get { return _kartotekaSRTRList; }
            set { _kartotekaSRTRList = value; RaisePropertyChanged(() => KartotekaSRTRList); }
        }

        #endregion //Properties

        #region Methods

        private void WypelnijKartoteke(List<KartotekaSRTR> kartotekaList)
        {
            KartotekaSRTRList = kartotekaList.ToObservableCollection<KartotekaSRTR>();
        }

        #endregion //Methods
    }
}
