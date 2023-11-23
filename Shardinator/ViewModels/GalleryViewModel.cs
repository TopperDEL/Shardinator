using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmGen;
using Shardinator.DataContracts.Interfaces;
using Shardinator.DataContracts.Models;
using Shardinator.Helper;
using System.ComponentModel;
using CommunityToolkit.WinUI.Collections;

namespace Shardinator.ViewModels;

[Bindable(BindableSupport.Default)]
[ViewModel]
[Inject(typeof(IGalleryService))]
public partial class GalleryViewModel
{
    [Property] private IncrementalLoadingCollection<GallerySource, GalleryEntry> _collection = new IncrementalLoadingCollection<GallerySource, GalleryEntry>();


    [Command]
    private async Task Refresh()
    {
        var newKeys = await GalleryService.GetGalleryKeysAsync();
        GallerySource.Refresh(newKeys);
        Collection = new IncrementalLoadingCollection<GallerySource, GalleryEntry>();
    }
}
