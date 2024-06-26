using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shardinator.DataContracts.Models;

public enum MediaReferenceTypes
{
    Image,
    Video
}

[Bindable(BindableSupport.Default)]
public class MediaReference
{
    public string Id { get; set; }
    public string Name { get; set; }
    public MediaReferenceTypes Type { get; set; }
    public string Path { get; set; }
    public DateTime CreationDate { get; set; }
    public Stream MediaStream { get; set; }
    public Stream ThumbnailStream { get; set; }
    public string MediaURI { get; set; }
    public long Size { get; set; }
    public string SizeInMB { get; set; }
}
