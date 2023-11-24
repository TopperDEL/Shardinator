using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmGen;
using Shardinator.DataContracts.Interfaces;
using Shardinator.Services.Authentication;
using Uno;
using uplink.NET.Interfaces;
using uplink.NET.Models;
using uplink.NET.Services;

namespace Shardinator.ViewModels
{
    [ViewModel]
    [Inject(typeof(ILocalSecretsStore))]
    public partial class VideoDetailViewModel
    {
        private static Bucket _bucket;
        private static IObjectService _objectService;
        [Property] private string _videoThumbKey;
        [Property] private string _videoKey;

        public async Task<Stream> InitStreamAsync()
        {
            var bucketName = LocalSecretsStore.GetSecret(StorjAuthenticationService.BUCKET);
            var accessGrant = LocalSecretsStore.GetSecret(StorjAuthenticationService.ACCESS_GRANT);
            var access = new Access(accessGrant);
            var bucketService = new BucketService(access);
            _bucket = await bucketService.GetBucketAsync(bucketName);
            _objectService = new ObjectService(access);
            
            var objectInfo = await _objectService.GetObjectAsync(_bucket, VideoKey);

            return new DownloadStream(_bucket, (int)objectInfo.SystemMetadata.ContentLength, VideoKey);
        }
    }
}
