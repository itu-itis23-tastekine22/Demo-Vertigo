using System.Collections.Generic;
using VertigoWheel.Data;

namespace VertigoWheel.Core
{
    public sealed class RuntimeWheel
    {
        public const int NoBombIndex = -1;

        private readonly List<WheelSlice> _slices;

        public RuntimeWheel(ZoneTypeData zoneType, WheelConfig config, List<WheelSlice> slices, int bombIndex)
        {
            ZoneType = zoneType;
            Config = config;
            _slices = slices;
            BombIndex = bombIndex;
        }

        public ZoneTypeData ZoneType { get; }

        public WheelConfig Config { get; }

        public int BombIndex { get; }

        public IReadOnlyList<WheelSlice> Slices
        {
            get { return _slices; }
        }
    }
}