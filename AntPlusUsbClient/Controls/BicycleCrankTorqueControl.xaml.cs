﻿using AntPlus.DeviceProfiles.BicyclePower;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for BicycleCrankTorqueControl.xaml
    /// </summary>
    public partial class BicycleCrankTorqueControl : UserControl
    {
        public BicycleCrankTorqueControl(BicyclePower bp)
        {
            InitializeComponent();
            DataContext = bp.CrankTorqueSensor;
        }
    }
}
