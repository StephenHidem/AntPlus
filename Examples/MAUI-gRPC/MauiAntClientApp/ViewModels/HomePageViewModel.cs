using CommunityToolkit.Mvvm.ComponentModel;
using MauiAntClientApp.Services;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MauiAntClientApp.ViewModels
{
    public partial class HomePageViewModel(ILogger<HomePageViewModel> logger, AntRadioService antRadioService) : ObservableObject
    {
        private readonly ILogger<HomePageViewModel> _logger = logger;
        private readonly AntRadioService _antRadioService = antRadioService;

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
        }
    }
}
