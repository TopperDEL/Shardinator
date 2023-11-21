using System.Collections.ObjectModel;
using Shardinator.DataContracts.Interfaces;
using Shardinator.DataContracts.Models;

namespace Shardinator.ViewModels;

public partial record MainModel
{
    private INavigator _navigator;
    private IDispatcher _dispatcher;

    public MainModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        IAuthenticationService authentication,
        IMediaRetrievalService mediaRetrievalService,
        IShardinatorService shardinatorService,
        IDispatcher dispatcher,
        INavigator navigator)
    {
        _navigator = navigator;
        _authentication = authentication;
        _mediaRetrievalService = mediaRetrievalService;
        _shardinatorService = shardinatorService;
        _dispatcher = dispatcher;
        Title = "Shardinator";

        _mediaRetrievalService.OnMediaReferenceLoaded += _mediaRetrievalService_OnMediaReferenceLoaded;
        _ = LoadMediaAsync();
    }

    private void _mediaRetrievalService_OnMediaReferenceLoaded(object? sender, MediaEventArgs e)
    {
        try
        {
            _dispatcher.TryEnqueue(() => Images.Add(e.Media));
        }
        catch (Exception ex)
        {

        }
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
                _dispatcher.TryEnqueue(Images.Clear);
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
