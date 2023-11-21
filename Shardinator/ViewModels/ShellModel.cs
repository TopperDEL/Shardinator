using MvvmGen;

namespace Shardinator.ViewModels;

[ViewModel]
[Inject(typeof(INavigator))]
[Inject(typeof(IAuthenticationService))]
public partial class ShellModel
{
    partial void OnInitialize()
    {
        AuthenticationService.LoggedOut += LoggedOut;
    }

    private async void LoggedOut(object? sender, EventArgs e)
    {
        await Navigator.NavigateViewModelAsync<LoginModel>(this, qualifier: Qualifiers.ClearBackStack);
    }
}
