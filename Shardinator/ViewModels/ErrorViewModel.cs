using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmGen;

namespace Shardinator.ViewModels;

[ViewModel]
[Inject(typeof(INavigator))]
public partial class ErrorViewModel
{
    [Property] string _errorMessage;

    [Command]
    private async Task Ok()
    {
        await Navigator.NavigateBackAsync(this);
    }
}
