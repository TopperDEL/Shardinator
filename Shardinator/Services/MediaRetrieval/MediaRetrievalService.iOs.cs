#if __IOS__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using Foundation;
using Photos;
using Shardinator.DataContracts.Interfaces;
using Shardinator.DataContracts.Models;
using UIKit;
using Windows.Media.Devices;

namespace Shardinator.Services.MediaRetrieval;

public partial class MediaRetrievalService : IMediaRetrievalService
{
    bool requestStop = false;

    public async Task<IList<MediaReference>> NativeGetMediaReferencesAsync(CancellationToken? cancelToken = null)
    {
        requestStop = false;

        if (!cancelToken.HasValue)
            cancelToken = CancellationToken.None;

        // We create a TaskCompletionSource of decimal
        var taskCompletionSource = new TaskCompletionSource<IList<MediaReference>>();

        // Registering a lambda into the cancellationToken
        cancelToken.Value.Register(() =>
        {
            requestStop = true;
            taskCompletionSource.TrySetCanceled();
        });

        _isLoading = true;

        var task = LoadMediaAsync();

        // Wait for the first task to finish among the two
        var completedTask = await Task.WhenAny(task, taskCompletionSource.Task);
        _isLoading = false;

        return await completedTask;
    }

    public async Task<bool> RequestPermissionAsync()
    {
        var status = PHPhotoLibrary.AuthorizationStatus;

        bool authotization = status == PHAuthorizationStatus.Authorized;

        if (!authotization)
        {
            authotization = await PHPhotoLibrary.RequestAuthorizationAsync() == PHAuthorizationStatus.Authorized;
        }
        return authotization;

    }

    async Task<IList<MediaReference>> LoadMediaAsync()
    {
        IList<MediaReference> assets = new List<MediaReference>();
        var imageManager = new PHCachingImageManager();
        var hasPermission = await RequestPermissionAsync();
        if (hasPermission)
        {
            await Task.Run(async () =>
            {
                var thumbnailRequestOptions = new PHImageRequestOptions();
                thumbnailRequestOptions.ResizeMode = PHImageRequestOptionsResizeMode.Fast;
                thumbnailRequestOptions.DeliveryMode = PHImageRequestOptionsDeliveryMode.FastFormat;
                thumbnailRequestOptions.NetworkAccessAllowed = true;
                thumbnailRequestOptions.Synchronous = true;

                var requestOptions = new PHImageRequestOptions();
                requestOptions.ResizeMode = PHImageRequestOptionsResizeMode.Exact;
                requestOptions.DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat;
                requestOptions.NetworkAccessAllowed = true;
                requestOptions.Synchronous = true;

                var fetchOptions = new PHFetchOptions();
                fetchOptions.SortDescriptors = new NSSortDescriptor[] { new NSSortDescriptor("creationDate", false) };
                fetchOptions.Predicate = NSPredicate.FromFormat($"mediaType == {(int)PHAssetMediaType.Image} || mediaType == {(int)PHAssetMediaType.Video}");
                var fetchResults = PHAsset.FetchAssets(fetchOptions);
                var tmpPath = Path.GetTempPath();
                var allAssets = fetchResults.Select(p => p as PHAsset).ToArray();
                var thumbnailSize = new CGSize(300.0f, 300.0f);

                imageManager.StartCaching(allAssets, thumbnailSize, PHImageContentMode.AspectFit, thumbnailRequestOptions);
                imageManager.StartCaching(allAssets, PHImageManager.MaximumSize, PHImageContentMode.AspectFit, requestOptions);


                foreach (var result in fetchResults)
                {
                    var phAsset = (result as PHAsset);
                    var name = PHAssetResource.GetAssetResources(phAsset)?.FirstOrDefault()?.OriginalFilename;
                    var asset = new MediaReference()
                    {
                        Id = phAsset.LocalIdentifier,
                        Name = name,
                        Type = phAsset.MediaType == PHAssetMediaType.Image ? MediaReferenceTypes.Image : MediaReferenceTypes.Video,
                    };

                    imageManager.RequestImageForAsset(phAsset, thumbnailSize, PHImageContentMode.AspectFit, thumbnailRequestOptions, (image, info) =>
                    {

                        if (image != null)
                        {
                            NSData imageData = null;
                            if (image.CGImage.RenderingIntent == CGColorRenderingIntent.Default)
                            {
                                imageData = image.AsJPEG(0.8f);

                            }
                            else
                            {
                                imageData = image.AsPNG();
                            }

                            if (imageData != null)
                            {

                                var fileName = Path.Combine(tmpPath, $"tmp_thumbnail_{Path.GetFileNameWithoutExtension(name)}.jpg");
                                NSError error = null;
                                imageData.Save(fileName, true, out error);
                                if (error == null)
                                {


                                    asset.PreviewPath = fileName;

                                }

                            }
                        }
                    });
                    switch (phAsset.MediaType)
                    {

                        case PHAssetMediaType.Image:

                            imageManager.RequestImageForAsset(phAsset, PHImageManager.MaximumSize, PHImageContentMode.AspectFit, requestOptions, (image, info) =>
                            {

                                if (image != null)
                                {
                                    NSData imageData = null;
                                    if (image.CGImage.RenderingIntent == CGColorRenderingIntent.Default)
                                    {
                                        imageData = image.AsJPEG(0.8f);

                                    }
                                    else
                                    {
                                        imageData = image.AsPNG();
                                    }

                                    if (imageData != null)
                                    {
                                        var fileName = Path.Combine(tmpPath, $"tmp_{name}");
                                        NSError error = null;
                                        imageData.Save(fileName, true, out error);
                                        if (error == null)
                                        {
                                            asset.Path = fileName;
                                        }

                                    }
                                }
                            });
                            break;
                        case PHAssetMediaType.Video:
                            var videoRequestOptions = new PHVideoRequestOptions();
                            videoRequestOptions.NetworkAccessAllowed = true;
                            var tcs = new TaskCompletionSource<bool>();
                            imageManager.RequestAVAsset(phAsset, null, (vAsset, audioMix, info) =>
                            {
                                var avAsset = vAsset as AVUrlAsset;
                                var avData = NSData.FromUrl(avAsset.Url);
                                NSError error = null;
                                var path = Path.Combine(tmpPath, $"tmp_{name}");
                                avData.Save(path, true, out error);
                                if (error == null)
                                {
                                    asset.Path = path;


                                    tcs.TrySetResult(true);
                                }
                                else
                                {
                                    tcs.TrySetResult(false);
                                }
                            });
                            await tcs.Task;
                            break;
                    }

                    UIApplication.SharedApplication.InvokeOnMainThread(delegate
                    {
                        OnMediaReferenceLoaded?.Invoke(this, new MediaEventArgs(asset));
                    });
                    assets.Add(asset);

                    if (requestStop)
                        break;
                }
            });

            imageManager.StopCaching();
        }

        return assets;
    }
}
#endif
