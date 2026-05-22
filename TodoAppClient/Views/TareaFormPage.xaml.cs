using TodoAppClient.ViewModels;

namespace TodoAppClient.Views;

public partial class TareaFormPage : ContentPage
{
    public TareaFormPage(TareaFormViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
