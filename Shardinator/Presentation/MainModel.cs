using Shardinator.DataContracts.Interfaces;
using Shardinator.DataContracts.Models;

namespace Shardinator.Presentation;

public partial record MainModel
{
    private INavigator _navigator;

    public MainModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        IAuthenticationService authentication,
        IMediaRetrievalService mediaRetrievalService,
        INavigator navigator)
    {
        _navigator = navigator;
        _authentication = authentication;
        _mediaRetrievalService = mediaRetrievalService;
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
    }

    public string? Title { get; }

    public IState<string> Name => State<string>.Value(this, () => string.Empty);

    public IListFeed<MediaReference> Images => Feed
            .Async(async ct => await _mediaRetrievalService.GetMediaReferencesAsync())
            .Select(list => list.ToImmutableList())
            .AsListFeed();

    public async Task GoToSecond()
    {
        var name = await Name;
        await _navigator.NavigateViewModelAsync<SecondModel>(this, data: new Entity(name!));
    }

    public async ValueTask Logout(CancellationToken token)
    {
        await _authentication.LogoutAsync(token);
    }

    private IAuthenticationService _authentication;
    private IMediaRetrievalService _mediaRetrievalService;
}
