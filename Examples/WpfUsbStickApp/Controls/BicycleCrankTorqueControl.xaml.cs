﻿using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Windows.Controls;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for BicycleCrankTorqueControl.xaml
    /// </summary>
    public partial class BicycleCrankTorqueControl : UserControl
    {
        public BicycleCrankTorqueControl(TorqueSensor sensor)
        {
            InitializeComponent();
            DataContext = sensor;
        }
    }
}
