using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using Migrator.Model;
using Migrator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Migrator.ViewModel.SRTRViewModel
{
    public class SrtrLoadWykazViewModel : MainWizardPageViewModelBase
    {
        #region Fields

        private readonly ISRTRService _fSrtrToZwsironService;

        #endregion //Fields

        #region Constructor

        public SrtrLoadWykazViewModel(ISRTRService fSrtrToZwsironService)
        {
            _fSrtrToZwsironService = fSrtrToZwsironService;

            SelectedCells = new List<DataGridCellInfo>();

            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion //Constructor

        #region Properties

        private List<WykazIlosciowy> _listWykazIlosciowySRTR;
        public List<WykazIlosciowy> ListWykazIlosciowySRTR
        {
            get { return _listWykazIlosciowySRTR; }
            set { _listWykazIlosciowySRTR = value; RaisePropertyChanged(() => ListWykazIlosciowySRTR); }
        }

        private string _wykazIlosciowyPath;
        public string WykazIlosciowyPath
        {
            get { return _wykazIlosciowyPath; }
            set { _wykazIlosciowyPath = value; RaisePropertyChanged(() => WykazIlosciowyPath); }
        }

        private string _zakladText;
        public string ZakladText
        {
            get { return _zakladText; }
            set { _zakladText = value; RaisePropertyChanged(() => ZakladText); }
        }

        private List<DataGridCellInfo> _selectedCells;
        public List<DataGridCellInfo> SelectedCells
        {
            get { return _selectedCells; }
            set { _selectedCells = value; RaisePropertyChanged(() => SelectedCells); }
        }

        #endregion //Properties

        #region Commands

        #region SelectedCellsChangedCommand

        private RelayCommand<SelectedCellsChangedEventArgs> _selectedCellsChangedCommand;
        public RelayCommand<SelectedCellsChangedEventArgs> SelectedCellsChangedCommand
        {
            get
            {
                return _selectedCellsChangedCommand
                    ?? (_selectedCellsChangedCommand = new RelayCommand<SelectedCellsChangedEventArgs>(
                        user =>
                        {
                            SelectedCells.AddRange(user.AddedCells);
                            SelectedCells.RemoveAll(x => user.RemovedCells.Contains(x));

                        }
                ));
            }
        }

        #endregion

        #region KeyDownCommand

        private RelayCommand<KeyEventArgs> _keyDownCommand;
        public RelayCommand<KeyEventArgs> KeyDownCommand
        {
            get
            {
                return _keyDownCommand
                    ?? (_keyDownCommand = new RelayCommand<KeyEventArgs>(
                        user =>
                        {
                            if (user.Key == Key.V && (Keyboard.Modifiers == ModifierKeys.Control)) // Paste
                            {
                                WklejDaneZeSchowkaSystemowego();
                            }
                        }
                ));
            }
        }

        #endregion

        #region WczytajPlikWykazuCommand

        private RelayCommand<string> _wczytajPlikWykazuCommand;
        public RelayCommand<string> WczytajPlikWykazuCommand
        {
            get
            {
                return _wczytajPlikWykazuCommand
                    ?? (_wczytajPlikWykazuCommand = new RelayCommand<string>(
                        title => WczytajPlikWykazu()
                ));
            }
        }

        #endregion

        #region WypelnijZakladCommand

        private RelayCommand<ComboBoxItem> _wypelnijZakladCommand;
        public RelayCommand<ComboBoxItem> WypelnijZakladCommand
        {
            get
            {
                return _wypelnijZakladCommand
                    ?? (_wypelnijZakladCommand = new RelayCommand<ComboBoxItem>(
                        m => WypelnijZaklad(m),
                        m => CzyMozeUzupelnicZaklad()
                ));
            }
        }

        #endregion

        #endregion //Commands

        #region Private Methods

        private void HandleMessage(Message msg)
        {
            if (msg.MessageText.Equals("synchronizuj dane"))
            {
                if (_fSrtrToZwsironService.Wykaz != null)
                    ListWykazIlosciowySRTR = _fSrtrToZwsironService.Wykaz;
            }
            if (msg.MessageText.Equals("zapisz dane"))
            {
                _fSrtrToZwsironService.Wykaz = ListWykazIlosciowySRTR;

                Messenger.Default.Send<Message, SrtrJimViewModel>(new Message("synchronizuj dane"));
            }
        }

        private void WczytajPlikWykazu()
        {
            WykazIlosciowyPath = _fSrtrToZwsironService.OpenWykazFile();
            if (!string.IsNullOrEmpty(WykazIlosciowyPath))
            {
                try
                {
                    // Czytanie pliku
                    _fSrtrToZwsironService.LoadWykazData(WykazIlosciowyPath);
                    ListWykazIlosciowySRTR = _fSrtrToZwsironService.Wykaz;

                    Messenger.Default.Send<Message, MainWizardViewModel>(new Message("Plik wczytano poprawnie."));
                }
                catch (Exception ex)
                {
                    Messenger.Default.Send<Message, MainWizardViewModel>(new Message(string.Format("BŁĄD! - {0}", ex.Message)));
                }
            }
        }

        private bool CzyMozeUzupelnicZaklad()
        {
            // czy jest uzupełnione pole zakład
            bool zakTxt = !string.IsNullOrEmpty(ZakladText);
            // czy lista wykazów jest wypełniona
            bool zakData = ListWykazIlosciowySRTR != null && ListWykazIlosciowySRTR.Count > 0;

            return zakTxt && zakData;
        }

        private void WypelnijZaklad(ComboBoxItem item)
        {
            if (item.Content.Equals("Zaznaczone komórki"))
            {
                List<WykazIlosciowy> list = new List<WykazIlosciowy>();
                SelectedCells.ForEach(x =>
                    {
                        WykazIlosciowy wykaz = (WykazIlosciowy)x.Item;
                        wykaz.Zaklad = ZakladText;
                        list.Add(wykaz);
                    });

                bool znalazl = false;
                List<WykazIlosciowy> temp = new List<WykazIlosciowy>();
                foreach (WykazIlosciowy wykaz in ListWykazIlosciowySRTR)
                {
                    znalazl = false;
                    foreach (WykazIlosciowy wykaz2 in list)
                    {
                        if (wykaz.NrInwentarzowy.Equals(wykaz2.NrInwentarzowy))
                        {
                            znalazl = true;
                            temp.Add(wykaz2);
                        }
                    }
                    if (!znalazl)
                        temp.Add(wykaz);
                }

                ListWykazIlosciowySRTR = temp;
            }
            else
            {
                //ListWykazIlosciowySRTR.ForEach(x => x.Zaklad = ZakladText);
                List<WykazIlosciowy> tempList = new List<WykazIlosciowy>();
                foreach (WykazIlosciowy wykaz in ListWykazIlosciowySRTR)
                {
                    wykaz.Zaklad = ZakladText;
                    tempList.Add(wykaz);
                }
                ListWykazIlosciowySRTR = tempList;
            }
        }

        private void WklejDaneZeSchowkaSystemowego()
        {
            string[] separator = new string[] { "\r\n" };
            string[] ClipboardContent = Clipboard.GetText().Split(separator, StringSplitOptions.RemoveEmptyEntries);

            List<WykazIlosciowy> list = new List<WykazIlosciowy>();
            for (int i = 0; i < ClipboardContent.Length && i < SelectedCells.Count; i++)
            {
                if (SelectedCells[i].Column.Header.Equals("Zakład"))
                {
                    WykazIlosciowy wykaz = (WykazIlosciowy)SelectedCells[i].Item;
                    wykaz.Zaklad = ClipboardContent[i];
                    list.Add(wykaz);
                }
            }

            bool znalazl = false;
            List<WykazIlosciowy> temp = new List<WykazIlosciowy>();
            foreach (WykazIlosciowy wykaz in ListWykazIlosciowySRTR)
            {
                znalazl = false;
                foreach (WykazIlosciowy wykaz2 in list)
                {
                    if (wykaz.NrInwentarzowy.Equals(wykaz2.NrInwentarzowy))
                    {
                        znalazl = true;
                        temp.Add(wykaz2);
                    }
                }
                if (!znalazl)
                    temp.Add(wykaz);
            }

            ListWykazIlosciowySRTR = temp;
        }

        internal override bool IsValid()
        {
            if (!string.IsNullOrEmpty(WykazIlosciowyPath))
                return true;

            return false;
        }

        internal override string GetPageName()
        {
            return SRTRPages.SrtrWykaz.ToString();
        }

        private void CallCleanUp(CleanUp cu)
        {
            if (WykazIlosciowyPath != null) WykazIlosciowyPath = string.Empty;
            if (ZakladText != null) ZakladText = string.Empty;
            if (SelectedCells != null) SelectedCells.Clear();
            ListWykazIlosciowySRTR = null;
        }

        #endregion //Private Methods

        internal override void LoadData()
        {
            throw new NotImplementedException();
        }
    }
}
