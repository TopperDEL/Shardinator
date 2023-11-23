using System.ComponentModel;
using MvvmGen;

namespace Shardinator.ViewModels;

[Bindable(BindableSupport.Default)]
[ViewModel]
[Inject(typeof(INavigator))]
[Inject(typeof(IAuthenticationService))]
public partial class ShellViewModel
{
    partial void OnInitialize()
    {
        AuthenticationService.LoggedOut += LoggedOut;
    }

    private async void LoggedOut(object? sender, EventArgs e)
    {
        await Navigator.NavigateViewModelAsync<LoginViewModel>(this, qualifier: Qualifiers.ClearBackStack);
    }
}
