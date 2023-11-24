using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmGen;

namespace Shardinator.ViewModels
{
    [ViewModel]
    public partial class ImageDetailViewModel
    {
        [Property] private string _imageThumbKey;
        [Property] private string _imageKey;
    }
}
