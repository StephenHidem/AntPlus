using SmallEarthTech.AntPlus;

namespace MauiAntGrpcClient.Views;

public partial class CommonDataPagesView : ContentView
{
    public static readonly BindableProperty CommonDataPagesProperty =
        BindableProperty.Create(
            nameof(CommonDataPages),
            typeof(CommonDataPages),
            typeof(CommonDataPagesView));
    public CommonDataPages CommonDataPages
    {
        get => (CommonDataPages)GetValue(CommonDataPagesProperty);
        set => SetValue(CommonDataPagesProperty, value);
    }

    public CommonDataPagesView()
    {
        InitializeComponent();
    }
}