﻿using AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using AntPlusUsbClient.ViewModels;
using System.Windows;

namespace AntPlusUsbClient.Views
{
    /// <summary>
    /// Interaction logic for BikeSpeedAndCadenceWindow.xaml
    /// </summary>
    public partial class BikeSpeedAndCadenceWindow : Window
    {
        public BikeSpeedAndCadenceWindow(CombinedSpeedAndCadenceSensor combinedSpeedAndCadence)
        {
            InitializeComponent();
            DataContext = new BikeSpeedAndCadenceViewModel(combinedSpeedAndCadence);
        }
    }
}
