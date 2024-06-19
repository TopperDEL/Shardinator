using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shardinator.DataContracts.Interfaces;
using Shardinator.DataContracts.Models;

namespace Shardinator.Services.MediaRetrieval;

#if WINDOWS
public partial class MediaRetrievalService : IMediaRetrievalService
{
    public async Task<IList<MediaReference>> NativeGetMediaReferencesAsync(int shardinationDays, CancellationToken? cancelToken = null)
    {
        var borderTime = DateTime.Now.AddDays(-shardinationDays);
        var result = new List<MediaReference>();

        string defaultImageFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        if (Directory.Exists(defaultImageFolderPath))
        {
            try
            {
                // Get all image files in the folder
                string[] imageFiles = Directory.GetFiles(defaultImageFolderPath, "*.*", SearchOption.AllDirectories)
                    .Where(file => file.ToLower().EndsWith("jpg") || file.ToLower().EndsWith("jpeg") || file.ToLower().EndsWith("png") || file.ToLower().EndsWith("gif") || file.ToLower().EndsWith("bmp"))
                    .Where(file => File.GetCreationTime(file) < borderTime)
                    .Take(50)
                    .ToArray();
                foreach (var image in imageFiles)
                {
                    var creationTime = File.GetCreationTime(image);

                    StorageFile storageFile = await StorageFile.GetFileFromPathAsync(image);

                    var media = new MediaReference
                    {
                        Id = Path.GetFileName(image),
                        Name = Path.GetFileName(image),
                        Type = MediaReferenceTypes.Image,
                        Path = image,
                        CreationDate = creationTime,
                        ThumbnailStream = (await storageFile.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem)).AsStream(),
                        MediaStream = File.OpenRead(image),
                        Size = new FileInfo(image).Length
                    };
                    CalculateSizeInMB(media);

                    result.Add(media);
                    OnMediaReferenceLoaded?.Invoke(this, new MediaEventArgs(media));
                }
            }
            catch (Exception ex)
            {

            }
        }

        string defaultVideoFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

        if (Directory.Exists(defaultVideoFolderPath))
        {
            // Get all video files in the folder
            string[] videoFiles = Directory.GetFiles(defaultImageFolderPath, "*.*", SearchOption.AllDirectories)
                .Where(file => file.ToLower().EndsWith("mp4"))
                .Where(file => File.GetCreationTime(file) < borderTime)
                    .Take(50)
                    .ToArray();
            foreach (var video in videoFiles)
            {
                var creationTime = File.GetCreationTime(video);

                StorageFile storageFile = await StorageFile.GetFileFromPathAsync(video);
                var media = new MediaReference
                {
                    Id = Path.GetFileName(video),
                    Name = Path.GetFileName(video),
                    Type = MediaReferenceTypes.Video,
                    Path = video,
                    CreationDate = creationTime,
                    MediaStream = File.OpenRead(video),
                    Size = new FileInfo(video).Length
                };
                result.Add(media);
                OnMediaReferenceLoaded?.Invoke(this, new MediaEventArgs(media));
            }
        }

        return result;
    }
}
#endif
