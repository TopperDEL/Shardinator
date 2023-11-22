using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmGen;
using Shardinator.DataContracts.Interfaces;
using Shardinator.DataContracts.Models;
using Shardinator.Helper;
using CommunityToolkit.WinUI;

namespace Shardinator.ViewModels;

[ViewModel]
[Inject(typeof(IGalleryService))]
public partial class GalleryViewModel
{
    private IncrementalLoadingCollection<GallerySource, GalleryEntry> _galleryCollection;


    [Command]
    private async Task Refresh()
    {
    }
}
