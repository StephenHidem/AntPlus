﻿using AntPlus.DeviceProfiles.AssetTracker;
using AntPlusUsbClient.ViewModels;
using System.Windows;
using System.Windows.Data;

namespace AntPlusUsbClient.Views
{
    /// <summary>
    /// Interaction logic for AssetTrackerWindow.xaml
    /// </summary>
    public partial class AssetTrackerWindow : Window
    {
        public AssetTrackerWindow(AssetTracker assetTracker)
        {
            InitializeComponent();
            AssetTrackerViewModel vm = new AssetTrackerViewModel(assetTracker);
            BindingOperations.EnableCollectionSynchronization(assetTracker.Assets, assetTracker.Assets.collectionLock);
            //CommandBindings.AddRange(geo.CommandBindings);
            DataContext = vm;
        }
    }
}