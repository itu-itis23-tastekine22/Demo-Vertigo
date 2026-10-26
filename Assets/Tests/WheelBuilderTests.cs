using System;
using NUnit.Framework;
using VertigoWheel.Core;
using VertigoWheel.Data;

namespace VertigoWheel.Tests
{
    public class WheelBuilderTests
    {
        private TestAssetFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new TestAssetFactory();
        }

        [TearDown]
        public void TearDown()
        {
            _factory.DestroyAll();
        }

        [Test]
        public void Build_ZoneWithBomb_PlacesExactlyOneBombAtRandomIndex()
        {
            RewardData gold = _factory.CreateReward("Reward_Gold", true);
            WheelConfig wheel = _factory.CreateWheel("Wheel", gold, 10);
            ZoneTypeData normalZone = _factory.CreateZoneType("Zone_Normal", 1, 0, true, false, (1, wheel));
            ZoneSettings settings = _factory.CreateZoneSettings(0f, normalZone);
            WheelBuilder builder = new WheelBuilder(settings, new QueueRandomProvider(3));

            RuntimeWheel runtimeWheel = builder.Build(1);

            Assert.AreEqual(3, runtimeWheel.BombIndex);
            Assert.IsTrue(runtimeWheel.Slices[3].IsBomb);
            Assert.AreEqual(1, CountBombs(runtimeWheel));
        }

        [Test]
        public void Build_ZoneWithoutBomb_HasNoBombAndUsesNoRandomness()
        {
            RewardData gold = _factory.CreateReward("Reward_Gold", true);
            WheelConfig wheel = _factory.CreateWheel("Wheel", gold, 10);
            ZoneTypeData safeZone = _factory.CreateZoneType("Zone_Safe", 1, 0, false, true, (1, wheel));
            ZoneSettings settings = _factory.CreateZoneSettings(0f, safeZone);
            WheelBuilder builder = new WheelBuilder(settings, new QueueRandomProvider());

            RuntimeWheel runtimeWheel = builder.Build(1);

            Assert.AreEqual(RuntimeWheel.NoBombIndex, runtimeWheel.BombIndex);
            Assert.AreEqual(0, CountBombs(runtimeWheel));
        }

        [Test]
        public void Build_MultipleTiers_UsesHighestTierNotAboveZone()
        {
            RewardData gold = _factory.CreateReward("Reward_Gold", true);
            WheelConfig earlyWheel = _factory.CreateWheel("Wheel_Early", gold, 10);
            WheelConfig lateWheel = _factory.CreateWheel("Wheel_Late", gold, 50);
            ZoneTypeData normalZone = _factory.CreateZoneType("Zone_Normal", 1, 0, false, false, (1, earlyWheel), (11, lateWheel));
            ZoneSettings settings = _factory.CreateZoneSettings(0f, normalZone);
            WheelBuilder builder = new WheelBuilder(settings, new QueueRandomProvider());

            Assert.AreSame(earlyWheel, builder.Build(10).Config);
            Assert.AreSame(lateWheel, builder.Build(11).Config);
            Assert.AreSame(lateWheel, builder.Build(25).Config);
        }

        [Test]
        public void Build_ScalableReward_AmountGrowsWithZone()
        {
            RewardData gold = _factory.CreateReward("Reward_Gold", true);
            WheelConfig wheel = _factory.CreateWheel("Wheel", gold, 10);
            ZoneTypeData zoneType = _factory.CreateZoneType("Zone", 1, 0, false, false, (1, wheel));
            ZoneSettings settings = _factory.CreateZoneSettings(0.1f, zoneType);
            WheelBuilder builder = new WheelBuilder(settings, new QueueRandomProvider());

            Assert.AreEqual(20, builder.Build(11).Slices[0].Amount);
        }

        [Test]
        public void Build_SingleItemReward_AmountIsNotMultiplied()
        {
            RewardData sniper = _factory.CreateReward("Reward_Weapon_Sniper_T3", false);
            WheelConfig wheel = _factory.CreateWheel("Wheel", sniper, 1);
            ZoneTypeData zoneType = _factory.CreateZoneType("Zone", 1, 0, false, false, (1, wheel));
            ZoneSettings settings = _factory.CreateZoneSettings(0.1f, zoneType);
            WheelBuilder builder = new WheelBuilder(settings, new QueueRandomProvider());

            Assert.AreEqual(1, builder.Build(30).Slices[0].Amount);
        }

        [Test]
        public void Build_WheelWithEmptySlice_ThrowsAtRuntime()
        {
            WheelConfig brokenWheel = _factory.CreateWheel("Wheel_Broken", null, 0);
            ZoneTypeData zoneType = _factory.CreateZoneType("Zone", 1, 0, true, false, (1, brokenWheel));
            ZoneSettings settings = _factory.CreateZoneSettings(0f, zoneType);
            WheelBuilder builder = new WheelBuilder(settings, new QueueRandomProvider(0));

            Assert.Throws<InvalidOperationException>(() => builder.Build(1));
        }

        private static int CountBombs(RuntimeWheel runtimeWheel)
        {
            int bombCount = 0;

            for (int i = 0; i < runtimeWheel.Slices.Count; i++)
            {
                if (runtimeWheel.Slices[i].IsBomb)
                {
                    bombCount++;
                }
            }

            return bombCount;
        }
    }
}