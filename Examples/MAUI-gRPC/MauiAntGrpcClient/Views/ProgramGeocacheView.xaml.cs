using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Views;

public partial class ProgramGeocacheView : ContentView
{
    public static readonly BindableProperty ProgramGeocacheProperty =
        BindableProperty.Create(
            nameof(ProgramGeocache),
            typeof(GeocacheViewModel),
            typeof(ProgramGeocacheView));

    public GeocacheViewModel ProgramGeocache
    {
        get => (GeocacheViewModel)GetValue(ProgramGeocacheProperty);
        set => SetValue(ProgramGeocacheProperty, value);
    }

    public ProgramGeocacheView()
    {
        InitializeComponent();
    }
}