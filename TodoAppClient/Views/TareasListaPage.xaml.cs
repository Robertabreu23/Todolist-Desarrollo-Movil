using TodoAppClient.ViewModels;

namespace TodoAppClient.Views;

public partial class TareasListaPage : ContentPage
{
    private readonly TareasListaViewModel _vm;

    public TareasListaPage(TareasListaViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
