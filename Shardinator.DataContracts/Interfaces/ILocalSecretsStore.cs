using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shardinator.DataContracts.Interfaces;
public interface ILocalSecretsStore
{
    bool SetSecret(string secretKey, string secretValue);
    string GetSecret(string secretKey);
    void ClearSecret(string secretKey);
}
