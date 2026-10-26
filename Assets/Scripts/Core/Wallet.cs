using System;
using System.Collections.Generic;
using VertigoWheel.Events;

namespace VertigoWheel.Core
{
    public sealed class Wallet : IWallet
    {
        private readonly Dictionary<string, int> _balances;
        private readonly IWalletStorage _storage;
        private readonly IEventBus _eventBus;

        public Wallet(IWalletStorage storage, IEventBus eventBus)
        {
            if (storage == null)
            {
                throw new ArgumentNullException(nameof(storage));
            }

            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }

            _storage = storage;
            _eventBus = eventBus;
            _balances = storage.Load();
        }

        public IReadOnlyDictionary<string, int> Balances
        {
            get { return _balances; }
        }

        public int GetBalance(string itemId)
        {
            if (_balances.TryGetValue(itemId, out int balance))
            {
                return balance;
            }

            return 0;
        }

        public void Deposit(string itemId, int amount)
        {
            AddToBalance(itemId, amount);
            SaveAndNotify();
        }

        public void DepositAll(IReadOnlyList<KeyValuePair<string, int>> deposits)
        {
            if (deposits.Count == 0)
            {
                return;
            }

            for (int i = 0; i < deposits.Count; i++)
            {
                AddToBalance(deposits[i].Key, deposits[i].Value);
            }

            SaveAndNotify();
        }

        private void AddToBalance(string itemId, int amount)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                throw new ArgumentException("Item id cannot be empty.", nameof(itemId));
            }

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Deposit amount must be positive.");
            }

            _balances[itemId] = GetBalance(itemId) + amount;
        }

        private void SaveAndNotify()
        {
            _storage.Save(_balances);
            _eventBus.Publish(new WalletChangedEvent());
        }
    }
}