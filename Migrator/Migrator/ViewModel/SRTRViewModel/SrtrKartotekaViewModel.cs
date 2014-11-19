using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Services;

namespace Migrator.ViewModel.SRTRViewModel
{
    public class SrtrKartotekaViewModel : MainWizardPageViewModelBase
    {
        private readonly IKartotekaSRTRService _kartotekaSRTRService;
        public SrtrKartotekaViewModel(IKartotekaSRTRService kartotekaSRTRService)
        {
            _kartotekaSRTRService = kartotekaSRTRService;
        }

        private RelayCommand<string> _test1;
        public RelayCommand<string> Test1
        {
            get
            {
                return _test1
                    ?? (_test1 = new RelayCommand<string>(
                        title =>
                        {
                            Test();
                        }
                ));
            }
        }

        private RelayCommand<string> _test2;
        public RelayCommand<string> Test2
        {
            get
            {
                return _test2
                    ?? (_test2 = new RelayCommand<string>(
                        title =>
                        {
                            Messenger.Default.Send<string>("Test2."); ;
                        }
                ));
            }
        }

        public void Test()
        {
            string str = string.Format("Path: {0} - DATA: {1}", _kartotekaSRTRService.GetPath(), _kartotekaSRTRService.HasData());
            Messenger.Default.Send<string>(str);
            Messenger.Default.Send<bool>(true);
        }

        #region Methods

        internal override bool IsValid()
        {
            return true;
        }

        #endregion // Methods
    }
}
