using MvvmGen;

namespace Shardinator.ViewModels;

[ViewModel]
[Inject(typeof(IDispatcher))]
[Inject(typeof(INavigator))]
[Inject(typeof(IAuthenticationService))]
public partial class LoginModel
{
    [Property] string _title = "Shardinator - Login";

    [Property] private string _bucket;
    [Property] private string _accessGrant;

    [Command]
    private async Task Login()
    {
        var success = await AuthenticationService.LoginAsync(Dispatcher, new Dictionary<string, string> { { nameof(Bucket), Bucket.ToLower()}, { nameof(AccessGrant), AccessGrant} });
        if (success)
        {
            await Navigator.NavigateViewModelAsync<MainModel>(this, qualifier: Qualifiers.ClearBackStack);
        }
    }

}
