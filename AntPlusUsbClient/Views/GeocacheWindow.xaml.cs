﻿using AntPlusUsbClient.ViewModels;
using SmallEarthTech.AntPlus.DeviceProfiles.Geocache;
using System.Windows;

namespace AntPlusUsbClient.Views
{
    /// <summary>
    /// Interaction logic for GeocacheWindow.xaml
    /// </summary>
    public partial class GeocacheWindow : Window
    {
        public GeocacheWindow(Geocache geocache)
        {
            InitializeComponent();
            GeocacheViewModel geo = new GeocacheViewModel(geocache);
            CommandBindings.AddRange(geo.CommandBindings);
            DataContext = geo;
        }
    }
}
