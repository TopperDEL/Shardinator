using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shardinator.DataContracts.Interfaces;

namespace Shardinator.Services.Authentication;
public class LocalSecretsStore : ILocalSecretsStore
{
    private const string RESOURCE = "Shardinator";

    public void ClearSecret(string secretKey)
    {
        try
        {
            var vault = new Windows.Security.Credentials.PasswordVault();
            var credential = vault.Retrieve(RESOURCE, secretKey);
            vault.Remove(credential);
        }
        catch
        {
            //Ignore
        }
    }

    public string GetSecret(string secretKey)
    {
        try
        {
            var vault = new Windows.Security.Credentials.PasswordVault();
            var credential = vault.Retrieve(RESOURCE, secretKey);
            credential.RetrievePassword();
            return credential.Password;
        }
        catch
        {
            return string.Empty;
        }
    }

    public bool SetSecret(string secretKey, string secretValue)
    {
        try
        {
            var vault = new Windows.Security.Credentials.PasswordVault();
            vault.Add(new Windows.Security.Credentials.PasswordCredential(RESOURCE, secretKey, secretValue));
            return true;
        }
        catch
        {
            return false;
        }
    }
}
