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
    public async ValueTask<List<MediaReference>> NativeGetMediaReferencesAsync()
    {
        var result = new List<MediaReference>();

        string defaultImageFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        if (Directory.Exists(defaultImageFolderPath))
        {
            try
            {
                // Get all image files in the folder
                string[] imageFiles = Directory.GetFiles(defaultImageFolderPath, "*.*", SearchOption.AllDirectories)
                    .Where(file => file.ToLower().EndsWith("jpg") || file.ToLower().EndsWith("jpeg") || file.ToLower().EndsWith("png") || file.ToLower().EndsWith("gif") || file.ToLower().EndsWith("bmp"))
                    .ToArray();
                foreach (var image in imageFiles)
                {
                    result.Add(new MediaReference { Name = image, Type = MediaReferenceTypes.Image });
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
                .ToArray();
            foreach (var video in videoFiles)
            {
                result.Add(new MediaReference { Name = video, Type = MediaReferenceTypes.Video });
            }
        }

        return result;
    }
}
#endif