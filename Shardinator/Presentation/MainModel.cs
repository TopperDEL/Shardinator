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
        IShardinatorService shardinatorService,
        INavigator navigator)
    {
        _navigator = navigator;
        _authentication = authentication;
        _mediaRetrievalService = mediaRetrievalService;
        _shardinatorService = shardinatorService;
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";

        _mediaRetrievalService.OnMediaReferenceLoaded += _mediaRetrievalService_OnMediaReferenceLoaded;
        _ = LoadMediaAsync();
    }

    private void _mediaRetrievalService_OnMediaReferenceLoaded(object? sender, MediaEventArgs e)
    {
        Images.Add(e.Media);
    }

    public string? Title { get; }

    public IState<string> Name => State<string>.Value(this, () => string.Empty);

    public ObservableCollection<MediaReference> Images { get; set; } = new ObservableCollection<MediaReference>();

    public async Task ShardinateCommand()
    {
        var shardinated = await _shardinatorService.ShardinateAsync(Images.First());
        if (shardinated)
        {
            try
            {
                Images.Clear();
            }
            catch { }
            await LoadMediaAsync();
        }
    }

    private async Task LoadMediaAsync()
    {
        await _mediaRetrievalService.GetMediaReferencesAsync();
    }

    public async ValueTask Logout(CancellationToken token)
    {
        await _authentication.LogoutAsync(token);
    }

    private IAuthenticationService _authentication;
    private IMediaRetrievalService _mediaRetrievalService;
    private IShardinatorService _shardinatorService;
}
