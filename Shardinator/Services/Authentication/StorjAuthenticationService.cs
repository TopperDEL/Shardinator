using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shardinator.DataContracts.Interfaces;

namespace Shardinator.Services.Authentication;
public class StorjAuthenticationService : IAuthenticationService
{
    public const string BUCKET = "Bucket";
    public const string ACCESS_GRANT = "AccessGrant";
    public string[] Providers => new[] { "Storj" };

    public event EventHandler LoggedOut;

    private readonly ILocalSecretsStore _localSecretsStore;

    public StorjAuthenticationService(ILocalSecretsStore localSecretsStore)
    {
        _localSecretsStore = localSecretsStore;
    }

    public async ValueTask<bool> IsAuthenticated(CancellationToken? cancellationToken = null)
    {
        return _localSecretsStore.GetSecret(BUCKET) != string.Empty;
    }

    public async ValueTask<bool> LoginAsync(IDispatcher? dispatcher, IDictionary<string, string>? credentials = null, string? provider = null, CancellationToken? cancellationToken = null)
    {
        var bucket = credentials["Bucket"];
        var accessGrant = credentials["AccessGrant"];
        if(string.IsNullOrEmpty(bucket) || string.IsNullOrEmpty(accessGrant))
        {
            return false;
        }

        _localSecretsStore.SetSecret(BUCKET, bucket);
        _localSecretsStore.SetSecret(ACCESS_GRANT, accessGrant);
        
        return true;
    }

    public async ValueTask<bool> LogoutAsync(IDispatcher? dispatcher, CancellationToken? cancellationToken = null)
    {
        _localSecretsStore.ClearSecret(BUCKET);
        _localSecretsStore.ClearSecret(ACCESS_GRANT);
        LoggedOut?.Invoke(this, new EventArgs());
        return true;
    }

    public async ValueTask<bool> RefreshAsync(CancellationToken? cancellationToken = null)
    {
        return _localSecretsStore.GetSecret(BUCKET) != string.Empty;
    }
}
