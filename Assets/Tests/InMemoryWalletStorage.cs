using System.Collections.Generic;
using VertigoWheel.Core;

namespace VertigoWheel.Tests
{
    internal sealed class InMemoryWalletStorage : IWalletStorage
    {
        private readonly Dictionary<string, int> _savedBalances = new Dictionary<string, int>();
        private int _saveCount;

        public int SaveCount
        {
            get { return _saveCount; }
        }

        public Dictionary<string, int> Load()
        {
            Dictionary<string, int> copy = new Dictionary<string, int>();

            foreach (KeyValuePair<string, int> balance in _savedBalances)
            {
                copy[balance.Key] = balance.Value;
            }

            return copy;
        }

        public void Save(IReadOnlyDictionary<string, int> balances)
        {
            _savedBalances.Clear();

            foreach (KeyValuePair<string, int> balance in balances)
            {
                _savedBalances[balance.Key] = balance.Value;
            }

            _saveCount++;
        }
    }
}