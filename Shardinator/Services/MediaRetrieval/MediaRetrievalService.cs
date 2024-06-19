using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shardinator.DataContracts.Interfaces;
using Shardinator.DataContracts.Models;

namespace Shardinator.Services.MediaRetrieval;
public partial class MediaRetrievalService : IMediaRetrievalService
{
    bool stopLoad = false;

    bool _isLoading = false;
    public bool IsLoading => _isLoading;

    public event EventHandler<MediaEventArgs> OnMediaReferenceLoaded;

    public async Task<IList<MediaReference>> GetMediaReferencesAsync(int shardinationDays, CancellationToken? cancelToken = null)
    {
        return await NativeGetMediaReferencesAsync(shardinationDays, cancelToken);
    }

    private void CalculateSizeInMB(MediaReference mediaReference)
    {
        if (mediaReference.Size > 0)
        {
            //Convert byte-size to human readable MB:
            var sizeInMB = mediaReference.Size / 1024 / 1024;
            mediaReference.SizeInMB = $"{sizeInMB} MB";
        }
    }

    public void InformOSAboutShardedFile(string path)
    {
#if __ANDROID__
        InformOSAboutShardedFile_Android(path);
#endif

#if __IOS__
        InformOSAboutShardedFile_iOs(path);
#endif
    }
}
