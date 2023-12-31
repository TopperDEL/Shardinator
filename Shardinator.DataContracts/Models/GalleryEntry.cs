using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shardinator.DataContracts.Models;

[Bindable(BindableSupport.Default)]
public class GalleryEntry
{
    public string Key { get; set; }
    public Stream ThumbnailStream { get; set; }
}
