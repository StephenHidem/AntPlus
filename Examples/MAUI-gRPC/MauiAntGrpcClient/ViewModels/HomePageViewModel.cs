﻿using AntGrpcShared.ClientServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntPlus.Extensions.Hosting;
using SmallEarthTech.AntRadioInterface;
using System.Diagnostics;
using System.Net;

namespace MauiAntGrpcClient.ViewModels
{
    public partial class HomePageViewModel : ObservableObject
    {
        private readonly AntRadioService _antRadioService;
        private readonly AntCollection _antCollection;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ShowRadioCapabilitiesCommand))]
        private bool isBusy;
        [ObservableProperty]
        private IPAddress? serverIPAddress;
        [ObservableProperty]
        private string? productDescription;
        [ObservableProperty]
        private uint? serialNumber;
        [ObservableProperty]
        private string? hostVersion;
        [ObservableProperty]
        public AntCollection? antDevices;

        public HomePageViewModel(IAntRadio antRadioService, AntCollection antCollection)
        {
            _antCollection = antCollection;
            _antRadioService = (AntRadioService)antRadioService;
            _ = Task.Run(() => { SearchAsync(); });
        }

        private async void SearchAsync()
        {
            try
            {
                IsBusy = true;
                await _antRadioService.FindAntRadioServerAsync();
                MainThread.BeginInvokeOnMainThread(() => IsBusy = false);
                ServerIPAddress = _antRadioService.ServerIPAddress;
                ProductDescription = _antRadioService.ProductDescription;
                SerialNumber = _antRadioService.SerialNumber;
                HostVersion = _antRadioService.Version;
                AntDevices = _antCollection;
                await AntDevices.StartScanning();
            }
            catch (OperationCanceledException)
            {
                // app is exiting
                Debug.WriteLine("SearchAsync: OperationCancelledException");
            }
        }

        [RelayCommand(CanExecute = nameof(CanShowRadioCapabilities))]
        private static async Task ShowRadioCapabilities()
        {
            await Shell.Current.GoToAsync("RadioCapabilities");
        }

        private bool CanShowRadioCapabilities()
        {
            return !IsBusy;
        }

        [RelayCommand]
        private static async Task ShowDetails(AntDevice antDevice)
        {
            switch (antDevice)
            {
                case Tracker:
                    await Shell.Current.GoToAsync("AssetTracker", new Dictionary<string, object> {
                    { "Sensor", (Tracker)antDevice },
                });
                    break;
                case StandardPowerSensor:
                    await Shell.Current.GoToAsync("BicyclePower", new Dictionary<string, object> {
                    { "Sensor", (BicyclePower)antDevice },
                });
                    break;
                case CrankTorqueFrequencySensor:
                    await Shell.Current.GoToAsync("CrankTorqueFrequency", new Dictionary<string, object> {
                    { "Sensor", (CrankTorqueFrequencySensor)antDevice },
                });
                    break;
                case BikeSpeedSensor:
                    await Shell.Current.GoToAsync("BikeSpeed", new Dictionary<string, object> {
                    { "Sensor", (BikeSpeedSensor)antDevice }
                });
                    break;
                case CombinedSpeedAndCadenceSensor:
                    await Shell.Current.GoToAsync("SpeedAndCadence", new Dictionary<string, object> {
                    { "Sensor", (CombinedSpeedAndCadenceSensor)antDevice }
                });
                    break;
                case BikeCadenceSensor:
                    await Shell.Current.GoToAsync("BikeCadence", new Dictionary<string, object> {
                    { "Sensor", (BikeCadenceSensor)antDevice }
                });
                    break;
                case FitnessEquipment:
                    await Shell.Current.GoToAsync("FitnessEquipment", new Dictionary<string, object> {
                    { "Sensor", (FitnessEquipment)antDevice }
                });
                    break;
                case Geocache:
                    await Shell.Current.GoToAsync("Geocache", new Dictionary<string, object> {
                    { "Sensor", (Geocache)antDevice }
                });
                    break;
                case HeartRate:
                    await Shell.Current.GoToAsync("HeartRate", new Dictionary<string, object> {
                    { "Sensor", (HeartRate)antDevice }
                });
                    break;
                case MuscleOxygen:
                    await Shell.Current.GoToAsync("MuscleOxygen", new Dictionary<string, object> {
                    { "Sensor", (MuscleOxygen)antDevice }
                });
                    break;
                case StrideBasedSpeedAndDistance:
                    await Shell.Current.GoToAsync("StrideBasedMonitor", new Dictionary<string, object> {
                    { "Sensor", (StrideBasedSpeedAndDistance)antDevice }
                });
                    break;
                case BikeRadar:
                    await Shell.Current.GoToAsync("BikeRadar", new Dictionary<string, object> {
                    { "Sensor", (BikeRadar)antDevice }
                });
                    break;
                case UnknownDevice:
                    await Shell.Current.GoToAsync("UnknownDevice", new Dictionary<string, object> {
                    { "Sensor", (UnknownDevice)antDevice }
                });
                    break;
                default:
                    return;
            }
        }
    }
}
