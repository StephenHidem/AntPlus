using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class HomePage : ContentPage
{
    private readonly HomePageViewModel _viewModel;

    public HomePage(HomePageViewModel viewModel)
    {
        BindingContext = _viewModel = viewModel;
        InitializeComponent();
    }

    private async void AntDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            _ = _viewModel.ShowDetailsCommand.ExecuteAsync(e.CurrentSelection[0]);
            await Task.Delay(100);      // HACK: wait for the UI to update
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}