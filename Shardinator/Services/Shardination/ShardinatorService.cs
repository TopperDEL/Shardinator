using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shardinator.DataContracts.Interfaces;
using Shardinator.DataContracts.Models;
using Shardinator.Services.Authentication;
using uplink.NET.Interfaces;
using uplink.NET.Models;
using uplink.NET.Services;

namespace Shardinator.Services.Shardination;
public class ShardinatorService : IShardinatorService
{
    public const string THUMB_PREFIX = "thumb/";
    private readonly ILocalSecretsStore _localSecretsStore;
    private CancellationToken _token;
    private Access _access;
    private IBucketService _bucketService;
    private IObjectService _objectService;
    private Bucket _bucket;
    private bool _isInitialized;

    public ShardinatorService(ILocalSecretsStore localSecretsStore)
    {
        _localSecretsStore = localSecretsStore;
    }

    private async Task InitAsync()
    {
        if (_isInitialized)
        {
            return;
        }
        _access = new Access(_localSecretsStore.GetSecret(StorjAuthenticationService.ACCESS_GRANT));
        _bucketService = new BucketService(_access);
        _bucket = await _bucketService.GetBucketAsync(_localSecretsStore.GetSecret(StorjAuthenticationService.BUCKET)).ConfigureAwait(false);
        _objectService = new ObjectService(_access);

        _isInitialized = true;
    }

    public void Clear()
    {
        if (_bucket != null)
        {
            _bucket.Dispose();
            _bucket = null;
        }

        if (_access != null)
        {
            _access.Dispose();
            _access = null;
        }

        _objectService = null;
        _bucketService = null;
        _isInitialized = false;
    }

    public async Task<Tuple<bool, string>> ShardinateAsync(MediaReference media, CancellationToken cancellationToken)
    {
        await InitAsync();

        _token = cancellationToken;
        try
        {
            string mediaType = media.Type.ToString();
            string targetPath = $"{media.CreationDate.Year}/{media.CreationDate.Month:D2}/{media.CreationDate.Day:D2}/{media.Name}";

            if(media.ThumbnailStream == null || media.MediaStream == null)
            {
                return new Tuple<bool, string>(false, "Media or thumbnail stream is null");
            }
            var thumbnailShardinated = await ShardinateAsync(THUMB_PREFIX + targetPath.Replace("mp4", "png"), media.ThumbnailStream, mediaType, _objectService, _bucket);

            if (thumbnailShardinated)
            {
                var fileShardinated = await ShardinateAsync(targetPath, media.MediaStream, mediaType, _objectService, _bucket);

                if (!fileShardinated)
                {
                    return new Tuple<bool, string>(false, "Could not shardinate media");
                }
            }
            else
            {
                return new Tuple<bool, string>(false, "Could not shardinate thumbnail");
            }

            if (!_token.IsCancellationRequested)
            {
                var objectInfo = await _objectService.GetObjectAsync(_bucket, targetPath);
                if (objectInfo.SystemMetadata.ContentLength == media.MediaStream.Length)
                {
                    File.Delete(media.Path);
                    return new Tuple<bool, string>(true, "");
                }
                else
                {
                    return new Tuple<bool, string>(false, "Shardinated media has not the same content length - cancelling");
                }
            }
            else
            {
                return new Tuple<bool, string>(false, "Cancellation requested");
            }
        }
        catch (Exception ex)
        {
            // Log the exception here
            return new Tuple<bool, string>(false, "Error occurred: " + ex.Message);
        }
    }

    private async Task<bool> ShardinateAsync(string targetPath, Stream fileData, string mediaType, IObjectService objectService, Bucket bucket)
    {
        CustomMetadata customMetadata = new CustomMetadata();
        customMetadata.Entries.Add(new CustomMetadataEntry { Key = "MediaType", Value = mediaType });
        fileData.Position = 0;

        var upload = await objectService.UploadObjectAsync(bucket, targetPath, new UploadOptions(), fileData, customMetadata, false).ConfigureAwait(false);
        upload.UploadOperationProgressChanged += Upload_UploadOperationProgressChanged;
        try
        {
            if (!_token.IsCancellationRequested)
            {
                await upload.StartUploadAsync().ConfigureAwait(false);

                if (!upload.Completed)
                {
                    return false;
                }
            }
            return true;
        }
        finally
        {
            // Unsubscribe to avoid potential memory leaks
            upload.UploadOperationProgressChanged -= Upload_UploadOperationProgressChanged;
        }
    }


    private void Upload_UploadOperationProgressChanged(UploadOperation uploadOperation)
    {
        if (_token != null && _token.IsCancellationRequested)
        {
            uploadOperation.Cancel();
        }
    }
}
