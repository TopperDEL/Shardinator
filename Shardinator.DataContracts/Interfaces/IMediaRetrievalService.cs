using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shardinator.DataContracts.Models;

namespace Shardinator.DataContracts.Interfaces;
public interface IMediaRetrievalService
{
    ValueTask<List<MediaReference>> GetMediaReferencesAsync();
}
