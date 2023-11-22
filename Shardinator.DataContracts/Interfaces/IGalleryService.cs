using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shardinator.DataContracts.Interfaces;
public interface IGalleryService
{
    Task<List<string>> GetGalleryKeysAsync();


    //IAsyncEnumerable<>
    //https://daninacan.com/how-to-use-iasyncenumerable-in-c-for-paged-apis/
    //https://markheath.net/post/async-enumerable-1
}
