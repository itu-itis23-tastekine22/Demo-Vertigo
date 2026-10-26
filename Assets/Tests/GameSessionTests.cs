using NUnit.Framework;
using VertigoWheel.Core;
using VertigoWheel.Data;
using VertigoWheel.Events;

namespace VertigoWheel.Tests
{
    // Random values are consumed in this order: bomb placement when a bomb zone is built,
    // then the spin result. Safe zones are built without consuming a value.
    public class GameSessionTests
    {
        private const int GoldAmount = 10;

        private TestAssetFactory _factory;
        private RewardData _gold;
        private ZoneSettings _zoneSettings;
        private InMemoryWalletStorage _walletStorage;
        private Wallet _wallet;

        [SetUp]
        public void SetUp()
        {
            _factory = new TestAssetFactory();
            _gold = _factory.CreateReward("Reward_Gold", true);

            WheelConfig wheel = _factory.CreateWheel("Wheel", _gold, GoldAmount);
            ZoneTypeData normalZone = _factory.CreateZoneType("Zone_Normal", 1, 0, true, false, (1, wheel));
            ZoneTypeData safeZone = _factory.CreateZoneType("Zone_Safe", 2, 1, false, true, (2, wheel));

            _zoneSettings = _factory.CreateZoneSettings(0f, normalZone, safeZone);
            _walletStorage = new InMemoryWalletStorage();
        }

        [TearDown]
        public void TearDown()
        {
            _factory.DestroyAll();
        }

        [Test]
        public void StartNewRun_StartsAtFirstZoneInIdleState()
        {
            GameSession session = CreateSession(0);

            session.StartNewRun();

            Assert.AreEqual(1, session.CurrentZone);
            Assert.AreEqual(RunState.Idle, session.State);
            Assert.IsTrue(session.CanSpin);
        }

        [Test]
        public void CompleteSpin_OnReward_AddsRewardAndAdvancesZone()
        {
            GameSession session = CreateSession(0, 1);
            session.StartNewRun();

            SpinAndComplete(session);

            Assert.AreEqual(2, session.CurrentZone);
            Assert.AreEqual(RunState.Idle, session.State);
            Assert.AreEqual(GoldAmount, session.Inventory.Items[_gold]);
        }

        [Test]
        public void CompleteSpin_OnBomb_EndsRunAndClearsRewardsImmediately()
        {
            GameSession session = CreateSession(0, 1, 2, 4, 4);
            session.StartNewRun();
            SpinAndComplete(session);
            SpinAndComplete(session);
            Assert.IsFalse(session.Inventory.IsEmpty);

            SpinAndComplete(session);

            Assert.AreEqual(RunState.Dead, session.State);
            Assert.IsTrue(session.Inventory.IsEmpty);
        }

        [Test]
        public void DeadRun_CannotSpinOrLeave()
        {
            GameSession session = CreateSession(0, 0);
            session.StartNewRun();
            SpinAndComplete(session);

            Assert.AreEqual(RunState.Dead, session.State);
            Assert.IsFalse(session.TryStartSpin(out _));
            Assert.IsFalse(session.TryLeave());
        }

        [Test]
        public void WhileSpinning_CannotSpinAgainOrLeave()
        {
            GameSession session = CreateSession(0, 1);
            session.StartNewRun();

            Assert.IsTrue(session.TryStartSpin(out _));

            Assert.AreEqual(RunState.Spinning, session.State);
            Assert.IsFalse(session.TryStartSpin(out _));
            Assert.IsFalse(session.CanLeave);
        }

        [Test]
        public void TryLeave_OnNormalZone_IsRejected()
        {
            GameSession session = CreateSession(0);
            session.StartNewRun();

            Assert.IsFalse(session.TryLeave());
            Assert.AreEqual(RunState.Idle, session.State);
        }

        [Test]
        public void TryLeave_OnSafeZone_BanksRewardsAndEndsRun()
        {
            GameSession session = CreateSession(0, 1);
            session.StartNewRun();
            SpinAndComplete(session);

            Assert.IsTrue(session.TryLeave());

            Assert.AreEqual(RunState.Left, session.State);
            Assert.IsTrue(session.Inventory.IsEmpty);
            Assert.AreEqual(GoldAmount, _wallet.GetBalance(_gold.Id));
        }

        private GameSession CreateSession(params int[] randomValues)
        {
            IEventBus eventBus = new EventBus();
            IRandomProvider randomProvider = new QueueRandomProvider(randomValues);

            _wallet = new Wallet(_walletStorage, eventBus);

            return new GameSession(
                new WheelBuilder(_zoneSettings, randomProvider),
                new SpinResolver(randomProvider),
                new RewardInventory(eventBus),
                _wallet,
                eventBus);
        }

        private static void SpinAndComplete(GameSession session)
        {
            Assert.IsTrue(session.TryStartSpin(out _));
            session.CompleteSpin();
        }
    }
}