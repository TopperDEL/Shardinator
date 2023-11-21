using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmGen;
using Shardinator.DataContracts.Interfaces;

namespace Shardinator.ViewModels;

[ViewModel]
[Inject(typeof(IGalleryService))]
public partial class GalleryViewModel
{

}
