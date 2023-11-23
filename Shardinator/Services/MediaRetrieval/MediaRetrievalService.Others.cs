#if !__ANDROID__ && !WINDOWS && !__IOS__
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
    public async Task<IList<MediaReference>> NativeGetMediaReferencesAsync(int shardinationDays,CancellationToken? cancelToken = null)
    {
        var result = new List<MediaReference>();
        return result;
    }
}
#endif
