using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shardinator.DataContracts.Interfaces;
using Shardinator.DataContracts.Models;
using Shardinator.Services.Authentication;
using uplink.NET.Models;
using uplink.NET.Services;

namespace Shardinator.Services.Shardination;
public class ShardinatorService : IShardinatorService
{
    public const string THUMB_PREFIX = "thumb/";
    private readonly ILocalSecretsStore _localSecretsStore;
    private CancellationToken _token;

    public ShardinatorService(ILocalSecretsStore localSecretsStore)
    {
        _localSecretsStore = localSecretsStore;
    }

    public async Task<bool> ShardinateAsync(MediaReference media, CancellationToken cancellationToken)
    {
        _token = cancellationToken;
        try
        {
            string mediaType = media.Type.ToString();
            string targetPath = media.CreationDate.Year + "/" + media.CreationDate.Month.ToString("D2") + "/" + media.CreationDate.Day.ToString("D2") + "/" + media.Name;

            var thumbnailShardinated = await ShardinateAsync(THUMB_PREFIX + targetPath.Replace("mp4", "png"), media.ThumbnailStream, mediaType);
            if (thumbnailShardinated)
            {
                var fileShardinated = await ShardinateAsync(targetPath, media.MediaStream, mediaType);
                if (!fileShardinated)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (!_token.IsCancellationRequested)
            {
                File.Delete(media.Path);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> ShardinateAsync(string targetPath, Stream fileData, string mediaType)
    {
        CustomMetadata customMetadata = new CustomMetadata();
        customMetadata.Entries.Add(new CustomMetadataEntry { Key = "MediaType", Value = mediaType });
        fileData.Position = 0;
        using (Access access = new Access(_localSecretsStore.GetSecret(StorjAuthenticationService.ACCESS_GRANT)))
        {
            //ToDo: Check Disposing
            var bucketService = new BucketService(access);
            var bucket = await bucketService.GetBucketAsync(_localSecretsStore.GetSecret(StorjAuthenticationService.BUCKET)).ConfigureAwait(false);
            var objectService = new ObjectService(access);
            var upload = await objectService.UploadObjectAsync(bucket, targetPath, new UploadOptions(), fileData, customMetadata, false).ConfigureAwait(false);
            upload.UploadOperationProgressChanged += Upload_UploadOperationProgressChanged;
            if (!_token.IsCancellationRequested)
            {
                await upload.StartUploadAsync().ConfigureAwait(false);

                if (!upload.Completed)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void Upload_UploadOperationProgressChanged(UploadOperation uploadOperation)
    {
        if (_token != null && _token.IsCancellationRequested)
        {
            uploadOperation.Cancel();
        }
    }
}
