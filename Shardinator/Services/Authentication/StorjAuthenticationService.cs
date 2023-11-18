using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shardinator.Services.Authentication;
public class StorjAuthenticationService : IAuthenticationService
{
    public string[] Providers => new[] { "Storj" };

    public event EventHandler LoggedOut;

    public async ValueTask<bool> IsAuthenticated(CancellationToken? cancellationToken = null)
    {
        return false;
    }

    public async ValueTask<bool> LoginAsync(IDispatcher? dispatcher, IDictionary<string, string>? credentials = null, string? provider = null, CancellationToken? cancellationToken = null)
    {
        return true;
    }

    public async ValueTask<bool> LogoutAsync(IDispatcher? dispatcher, CancellationToken? cancellationToken = null)
    {
        LoggedOut?.Invoke(this, new EventArgs());
        return true;
    }

    public async ValueTask<bool> RefreshAsync(CancellationToken? cancellationToken = null)
    {
        return true;
    }
}
