namespace Shardinator.Presentation;

public partial record LoginModel(IDispatcher Dispatcher, INavigator Navigator, IAuthenticationService Authentication)
{
    public string Title { get; } = "Shardinator - Login";

    public IState<string> Bucket => State<string>.Value(this, () => string.Empty);

    public IState<string> AccessGrant => State<string>.Value(this, () => string.Empty);

    public async ValueTask Login(CancellationToken token)
    {
        var bucket = await Bucket ?? string.Empty;
        var accessGrant = await AccessGrant ?? string.Empty;

        var success = await Authentication.LoginAsync(Dispatcher, new Dictionary<string, string> { { nameof(Bucket), bucket}, { nameof(AccessGrant), accessGrant} });
        if (success)
        {
            await Navigator.NavigateViewModelAsync<MainModel>(this, qualifier: Qualifiers.ClearBackStack);
        }
    }

}
