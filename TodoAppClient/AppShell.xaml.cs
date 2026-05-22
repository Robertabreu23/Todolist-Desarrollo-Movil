using TodoAppClient.Views;

namespace TodoAppClient;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(TareaFormPage), typeof(TareaFormPage));
	}
}
