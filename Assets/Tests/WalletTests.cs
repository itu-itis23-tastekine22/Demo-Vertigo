using System;
using System.Collections.Generic;
using NUnit.Framework;
using VertigoWheel.Core;
using VertigoWheel.Events;

namespace VertigoWheel.Tests
{
    public class WalletTests
    {
        [Test]
        public void Deposit_IsRestoredByANewWalletInstance()
        {
            InMemoryWalletStorage storage = new InMemoryWalletStorage();
            Wallet wallet = new Wallet(storage, new EventBus());

            wallet.Deposit("Reward_Gold", 25);
            Wallet reloadedWallet = new Wallet(storage, new EventBus());

            Assert.AreEqual(25, reloadedWallet.GetBalance("Reward_Gold"));
        }

        [Test]
        public void Deposit_NewCurrencyIds_AreSupportedWithoutCodeChanges()
        {
            Wallet wallet = new Wallet(new InMemoryWalletStorage(), new EventBus());

            wallet.Deposit("Currency_Gem", 3);
            wallet.Deposit("Currency_Ticket", 7);

            Assert.AreEqual(3, wallet.GetBalance("Currency_Gem"));
            Assert.AreEqual(7, wallet.GetBalance("Currency_Ticket"));
        }

        [Test]
        public void DepositAll_AddsEverythingAndSavesOnce()
        {
            InMemoryWalletStorage storage = new InMemoryWalletStorage();
            Wallet wallet = new Wallet(storage, new EventBus());
            List<KeyValuePair<string, int>> deposits = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("Reward_Gold", 10),
                new KeyValuePair<string, int>("Reward_Cash", 50)
            };

            wallet.DepositAll(deposits);

            Assert.AreEqual(10, wallet.GetBalance("Reward_Gold"));
            Assert.AreEqual(50, wallet.GetBalance("Reward_Cash"));
            Assert.AreEqual(1, storage.SaveCount);
        }

        [Test]
        public void Deposit_NonPositiveAmount_Throws()
        {
            Wallet wallet = new Wallet(new InMemoryWalletStorage(), new EventBus());

            Assert.Throws<ArgumentOutOfRangeException>(() => wallet.Deposit("Reward_Gold", 0));
        }
    }
}