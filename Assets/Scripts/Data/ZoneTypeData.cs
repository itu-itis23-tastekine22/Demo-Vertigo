using System.Collections.Generic;
using UnityEngine;

namespace VertigoWheel.Data
{
    [CreateAssetMenu(fileName = "Zone_New", menuName = "Wheel Game/Zone Type")]
    public class ZoneTypeData : ScriptableObject
    {
        [Header("Occurrence")]
        [Tooltip("Applies to every zone divisible by this value. Use 1 for the default zone type.")]
        [SerializeField, Min(1)] private int _interval = 1;

        [Tooltip("When several zone types match the same zone, the highest priority wins.")]
        [SerializeField, Min(0)] private int _priority;

        [Header("Rules")]
        [SerializeField] private bool _hasBomb;
        [SerializeField] private bool _isLeaveAllowed;

        [Header("Visual")]
        [SerializeField] private Sprite _zoneBarSprite;

        [Header("Wheels")]
        [Tooltip("The tier with the highest start zone that is not above the current zone is used.")]
        [SerializeField] private List<WheelTier> _wheelTiers = new List<WheelTier>();

        public int Interval
        {
            get { return _interval; }
        }

        public int Priority
        {
            get { return _priority; }
        }

        public bool HasBomb
        {
            get { return _hasBomb; }
        }

        public bool IsLeaveAllowed
        {
            get { return _isLeaveAllowed; }
        }

        public Sprite ZoneBarSprite
        {
            get { return _zoneBarSprite; }
        }

        public bool Matches(int zone)
        {
            return zone > 0 && zone % _interval == 0;
        }

        public WheelConfig GetWheel(int zone)
        {
            WheelTier selectedTier = null;

            for (int i = 0; i < _wheelTiers.Count; i++)
            {
                WheelTier tier = _wheelTiers[i];

                if (tier == null || tier.StartZone > zone)
                {
                    continue;
                }

                if (selectedTier == null || tier.StartZone > selectedTier.StartZone)
                {
                    selectedTier = tier;
                }
            }

            if (selectedTier == null)
            {
                return null;
            }

            return selectedTier.Wheel;
        }
    }
}