using NUnit.Framework;
using VertigoWheel.Data;

namespace VertigoWheel.Tests
{
    public class ZoneSettingsTests
    {
        private TestAssetFactory _factory;
        private ZoneTypeData _normalZone;
        private ZoneTypeData _safeZone;
        private ZoneTypeData _superZone;
        private ZoneSettings _zoneSettings;

        [SetUp]
        public void SetUp()
        {
            _factory = new TestAssetFactory();

            RewardData gold = _factory.CreateReward("Reward_Gold", true);
            WheelConfig wheel = _factory.CreateWheel("Wheel_Test", gold, 10);

            _normalZone = _factory.CreateZoneType("Zone_Normal", 1, 0, true, false, (1, wheel));
            _safeZone = _factory.CreateZoneType("Zone_Safe", 5, 1, false, true, (5, wheel));
            _superZone = _factory.CreateZoneType("Zone_Super", 30, 2, false, true, (30, wheel));
            _zoneSettings = _factory.CreateZoneSettings(0.1f, _normalZone, _safeZone, _superZone);
        }

        [TearDown]
        public void TearDown()
        {
            _factory.DestroyAll();
        }

        [TestCase(1)]
        [TestCase(4)]
        [TestCase(29)]
        public void GetZoneType_RegularZone_ReturnsNormalZone(int zone)
        {
            Assert.AreSame(_normalZone, _zoneSettings.GetZoneType(zone));
        }

        [TestCase(5)]
        [TestCase(10)]
        [TestCase(25)]
        public void GetZoneType_EveryFifthZone_ReturnsSafeZone(int zone)
        {
            Assert.AreSame(_safeZone, _zoneSettings.GetZoneType(zone));
        }

        [TestCase(30)]
        [TestCase(60)]
        public void GetZoneType_EveryThirtiethZone_SuperZoneWinsOverSafeZone(int zone)
        {
            Assert.AreSame(_superZone, _zoneSettings.GetZoneType(zone));
        }

        [Test]
        public void GetScaledAmount_FirstZone_ReturnsBaseAmount()
        {
            Assert.AreEqual(10, _zoneSettings.GetScaledAmount(10, 1));
        }

        [Test]
        public void GetScaledAmount_EleventhZone_DoublesBaseAmount()
        {
            Assert.AreEqual(20, _zoneSettings.GetScaledAmount(10, 11));
        }
    }
}