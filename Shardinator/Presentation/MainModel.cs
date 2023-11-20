using System.Collections.ObjectModel;
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

        Images = new ObservableCollection<MediaReference>();
        _mediaRetrievalService.OnMediaReferenceLoaded += _mediaRetrievalService_OnMediaReferenceLoaded;
        _ = _mediaRetrievalService.GetMediaReferencesAsync();
    }

    private void _mediaRetrievalService_OnMediaReferenceLoaded(object? sender, MediaEventArgs e)
    {
        Images.Add(e.Media);
    }

    public string? Title { get; }

    public IState<string> Name => State<string>.Value(this, () => string.Empty);

    public ObservableCollection<MediaReference> Images { get; set; }

    public async Task ShardinateCommand()
    {

    }

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
