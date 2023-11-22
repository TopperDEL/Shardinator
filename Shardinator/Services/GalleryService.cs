using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shardinator.DataContracts.Interfaces;

namespace Shardinator.Services;
public class GalleryService : IGalleryService
{
    public async Task<List<string>> GetGalleryKeysAsync()
    {
        return new List<string>();
    }
}
