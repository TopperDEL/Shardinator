using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Extensions.Caching.Memory;
using MvvmGen;
using Shardinator.Converter;
using Shardinator.DataContracts.Interfaces;
using Shardinator.DataContracts.Models;
using Shardinator.Helper;
using Uno;

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
    [Property] private int _shardinationDays;

    partial void OnInitialize()
    {
        MediaRetrievalService.OnMediaReferenceLoaded += MediaRetrievalService_OnMediaReferenceLoaded;

        Gallery = new GalleryViewModel(GalleryService);

        var savedShardinationDays = LocalSecretsStore.GetSecret("ShardinationDays");
        ShardinationDays = string.IsNullOrEmpty(savedShardinationDays) ? 365 : int.Parse(savedShardinationDays);

        _ = StringToLazyBitmapImageConverter.InitAsync(LocalSecretsStore, Dispatcher, MemoryCache);
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
                if (shardinated.Item1)
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
                    await Navigator.NavigateViewModelAsync<ErrorViewModel>(this, qualifier: Qualifiers.Dialog, data: shardinated.Item2);
                    //Don't shardinate further
                    return;
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
        await MediaRetrievalService.GetMediaReferencesAsync(ShardinationDays);
    }

    [Command]
    public async Task Logout()
    {
        ShardinatorService.Clear();
        await AuthenticationService.LogoutAsync(null);
    }

    public async Task SaveShardinationDaysAsync(double newValue)
    {
        ShardinationDays = (int)newValue;
        LocalSecretsStore.SetSecret("ShardinationDays", ShardinationDays.ToString());

        Images.Clear();
        await LoadMediaAsync();
    }

    public void ActiveRegionChanged()
    {
        if (SelectedRegionIndex == 0)
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

    [Command]
    private async Task ShowDetail(object param)
    {
        string key = param as string;
        if (key.ToLower().Contains("mp4"))
        {
#if !__ANDROID__
            await Navigator.NavigateViewModelAsync<VideoDetailViewModel>(this, qualifier: Qualifiers.Dialog, data: key);
#endif
        }
        else
        {
            await Navigator.NavigateViewModelAsync<ImageDetailViewModel>(this, qualifier: Qualifiers.Dialog, data: key);
        }
    }
}
