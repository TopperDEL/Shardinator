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
    [Property] private bool _isLoading;
    [Property] private ObservableCollection<MediaReference> _images = new ObservableCollection<MediaReference>();

    partial void OnInitialize()
    {
        MediaRetrievalService.OnMediaReferenceLoaded += MediaRetrievalService_OnMediaReferenceLoaded;

        _ = LoadMediaAsync();
    }

    private void MediaRetrievalService_OnMediaReferenceLoaded(object? sender, MediaEventArgs e)
    {
        try
        {
            Dispatcher.TryEnqueue(() => Images.Add(e.Media));
        }
        catch (Exception ex)
        {

        }
    }

    [Command]
    private async Task Shardinate()
    {
        IsLoading = true;
        try
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
        }
        finally
        {
            IsLoading = false;
        }
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
