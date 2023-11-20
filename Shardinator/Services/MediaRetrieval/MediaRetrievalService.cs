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

    public async Task<IList<MediaReference>> GetMediaReferencesAsync(CancellationToken? cancelToken = null)
    {
        return await NativeGetMediaReferencesAsync(cancelToken);
    }
}
