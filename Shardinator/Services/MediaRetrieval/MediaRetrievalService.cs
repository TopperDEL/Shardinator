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
        var result = await NativeGetMediaReferencesAsync(shardinationDays, cancelToken);
        foreach(var media in result)
        {
            if(media.Size > 0)
            {
                //Convert byte-size to human readable MB:
                var sizeInMB = media.Size / 1024 / 1024;
                media.SizeInMB = $"{sizeInMB} MB";
            }
        }
        return result;
    }

    public void InformOSAboutShardedFile(string path)
    {
#if __ANDROID__
        InformOSAboutShardedFile_Android (path);
#endif

#if __IOS__
        InformOSAboutShardedFile_iOs(path);
#endif
    }
}
