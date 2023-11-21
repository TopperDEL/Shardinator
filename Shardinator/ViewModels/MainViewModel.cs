using System.Collections.ObjectModel;
using MvvmGen;
using Shardinator.DataContracts.Interfaces;
using Shardinator.DataContracts.Models;

namespace Shardinator.ViewModels;

[ViewModel]
[Inject(typeof(INavigator))]
[Inject(typeof(IStringLocalizer))]
[Inject(typeof(IAuthenticationService))]
[Inject(typeof(IMediaRetrievalService))]
[Inject(typeof(IShardinatorService))]
[Inject(typeof(IDispatcher))]
public partial class MainViewModel
{
    [Property] private bool _isShardinating;
    [Property] private bool _isCancelling;
    [Property] private ObservableCollection<MediaReference> _images = new ObservableCollection<MediaReference>();

    partial void OnInitialize()
    {
        MediaRetrievalService.OnMediaReferenceLoaded += MediaRetrievalService_OnMediaReferenceLoaded;

        _ = LoadMediaAsync();
    }

    private void MediaRetrievalService_OnMediaReferenceLoaded(object? sender, MediaEventArgs e)
    {
        Dispatcher.TryEnqueue(() => Images.Add(e.Media));
    }

    CancellationTokenSource _cancellationSource;

    [Command]
    private async Task Shardinate()
    {
        _cancellationSource = new CancellationTokenSource();
        var token = _cancellationSource.Token;
        _ = Shardinate(token);
    }

    private async Task Shardinate(CancellationToken cancellationToken)
    {
        IsShardinating = true;

        try
        {
            while (Images.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                var shardinated = await ShardinatorService.ShardinateAsync(Images.First());
                if (shardinated)
                {
                    try
                    {
                        Dispatcher.TryEnqueue(Images.Clear);
                    }
                    catch { }
                    await LoadMediaAsync();
                }
                else
                {
                    //Show error
                }
            }
        }
        finally
        {
            IsShardinating = false;
            IsCancelling = false;
        }
    }

    [Command]
    private async Task StopShardinate()
    {
        IsCancelling = true;
        _cancellationSource.Cancel();
    }

    private async Task LoadMediaAsync()
    {
        await MediaRetrievalService.GetMediaReferencesAsync();
    }

    public async ValueTask Logout(CancellationToken token)
    {
        await AuthenticationService.LogoutAsync(token);
    }
}
