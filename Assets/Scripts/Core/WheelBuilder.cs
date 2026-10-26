using System;
using System.Collections.Generic;
using VertigoWheel.Data;

namespace VertigoWheel.Core
{
    public sealed class WheelBuilder
    {
        private readonly ZoneSettings _zoneSettings;
        private readonly IRandomProvider _randomProvider;

        public WheelBuilder(ZoneSettings zoneSettings, IRandomProvider randomProvider)
        {
            if (!zoneSettings)
            {
                throw new ArgumentNullException(nameof(zoneSettings));
            }

            if (randomProvider == null)
            {
                throw new ArgumentNullException(nameof(randomProvider));
            }

            _zoneSettings = zoneSettings;
            _randomProvider = randomProvider;
        }

        public RuntimeWheel Build(int zone)
        {
            ZoneTypeData zoneType = _zoneSettings.GetZoneType(zone);
            WheelConfig config = zoneType.GetWheel(zone);

            if (!config)
            {
                throw new InvalidOperationException($"{zoneType.name} has no wheel tier for zone {zone}.");
            }

            if (!config.TryValidate(out string error))
            {
                throw new InvalidOperationException(error);
            }

            List<WheelSlice> slices = CreateRewardSlices(config, zone);
            int bombIndex = RuntimeWheel.NoBombIndex;

            if (zoneType.HasBomb)
            {
                bombIndex = _randomProvider.Range(0, slices.Count);
                slices[bombIndex] = WheelSlice.CreateBomb();
            }

            return new RuntimeWheel(zoneType, config, slices, bombIndex);
        }

        private List<WheelSlice> CreateRewardSlices(WheelConfig config, int zone)
        {
            List<WheelSlice> slices = new List<WheelSlice>(config.Slices.Count);

            for (int i = 0; i < config.Slices.Count; i++)
            {
                WheelSliceData sliceData = config.Slices[i];
                int amount = GetAmount(sliceData, zone);
                slices.Add(WheelSlice.CreateReward(sliceData.Data, amount));
            }

            return slices;
        }

        private int GetAmount(WheelSliceData sliceData, int zone)
        {
            if (!sliceData.Data.IsScaledByZone)
            {
                return sliceData.Amount;
            }

            return _zoneSettings.GetScaledAmount(sliceData.Amount, zone);
        }
    }
}