using System;
using System.Collections.Generic;
using UnityEngine;
using VertigoWheel.Core;

namespace VertigoWheel.Infrastructure
{
    public sealed class PlayerPrefsWalletStorage : IWalletStorage
    {
        private const string SaveKey = "VertigoWheel.Wallet";

        public Dictionary<string, int> Load()
        {
            Dictionary<string, int> balances = new Dictionary<string, int>();

            if (!PlayerPrefs.HasKey(SaveKey))
            {
                return balances;
            }

            WalletSaveData saveData = JsonUtility.FromJson<WalletSaveData>(PlayerPrefs.GetString(SaveKey));

            if (saveData == null || saveData.Entries == null)
            {
                return balances;
            }

            for (int i = 0; i < saveData.Entries.Count; i++)
            {
                WalletSaveEntry entry = saveData.Entries[i];
                balances[entry.Id] = entry.Amount;
            }

            return balances;
        }

        public void Save(IReadOnlyDictionary<string, int> balances)
        {
            WalletSaveData saveData = new WalletSaveData();

            foreach (KeyValuePair<string, int> balance in balances)
            {
                saveData.Entries.Add(new WalletSaveEntry(balance.Key, balance.Value));
            }

            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(saveData));
            PlayerPrefs.Save();
        }

        [Serializable]
        private class WalletSaveData
        {
            public List<WalletSaveEntry> Entries = new List<WalletSaveEntry>();
        }

        [Serializable]
        private class WalletSaveEntry
        {
            public string Id;
            public int Amount;

            public WalletSaveEntry(string id, int amount)
            {
                Id = id;
                Amount = amount;
            }
        }
    }
}