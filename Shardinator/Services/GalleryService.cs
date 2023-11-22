using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shardinator.DataContracts.Interfaces;
using Shardinator.Services.Authentication;
using Shardinator.Services.Shardination;
using uplink.NET.Interfaces;
using uplink.NET.Services;

namespace Shardinator.Services;
public class GalleryService : IGalleryService
{
    private readonly ILocalSecretsStore _localSecretsStore;

    public GalleryService(ILocalSecretsStore localSecretsStore)
    {
        _localSecretsStore = localSecretsStore;
    }

    public async Task<List<string>> GetGalleryKeysAsync()
    {
        var bucketName = _localSecretsStore.GetSecret(StorjAuthenticationService.BUCKET);
        var accessGrant = _localSecretsStore.GetSecret(StorjAuthenticationService.ACCESS_GRANT);
        var bucketService = new BucketService(new uplink.NET.Models.Access(accessGrant));
        var objectService = new ObjectService(new uplink.NET.Models.Access(accessGrant));

        var bucket = await bucketService.GetBucketAsync(bucketName);
        var thumbnailEntries = await objectService.ListObjectsAsync(bucket, new uplink.NET.Models.ListObjectsOptions { Prefix = ShardinatorService.THUMB_PREFIX, Recursive = true });

        var result = new List<string>();
        foreach (var thumbnailEntry in thumbnailEntries.Items)
        {
            result.Add(thumbnailEntry.Key);
        }

        return result;
    }
}
