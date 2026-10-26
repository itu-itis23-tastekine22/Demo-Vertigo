using System.Collections.Generic;

namespace VertigoWheel.Core
{
    public interface IWallet
    {
        IReadOnlyDictionary<string, int> Balances { get; }

        int GetBalance(string itemId);

        void Deposit(string itemId, int amount);

        void DepositAll(IReadOnlyList<KeyValuePair<string, int>> deposits);
    }
}