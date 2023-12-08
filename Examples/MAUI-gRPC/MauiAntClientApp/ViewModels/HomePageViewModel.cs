using CommunityToolkit.Mvvm.ComponentModel;
using MauiAntClientApp.Services;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntRadioInterface;
using System.Net;

namespace MauiAntClientApp.ViewModels
{
    public partial class HomePageViewModel(ILogger<HomePageViewModel> logger, IAntRadio antRadioService, IServiceProvider services) : ObservableObject
    {
        private readonly ILogger<HomePageViewModel> _logger = logger;
        private readonly AntRadioService _antRadioService = (AntRadioService)antRadioService;
        private readonly IServiceProvider _services = services;

        [ObservableProperty]
        private bool isBusy;
        [ObservableProperty]
        private IPAddress? serverIPAddress;
        [ObservableProperty]
        private string? productDescription;
        [ObservableProperty]
        private string? serialString;
        [ObservableProperty]
        private string? hostVersion;
        [ObservableProperty]
        public AntDeviceCollection? antDevices;

        public async Task SearchAsync()
        {
            _logger.LogInformation($"{nameof(SearchAsync)}");
            IsBusy = true;
            await _antRadioService.FindAntRadioServerAsync();
            IsBusy = false;
            ServerIPAddress = _antRadioService.ServerIPAddress;
            ProductDescription = _antRadioService.ProductDescription;
            SerialString = _antRadioService.SerialString;
            HostVersion = _antRadioService.HostVersion;
            AntDevices = _services.GetRequiredService<AntDeviceCollection>();
        }
    }
}
