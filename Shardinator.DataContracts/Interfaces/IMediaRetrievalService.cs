using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shardinator.DataContracts.Models;

namespace Shardinator.DataContracts.Interfaces;

public class MediaEventArgs : EventArgs
{
    public MediaReference Media { get; }
    public MediaEventArgs(MediaReference media)
    {
        Media = media;
    }
}
public interface IMediaRetrievalService
{
    event EventHandler<MediaEventArgs> OnMediaReferenceLoaded;
    bool IsLoading { get; }

    Task<IList<MediaReference>> GetMediaReferencesAsync(int shardinationDays, CancellationToken? cancelToken = null);
}
