﻿using Migrator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Migrator.View.MagmatView
{
    /// <summary>
    /// Interaction logic for MagmatEWPBSigmatView.xaml
    /// </summary>
    public partial class MagmatEWPBSigmatView : UserControl
    {
        public MagmatEWPBSigmatView()
        {
            InitializeComponent();
        }

        private void dataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                CustomPaste.Paste(sender as DataGrid);
            }
        }
    }
}
