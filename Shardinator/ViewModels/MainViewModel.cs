using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Extensions.Caching.Memory;
using MvvmGen;
using Shardinator.Converter;
using Shardinator.DataContracts.Interfaces;
using Shardinator.DataContracts.Models;
using Shardinator.Helper;

namespace Shardinator.ViewModels;

[Bindable(BindableSupport.Default)]
[ViewModel]
[Inject(typeof(INavigator))]
[Inject(typeof(IStringLocalizer))]
[Inject(typeof(IAuthenticationService))]
[Inject(typeof(IMediaRetrievalService))]
[Inject(typeof(IShardinatorService))]
[Inject(typeof(IDispatcher))]
[Inject(typeof(IGalleryService))]
[Inject(typeof(ILocalSecretsStore))]
[Inject(typeof(IMemoryCache))]
public partial class MainViewModel
{
    [Property] private bool _isShardinating;
    [Property] private bool _canEditSettings = true;
    [Property] private bool _isCancelling;
    [Property] private ObservableCollection<MediaReference> _images = new ObservableCollection<MediaReference>();
    [Property] private GalleryViewModel _gallery;
    [Property] private bool _showShardination = true;
    [Property] private bool _showGallery;
    [Property] private bool _showSettings;
    [Property] private int _selectedRegionIndex = 0;
    [Property] private int _shardinationDays = 365;

    partial void OnInitialize()
    {
        MediaRetrievalService.OnMediaReferenceLoaded += MediaRetrievalService_OnMediaReferenceLoaded;

        _ = LoadMediaAsync();

        Gallery = new GalleryViewModel(GalleryService);

        _ = StreamToLazyBitmapImageConverter.InitAsync(LocalSecretsStore, Dispatcher, MemoryCache);
    }

    private void MediaRetrievalService_OnMediaReferenceLoaded(object? sender, MediaEventArgs e)
    {
        Dispatcher.TryEnqueue(() => Images.Add(e.Media));
    }

    CancellationTokenSource _cancellationSource;

    [Command]
    private void Shardinate()
    {
        _cancellationSource = new CancellationTokenSource();
        var token = _cancellationSource.Token;
        _ = ShardinateAsync(token);
    }

    private async Task ShardinateAsync(CancellationToken cancellationToken)
    {
        IsShardinating = true;
        CanEditSettings = false;

        try
        {
            while (Images.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                var shardinated = await ShardinatorService.ShardinateAsync(Images.First(), cancellationToken);
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
            CanEditSettings = true;
        }
    }

    [Command]
    private void StopShardinate()
    {
        IsCancelling = true;
        _cancellationSource.Cancel();
    }

    private async Task LoadMediaAsync()
    {
        await MediaRetrievalService.GetMediaReferencesAsync();
    }

    [Command]
    public async Task Logout()
    {
        await AuthenticationService.LogoutAsync(null);
    }

    public void ActiveRegionChanged()
    {
        if(SelectedRegionIndex == 0)
        {
            ShowShardination = true;
            ShowGallery = false;
            ShowSettings = false;
        }
        else if (SelectedRegionIndex == 1)
        {
            ShowShardination = false;
            ShowGallery = true;
            ShowSettings = false;
            Gallery.RefreshCommand.Execute(null);
        }
        else if (SelectedRegionIndex == 2)
        {
            ShowShardination = false;
            ShowGallery = false;
            ShowSettings = true;
        }
    }
}
