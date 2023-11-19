using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shardinator.DataContracts.Models;

public enum MediaReferenceTypes
{
    Image,
    Video
}

public class MediaReference
{
    public string Filename { get; set; }
    public MediaReferenceTypes MediaReferenceType { get; set; }
}
