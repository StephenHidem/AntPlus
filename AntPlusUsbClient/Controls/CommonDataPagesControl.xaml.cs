﻿using SmallEarthTech.AntPlus;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for CommonDataPagesControl.xaml
    /// </summary>
    public partial class CommonDataPagesControl : UserControl
    {
        public CommonDataPagesControl()
        {
            InitializeComponent();
        }

        public CommonDataPagesControl(CommonDataPages ctx)
        {
            InitializeComponent();
            DataContext = ctx;
        }
    }
}
