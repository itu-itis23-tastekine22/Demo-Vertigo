using System;
using UnityEngine;

namespace VertigoWheel.Data
{
    [Serializable]
    public class WheelTier
    {
        [SerializeField, Min(1)] private int _startZone = 1;
        [SerializeField] private WheelConfig _wheel;

        public int StartZone
        {
            get { return _startZone; }
        }

        public WheelConfig Wheel
        {
            get { return _wheel; }
        }
    }
}