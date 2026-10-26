using System.Collections.Generic;

namespace VertigoWheel.Core
{
    public interface IWalletStorage
    {
        Dictionary<string, int> Load();

        void Save(IReadOnlyDictionary<string, int> balances);
    }
}