using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shardinator.DataContracts.Models;
using CommunityToolkit.Common.Collections;

namespace Shardinator.Helper
{
    public class GallerySource : IIncrementalSource<GalleryEntry>
    {
        private readonly List<string> _elementKeys;

        public GallerySource()
        {
            _elementKeys = new List<string>();
        }

        public async Task<IEnumerable<GalleryEntry>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            List<GalleryEntry> result = new List<GalleryEntry>();

            var keys = (from p in _elementKeys
                          select p).Skip(pageIndex * pageSize).Take(pageSize);

            foreach(var key in keys)
            {
                result.Add(new GalleryEntry());
            }

            return result;
        }
    }
}
