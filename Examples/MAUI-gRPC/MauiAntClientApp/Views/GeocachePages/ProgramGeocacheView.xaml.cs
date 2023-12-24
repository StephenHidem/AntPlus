using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views.GeocachePages;

public partial class ProgramGeocacheView : ContentView
{
    public static readonly BindableProperty GeocacheViewModelProperty =
   BindableProperty.Create(nameof(GeocacheViewModel), typeof(GeocacheViewModel), typeof(ProgramGeocacheView));
    public GeocacheViewModel GeocacheViewModel
    {
        get => (GeocacheViewModel)GetValue(GeocacheViewModelProperty);
        set => SetValue(GeocacheViewModelProperty, value);
    }

    public ProgramGeocacheView()
    {
        InitializeComponent();
    }
}