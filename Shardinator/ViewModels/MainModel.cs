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
public partial class MainModel
{
    partial void OnInitialize()
    {
        MediaRetrievalService.OnMediaReferenceLoaded += MediaRetrievalService_OnMediaReferenceLoaded;
        _ = LoadMediaAsync();
    }

    private void MediaRetrievalService_OnMediaReferenceLoaded(object? sender, MediaEventArgs e)
    {
        Images.Add(e.Media);
    }

    public ObservableCollection<MediaReference> Images { get; set; } = new ObservableCollection<MediaReference>();

    public async Task ShardinateCommand()
    {
        var shardinated = await ShardinatorService.ShardinateAsync(Images.First());
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
        await MediaRetrievalService.GetMediaReferencesAsync();
    }

    public async ValueTask Logout(CancellationToken token)
    {
        await AuthenticationService.LogoutAsync(token);
    }
}
