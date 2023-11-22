using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using Shardinator.DataContracts.Interfaces;
using Shardinator.Services.Authentication;
using uplink.NET.Interfaces;
using uplink.NET.Models;
using uplink.NET.Services;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage.Streams;

namespace Shardinator.Converter;
public class StreamToLazyBitmapImageConverter : IValueConverter
{
    private static Bucket _bucket;
    private static IObjectService _objectService;
    private static IDispatcher _dispatcher;
    private static IMemoryCache _memoryCache;

    public static async Task InitAsync(ILocalSecretsStore localSecretsStore, IDispatcher dispatcher, IMemoryCache memoryCache)
    {
        var bucketName = localSecretsStore.GetSecret(StorjAuthenticationService.BUCKET);
        var accessGrant = localSecretsStore.GetSecret(StorjAuthenticationService.ACCESS_GRANT);
        var access = new Access(accessGrant);
        var bucketService = new BucketService(access);
        _bucket = await bucketService.GetBucketAsync(bucketName);
        _objectService = new ObjectService(access);
        _dispatcher = dispatcher;
        _memoryCache = memoryCache;
    }

    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (_objectService == null)
        {
            return null;
        }
        if (value == null)
        {
            return null;
        }
        var image = new BitmapImage();
        _ = Task.Run(() => LoadImageAsync(value as string, image));
        return image;
    }

    private async Task LoadImageAsync(string key, BitmapImage image)
    {
        var objectInfo = await _objectService.GetObjectAsync(_bucket, key);
        if (objectInfo.SystemMetadata.ContentLength > 0)
        {
            var bytes = _memoryCache.Get<byte[]>(key);
            if (bytes == null)
            {
                using (var downloadOperation = await _objectService.DownloadObjectAsync(_bucket, key, new DownloadOptions(), false))
                {
                    await downloadOperation.StartDownloadAsync();
                    if (downloadOperation.Completed)
                    {
                        _memoryCache.Set(key, downloadOperation.DownloadedBytes, DateTime.Now.AddMinutes(10));
                        bytes = downloadOperation.DownloadedBytes;
                    }
                }
            }
            _dispatcher.TryEnqueue(() => image.SetSourceAsync(new MemoryStream(bytes).AsRandomAccessStream()));
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
