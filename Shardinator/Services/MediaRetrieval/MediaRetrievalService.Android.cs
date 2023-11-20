#if __ANDROID__

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Database;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using AndroidX.Core.App;
using Shardinator.DataContracts.Interfaces;
using Shardinator.DataContracts.Models;
using Uno.UI.ViewManagement;
using Windows.Media.Devices;

namespace Shardinator.Services.MediaRetrieval;

public partial class MediaRetrievalService : IMediaRetrievalService
{
    public static Activity CurrentActivity;

    static TaskCompletionSource<bool> mediaPermissionTcs;

    public const int RequestMedia = 1354;


    public async Task<IList<MediaReference>> NativeGetMediaReferencesAsync(CancellationToken? cancelToken = null)
    {
        stopLoad = false;

        if (!cancelToken.HasValue)
            cancelToken = CancellationToken.None;

        // We create a TaskCompletionSource of decimal
        var taskCompletionSource = new TaskCompletionSource<IList<MediaReference>>();

        // Registering a lambda into the cancellationToken
        cancelToken.Value.Register(() =>
        {
            // We received a cancellation message, cancel the TaskCompletionSource.Task
            stopLoad = true;
            taskCompletionSource.TrySetCanceled();
        });

        _isLoading = true;

        var task = LoadMediaAsync();

        // Wait for the first task to finish among the two
        var completedTask = await Task.WhenAny(task, taskCompletionSource.Task);
        _isLoading = false;

        return await completedTask;
    }

    async void RequestMediaPermissions()
    {
        if (ActivityCompat.ShouldShowRequestPermissionRationale(CurrentActivity, Android.Manifest.Permission.WriteExternalStorage))
        {

            // Provide an additional rationale to the user if the permission was not granted
            // and the user would benefit from additional context for the use of the permission.
            // For example, if the request has been denied previously.

            //await UserDialogs.Instance.AlertAsync("Media Permission", "This action requires external storage permission", "Ok");
        }
        else
        {
            // Media permissions have not been granted yet. Request them directly.
            ActivityCompat.RequestPermissions(CurrentActivity, new string[] { Android.Manifest.Permission.WriteExternalStorage }, RequestMedia);
        }
    }

    public static void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
    {
        if (requestCode == RequestMedia)
        {
            // We have requested multiple permissions for Media, so all of them need to be
            // checked.
            if (VerifyPermissions(grantResults))
            {
                // All required permissions have been granted, display Media fragment.
                mediaPermissionTcs.TrySetResult(true);
            }
            else
            {
                mediaPermissionTcs.TrySetResult(false);
            }

        }
    }

    /**
		* Check that all given permissions have been granted by verifying that each entry in the
		* given array is of the value Permission.Granted.
		*
		* See Activity#onRequestPermissionsResult (int, String[], int[])
		*/
    public static bool VerifyPermissions(Android.Content.PM.Permission[] grantResults)
    {
        // At least one result must be checked.
        if (grantResults.Length < 1)
            return false;

        // Verify that each required permission has been granted, otherwise return false.
        foreach (Android.Content.PM.Permission result in grantResults)
        {
            if (result != Android.Content.PM.Permission.Granted)
            {
                return false;
            }
        }
        return true;
    }

    public async Task<bool> RequestPermissionAsync()
    {
        mediaPermissionTcs = new TaskCompletionSource<bool>();
        // Verify that all required Media permissions have been granted.
        if (Uno.UI.ContextHelper.Current.CheckSelfPermission(Android.Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted)
        {
            // Media permissions have not been granted.
            RequestMediaPermissions();
        }
        else
        {
            // Media permissions have been granted. 
            mediaPermissionTcs.TrySetResult(true);
        }

        return await mediaPermissionTcs.Task;
    }

    async Task<IList<MediaReference>> LoadMediaAsync()
    {
        IList<MediaReference> assets = new List<MediaReference>();
        var hasPermission = await RequestPermissionAsync();
        if (hasPermission)
        {
            var uri = MediaStore.Files.GetContentUri("external");
            var ctx = Uno.UI.ContextHelper.Current;
            await Task.Run(async () =>
            {
                var cursor = ctx.ApplicationContext.ContentResolver.Query(uri, new string[]
                {
                        MediaStore.Files.FileColumns.Id,
                        MediaStore.Files.FileColumns.Data,
                        MediaStore.Files.FileColumns.DateAdded,
                        MediaStore.Files.FileColumns.MediaType,
                        MediaStore.Files.FileColumns.MimeType,
                        MediaStore.Files.FileColumns.Title,
                        MediaStore.Files.FileColumns.Parent,
                        MediaStore.Files.FileColumns.DisplayName,
                        MediaStore.Files.FileColumns.Size
                }, $"{MediaStore.Files.FileColumns.MediaType} = {(int)MediaType.Image} OR {MediaStore.Files.FileColumns.MediaType} = {(int)MediaType.Video}", null, $"{MediaStore.Files.FileColumns.DateAdded} ASC");
                if (cursor.Count > 0)
                {
                    while (cursor.MoveToNext())
                    {
                        try
                        {
                            var id = cursor.GetInt(cursor.GetColumnIndex(MediaStore.Files.FileColumns.Id));
                            var mediaType = cursor.GetInt(cursor.GetColumnIndex(MediaStore.Files.FileColumns.MediaType));
                            Bitmap bitmap = null;
                            switch (mediaType)
                            {
                                case (int)MediaType.Image:
                                    bitmap = MediaStore.Images.Thumbnails.GetThumbnail(Uno.UI.ContextHelper.Current.ContentResolver, id, ThumbnailKind.MiniKind, new BitmapFactory.Options()
                                    {
                                        InSampleSize = 4,
                                        InPurgeable = true
                                    });
                                    break;
                                case (int)MediaType.Video:
                                    bitmap = MediaStore.Video.Thumbnails.GetThumbnail(Uno.UI.ContextHelper.Current.ContentResolver, id, VideoThumbnailKind.MiniKind, new BitmapFactory.Options()
                                    {
                                        InSampleSize = 4,
                                        InPurgeable = true
                                    });
                                    break;
                            }
                            var tmpPath = System.IO.Path.GetTempPath();
                            var name = GetString(cursor, MediaStore.Files.FileColumns.DisplayName);
                            var filePath = System.IO.Path.Combine(tmpPath, $"tmp_{name}");

                            var path = GetString(cursor, MediaStore.Files.FileColumns.Data);
                            var created = GetString(cursor, MediaStore.Files.FileColumns.DateAdded);

                            //filePath = path;
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                bitmap?.Compress(Bitmap.CompressFormat.Png, 100, stream);
                                stream.Close();
                            }


                            if (!string.IsNullOrWhiteSpace(filePath))
                            {
                                var asset = new MediaReference()
                                {
                                    Id = $"{id}",
                                    Type = mediaType == (int)MediaType.Video ? MediaReferenceTypes.Video: MediaReferenceTypes.Image,
                                    Name = name,
                                    PreviewPath = filePath,
                                    Path = path
                                };

                                using (var h = new Handler(Looper.MainLooper))
                                    h.Post(async () => { OnMediaReferenceLoaded?.Invoke(this, new MediaEventArgs(asset)); });

                                assets.Add(asset);
                            }

                            if (assets.Count >= 10)
                                break;

                            if (stopLoad)
                                break;
                        }
                        catch (Exception ex)
                        {
                            //await UserDialogs.Instance.AlertAsync(ex.StackTrace.ToString(), "error", "ok");
                        }
                    }
                }
            });
        }
        return assets;
    }


    string GetString(ICursor cursor, string key)
    {
        return cursor.GetString(cursor.GetColumnIndex(key));
    }
}
#endif
