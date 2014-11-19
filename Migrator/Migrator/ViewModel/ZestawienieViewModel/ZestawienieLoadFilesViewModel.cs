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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Migrator.ViewModel.ZestawienieViewModel
{
    public class ZestawienieLoadFilesViewModel : MainWizardPageViewModelBase
    {
        #region Fields

        private IFileZestawienieService _fZestawienieService;

        #endregion //Fields

        #region Constructor

        public ZestawienieLoadFilesViewModel(IFileZestawienieService fZestawienieService)
        {
            _fZestawienieService = fZestawienieService;

            SelectedCells = new List<DataGridCellInfo>();
            SelectedMaterialy = new List<Zestawienie>();

            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion //Constructor

        #region Properties

        private ObservableCollection<Zestawienie> _listZestawienie;
        public ObservableCollection<Zestawienie> ListZestawienie
        {
            get { return _listZestawienie; }
            set { _listZestawienie = value; RaisePropertyChanged(() => ListZestawienie); }
        }

        private List<DataGridCellInfo> _selectedCells;
        public List<DataGridCellInfo> SelectedCells
        {
            get { return _selectedCells; }
            set { _selectedCells = value; RaisePropertyChanged(() => SelectedCells); }
        }

        private List<Zestawienie> _selectedMaterialy;
        public List<Zestawienie> SelectedMaterialy
        {
            get { return _selectedMaterialy; }
            set { _selectedMaterialy = value; RaisePropertyChanged(() => SelectedMaterialy); }
        }

        private string _dzialText;
        public string DzialText
        {
            get { return _dzialText; }
            set { _dzialText = value; RaisePropertyChanged(() => DzialText); }
        }

        #endregion

        #region Commands

        #region WczytajPlikiCommand

        private RelayCommand<string> _wczytajPlikiCommand;
        public RelayCommand<string> WczytajPlikiCommand
        {
            get
            {
                return _wczytajPlikiCommand
                    ?? (_wczytajPlikiCommand = new RelayCommand<string>(
                        path => WczytajPlik()
                ));
            }
        }

        #endregion

        #region CzyscCommand


        private RelayCommand<string> _czyscCommand;
        public RelayCommand<string> CzyscCommand
        {
            get
            {
                return _czyscCommand
                    ?? (_czyscCommand = new RelayCommand<string>(
                        path =>
                        {
                            if (ListZestawienie != null)
                            {
                                ListZestawienie.Clear();
                                _fZestawienieService.Clean();
                            }
                        }

                ));
            }
        }

        #endregion

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

        #region WypelnijDzialCommand

        private RelayCommand<ComboBoxItem> _wypelnijDzialCommand;
        public RelayCommand<ComboBoxItem> WypelnijDzialCommand
        {
            get
            {
                return _wypelnijDzialCommand
                    ?? (_wypelnijDzialCommand = new RelayCommand<ComboBoxItem>(
                        m => WypelnijDzial(m),
                        m => CzyMozeUzupelnicDzial()
                ));
            }
        }

        #endregion

        #region WypelnijMaterialKontoCommand

        private RelayCommand<ComboBoxItem> _wypelnijMaterialKontoCommand;
        public RelayCommand<ComboBoxItem> WypelnijMaterialKontoCommand
        {
            get
            {
                return _wypelnijMaterialKontoCommand
                    ?? (_wypelnijMaterialKontoCommand = new RelayCommand<ComboBoxItem>(
                        m => WypelnijMaterialKonto(m),
                        m => CzyMozeUzupelnicDzial()
                ));
            }
        }

        #endregion

        #endregion

        #region Methods

        private void WczytajPlik()
        {
            try
            {
                string[] PlikPath = _fZestawienieService.OpenFileDialog();
                if (PlikPath != null && PlikPath.Any())
                {
                    ListZestawienie = ExtensionMethods.ToObservableCollection<Zestawienie>(_fZestawienieService.GetAll(PlikPath));

                    Messenger.Default.Send<Message, MainWizardViewModel>(new Message("Poprawnie wczytano pliki"));
                    //Messenger.Default.Send<Message, ZestawienieJimViewModel>(new Message("synchronizuj dane"));
                    //Messenger.Default.Send<Message, ZestawieniePlikWynikowyViewModel>(new Message("synchronizuj dane"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Bład odczytu danych", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WklejDaneZeSchowkaSystemowego()
        {
            string[] separator = new string[] { "\r\n" };
            string[] ClipboardContent = Clipboard.GetText().Split(separator, StringSplitOptions.RemoveEmptyEntries);

            if (SelectedCells.Exists(x => x.Column.Header.Equals("Materiał prof")) && SelectedCells.Exists(x => x.Column.Header.Equals("Materiał konto")))
            {
                List<Zestawienie> list = new List<Zestawienie>();
                List<DataGridCellInfo> SelectedCellsTemp = SelectedCells.FindAll(x => x.Column.Header.Equals("Materiał prof"));

                for (int i = 0; i < SelectedCellsTemp.Count && i < ClipboardContent.Length; i++)
                {
                    string[] ClipboardLine = ClipboardContent[i].Split('\t');

                    Zestawienie material = (Zestawienie)SelectedCellsTemp[i].Item;
                    material.MaterialProf = ClipboardLine[0];

                    if (ClipboardLine.Length > 1)
                        material.MaterialKonto = ClipboardLine[1];

                    list.Add(material);
                }

                ListZestawienie = PrzypiszListe(list);
            }
            else if (SelectedCells.Exists(x => x.Column.Header.Equals("Materiał prof")) && !SelectedCells.Exists(x => x.Column.Header.Equals("Materiał konto")))
            {
                List<Zestawienie> list = new List<Zestawienie>();

                for (int i = 0; i < SelectedCells.Count && i < ClipboardContent.Length; i++)
                {
                    string[] ClipboardLine = ClipboardContent[i].Split('\t');

                    Zestawienie material = (Zestawienie)SelectedCells[i].Item;
                    material.MaterialProf = ClipboardLine[0];

                    list.Add(material);
                }

                ListZestawienie = PrzypiszListe(list);
            }
            else if (!SelectedCells.Exists(x => x.Column.Header.Equals("Materiał prof")) && SelectedCells.Exists(x => x.Column.Header.Equals("Materiał konto")))
            {
                List<Zestawienie> list = new List<Zestawienie>();

                for (int i = 0; i < SelectedCells.Count && i < ClipboardContent.Length; i++)
                {
                    string[] ClipboardLine = ClipboardContent[i].Split('\t');

                    Zestawienie material = (Zestawienie)SelectedCells[i].Item;
                    material.MaterialKonto = ClipboardLine[0];

                    list.Add(material);
                }

                ListZestawienie = PrzypiszListe(list);
            }
            else if (SelectedCells.Exists(x => x.Column.Header.Equals("Dział")))
            {
                List<Zestawienie> list = new List<Zestawienie>();

                for (int i = 0; i < SelectedCells.Count && i < ClipboardContent.Length; i++)
                {
                    string[] ClipboardLine = ClipboardContent[i].Split('\t');

                    Zestawienie material = (Zestawienie)SelectedCells[i].Item;
                    material.Dzial = ClipboardLine[0];

                    list.Add(material);
                }

                ListZestawienie = PrzypiszListe(list);
            }

        }

        private ObservableCollection<Zestawienie> PrzypiszListe(List<Zestawienie> list)
        {
            bool znalazl = false;
            List<Zestawienie> temp = new List<Zestawienie>();

            foreach (Zestawienie zestawienie in ListZestawienie)
            {
                znalazl = false;
                foreach (Zestawienie zestawienie2 in list)
                {
                    if (zestawienie.Jim.Equals(zestawienie2.Jim))
                    {
                        znalazl = true;
                        temp.Add(zestawienie2);
                    }
                }
                if (!znalazl)
                    temp.Add(zestawienie);
            }

            return ExtensionMethods.ToObservableCollection<Zestawienie>(temp);
        }

        private bool CzyMozeUzupelnicDzial()
        {
            // czy jest uzupełnione pole zakład
            bool zakTxt = !string.IsNullOrEmpty(DzialText);
            // czy lista wykazów jest wypełniona
            bool zakData = ListZestawienie != null && ListZestawienie.Any();

            return zakTxt && zakData;
        }

        private void WypelnijDzial(ComboBoxItem item)
        {
            if (item.Content.Equals("Zaznaczone komórki"))
            {
                List<Zestawienie> list = new List<Zestawienie>();
                SelectedCells.ForEach(x =>
                {
                    Zestawienie zestawienie = (Zestawienie)x.Item;
                    zestawienie.Dzial = DzialText;
                    list.Add(zestawienie);
                });

                bool znalazl = false;
                List<Zestawienie> temp = new List<Zestawienie>();
                foreach (Zestawienie zest in ListZestawienie)
                {
                    znalazl = false;
                    foreach (Zestawienie zest2 in list)
                    {
                        if (zest.Jim.Equals(zest2.Jim))
                        {
                            znalazl = true;
                            temp.Add(zest2);
                        }
                    }
                    if (!znalazl)
                        temp.Add(zest);
                }

                ListZestawienie = ExtensionMethods.ToObservableCollection<Zestawienie>(temp);
            }
            else
            {
                //ListWykazIlosciowySRTR.ForEach(x => x.Zaklad = ZakladText);
                List<Zestawienie> tempList = new List<Zestawienie>();
                foreach (Zestawienie zest in ListZestawienie)
                {
                    zest.Dzial = DzialText;
                    tempList.Add(zest);
                }
                ListZestawienie = ExtensionMethods.ToObservableCollection<Zestawienie>(tempList);
            }
        }

        private void WypelnijMaterialKonto(ComboBoxItem item)
        {
            if (item.Content.Equals("Zaznaczone komórki"))
            {
                List<Zestawienie> list = new List<Zestawienie>();
                SelectedCells.ForEach(x =>
                {
                    Zestawienie zestawienie = (Zestawienie)x.Item;
                    zestawienie.MaterialKonto = DzialText;
                    list.Add(zestawienie);
                });

                bool znalazl = false;
                List<Zestawienie> temp = new List<Zestawienie>();
                foreach (Zestawienie zest in ListZestawienie)
                {
                    znalazl = false;
                    foreach (Zestawienie zest2 in list)
                    {
                        if (zest.Jim.Equals(zest2.Jim))
                        {
                            znalazl = true;
                            temp.Add(zest2);
                        }
                    }
                    if (!znalazl)
                        temp.Add(zest);
                }

                ListZestawienie = ExtensionMethods.ToObservableCollection<Zestawienie>(temp);
            }
            else
            {
                //ListWykazIlosciowySRTR.ForEach(x => x.Zaklad = ZakladText);
                List<Zestawienie> tempList = new List<Zestawienie>();
                foreach (Zestawienie zest in ListZestawienie)
                {
                    zest.MaterialKonto = DzialText;
                    tempList.Add(zest);
                }
                ListZestawienie = ExtensionMethods.ToObservableCollection<Zestawienie>(tempList);
            }
        }

        private void CallCleanUp(CleanUp cu)
        {
            if (ListZestawienie != null) ListZestawienie.Clear();
        }

        #endregion

        internal override bool IsValid()
        {
            if (ListZestawienie != null)
            {
                bool isValid = ListZestawienie.ToList().Exists(x => string.IsNullOrEmpty(x.Dzial) || string.IsNullOrEmpty(x.MaterialKonto));
                return isValid ? false : true;
            }
            else
            {
                return false;
            }
        }

        internal override string GetPageName()
        {
            return ZestawieniePages.ZestawienieLoadFiles.ToString();
        }
    }
}
