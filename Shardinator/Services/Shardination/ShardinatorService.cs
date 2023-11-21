using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shardinator.DataContracts.Interfaces;
using Shardinator.DataContracts.Models;
using Shardinator.Services.Authentication;
using uplink.NET.Services;

namespace Shardinator.Services.Shardination;
public class ShardinatorService : IShardinatorService
{
    private readonly ILocalSecretsStore _localSecretsStore;

    public ShardinatorService(ILocalSecretsStore localSecretsStore)
    {
        _localSecretsStore = localSecretsStore;
    }

    public async Task<bool> ShardinateAsync(MediaReference media)
    {
        try
        {
            string targetPath = media.CreationDate.Year + "/" + media.CreationDate.Month.ToString("D2") + "/" + media.CreationDate.Day.ToString("D2") + "/" + media.Name;

            using (uplink.NET.Models.Access access = new uplink.NET.Models.Access(_localSecretsStore.GetSecret(StorjAuthenticationService.ACCESS_GRANT)))
            {
                var bucketService = new BucketService(access);
                var bucket = await bucketService.GetBucketAsync(_localSecretsStore.GetSecret(StorjAuthenticationService.BUCKET)).ConfigureAwait(false);
                var objectService = new ObjectService(access);
                var upload = await objectService.UploadObjectAsync(bucket, targetPath, new uplink.NET.Models.UploadOptions(), media.MediaStream, false).ConfigureAwait(false);
                await upload.StartUploadAsync().ConfigureAwait(false);

                if (!upload.Completed)
                {
                    return false;
                }
            }

            File.Delete(media.Path);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
