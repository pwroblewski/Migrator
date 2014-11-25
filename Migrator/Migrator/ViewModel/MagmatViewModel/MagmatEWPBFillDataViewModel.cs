using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Migrator.Services;
using GalaSoft.MvvmLight.Messaging;
using Migrator.Helpers;
using Migrator.Model;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Windows;

namespace Migrator.ViewModel.MagmatViewModel
{
    public class MagmatEWPBFillDataViewModel : MainWizardPageViewModelBase
    {
        #region Fields

        private IMAG_EWPBService _fMagEwpbService;

        #endregion //Fields

        #region Constructor

        public MagmatEWPBFillDataViewModel(IMAG_EWPBService fMagEwpbService)
        {
            _fMagEwpbService = fMagEwpbService;

            SelectedCells = new List<DataGridCellInfo>();
            SelectedMaterialy = new List<MagmatEwpb>();

            Messenger.Default.Register<Message>(this, HandleMessage);
            Messenger.Default.Register<CleanUp>(this, CallCleanUp);
        }

        #endregion //Constructor

        #region Properties

        private List<MagmatEwpb> _listMaterialy;
        public List<MagmatEwpb> ListMaterialy
        {
            get { return _listMaterialy; }
            set { _listMaterialy = value; RaisePropertyChanged(() => ListMaterialy); }
        }

        private List<DataGridCellInfo> _selectedCells;
        public List<DataGridCellInfo> SelectedCells
        {
            get { return _selectedCells; }
            set { _selectedCells = value; RaisePropertyChanged(() => SelectedCells); }
        }

        private List<MagmatEwpb> _selectedMaterialy;
        public List<MagmatEwpb> SelectedMaterialy
        {
            get { return _selectedMaterialy; }
            set { _selectedMaterialy = value; RaisePropertyChanged(() => SelectedMaterialy); }
        }

        private bool _wartoscVis;
        public bool WartoscVis
        {
            get { return _wartoscVis; }
            set { _wartoscVis = value; RaisePropertyChanged(() => WartoscVis); }
        }

        private bool _cenaVis;
        public bool CenaVis
        {
            get { return _cenaVis; }
            set { _cenaVis = value; RaisePropertyChanged(() => CenaVis); }
        }

        #endregion

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

                            SelectedMaterialy.Clear();

                            foreach (DataGridCellInfo cell in SelectedCells)
                            {
                                if (!SelectedMaterialy.Contains((MagmatEwpb)cell.Item))
                                    SelectedMaterialy.Add((MagmatEwpb)cell.Item);
                            }
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

        #region RozdzielDaneCommand

        private RelayCommand<string> _rozdzielDaneCommand;
        public RelayCommand<string> RozdzielDaneCommand
        {
            get
            {
                return _rozdzielDaneCommand
                    ?? (_rozdzielDaneCommand = new RelayCommand<string>(
                        user =>
                        {
                            List<MagmatEwpb> temp = new List<MagmatEwpb>();
                            int i = 1;

                            foreach (MagmatEwpb material in ListMaterialy)
                            {
                                if (SelectedMaterialy.Contains(material))
                                {
                                    int ilosc = material.Ilosc;
                                    material.Ilosc = 1;

                                    decimal ret;
                                    if(Decimal.TryParse(material.Wartosc, out ret))
                                    {
                                        material.Wartosc = Decimal.Round(Convert.ToDecimal(ret/ilosc), 2).ToString();
                                        material.Cena = material.Wartosc;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Nie można rozdzielić podanej wartości!", "Błąd wartości!");
                                    }

                                    for (int j = 0; j < ilosc; j++)
                                    {
                                        MagmatEwpb mat = new MagmatEwpb();
                                        mat = (MagmatEwpb)material.Clone();
                                        mat.Lp = i;
                                        temp.Add(mat);
                                        i++;
                                    }
                                }
                                else
                                {
                                    material.Lp = i;
                                    temp.Add(material);
                                    i++;
                                }
                            }

                            ListMaterialy = temp;
                            RaisePropertyChanged(() => ListMaterialy);
                        }
                ));
            }
        }

        #endregion

        #endregion

        #region Methods

        private void HandleMessage(Message msg)
        {
            if (msg.MessageText.Equals("synchronizuj dane"))
            {
                ListMaterialy = _fMagEwpbService.Materialy;
                MsgToEnum(_fMagEwpbService.TypWydruku);
            }
            if (msg.MessageText.Equals("zapisz dane"))
            {
                _fMagEwpbService.Materialy = ListMaterialy;
                Messenger.Default.Send<Message, MagmatEWPBFillDictionaryViewModel>(new Message("synchronizuj dane"));
            }
        }

        private void WklejDaneZeSchowkaSystemowego()
        {
            string[] separator = new string[] { "\r\n" };
            string[] ClipboardContent = Clipboard.GetText().Split(separator, StringSplitOptions.RemoveEmptyEntries);

            if (SelectedCells.Exists(x => x.Column.Header.Equals("Kategoria")) && SelectedCells.Exists(x => x.Column.Header.Equals("Nr seryjny")))
            {
                List<MagmatEwpb> list = new List<MagmatEwpb>();
                List<DataGridCellInfo> SelectedCellsTemp = SelectedCells.FindAll(x => x.Column.Header.Equals("Kategoria"));

                for (int i = 0; i < SelectedCellsTemp.Count && i < ClipboardContent.Length; i++)
                {
                    string[] ClipboardLine = ClipboardContent[i].Split('\t');

                    MagmatEwpb material = (MagmatEwpb)SelectedCellsTemp[i].Item;
                    material.Kategoria = ClipboardLine[0];

                    if (ClipboardLine.Length > 1)
                        material.NrSeryjny = ClipboardLine[1];

                    list.Add(material);
                }

                ListMaterialy = PrzypiszListe(list);
            }
            else if (SelectedCells.Exists(x => x.Column.Header.Equals("Kategoria")) && !SelectedCells.Exists(x => x.Column.Header.Equals("Nr seryjny")))
            {
                List<MagmatEwpb> list = new List<MagmatEwpb>();

                for (int i = 0; i < SelectedCells.Count && i < ClipboardContent.Length; i++)
                {
                    string[] ClipboardLine = ClipboardContent[i].Split('\t');

                    MagmatEwpb material = (MagmatEwpb)SelectedCells[i].Item;
                    material.Kategoria = ClipboardLine[0];

                    list.Add(material);
                }

                ListMaterialy = PrzypiszListe(list);
            }
            else if (!SelectedCells.Exists(x => x.Column.Header.Equals("Kategoria")) && SelectedCells.Exists(x => x.Column.Header.Equals("Nr seryjny")))
            {
                List<MagmatEwpb> list = new List<MagmatEwpb>();

                for (int i = 0; i < SelectedCells.Count && i < ClipboardContent.Length; i++)
                {
                    string[] ClipboardLine = ClipboardContent[i].Split('\t');

                    MagmatEwpb material = (MagmatEwpb)SelectedCells[i].Item;
                    material.NrSeryjny = ClipboardLine[0];

                    list.Add(material);
                }

                ListMaterialy = PrzypiszListe(list);
            }
        }

        private List<MagmatEwpb> PrzypiszListe(List<MagmatEwpb> list)
        {
            bool znalazl = false;
            List<MagmatEwpb> temp = new List<MagmatEwpb>();

            foreach (MagmatEwpb magEwpb in ListMaterialy)
            {
                znalazl = false;
                foreach (MagmatEwpb magEwpb2 in list)
                {
                    if (magEwpb.Lp.Equals(magEwpb2.Lp))
                    {
                        znalazl = true;
                        temp.Add(magEwpb2);
                    }
                }
                if (!znalazl)
                    temp.Add(magEwpb);
            }

            return temp;
        }

        private void MsgToEnum(MagmatEWPB typ)
        {
            switch (typ)
            {
                case MagmatEWPB.Magmat_305:
                    WartoscVis = true;
                    CenaVis = true;
                    break;

                case MagmatEWPB.EWPB_319_320:
                    WartoscVis = false;
                    CenaVis = false;
                    break;

                case MagmatEWPB.EWPB_351:
                    WartoscVis = true;
                    CenaVis = false;
                    break;
            }
        }

        private void CallCleanUp(CleanUp cu)
        {
            ListMaterialy = null;
            if (SelectedCells != null) SelectedCells.Clear();
            if (SelectedMaterialy != null) SelectedMaterialy.Clear();
        }

        #endregion //Methods

        internal override bool IsValid()
        {
            return true;
        }

        internal override string GetPageName()
        {
            return MagmatEWPBPages.MagFillData.ToString();
        }

        internal override void LoadData()
        {
            throw new NotImplementedException();
        }
    }
}
