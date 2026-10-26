using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace VertigoWheel.Data
{
    [CreateAssetMenu(fileName = "ZoneSettings", menuName = "Wheel Game/Zone Settings")]
    public class ZoneSettings : ScriptableObject
    {
        [Header("Zone Types")]
        [SerializeField] private List<ZoneTypeData> _zoneTypes = new List<ZoneTypeData>();

        [Header("Reward Scaling")]
        [Tooltip("Multiplier added per zone for rewards that scale with the zone.")]
        [SerializeField, FormerlySerializedAs("rewardIncreaseRate"), Min(0f)] private float _rewardIncreaseRate = 0.1f;

        public ZoneTypeData GetZoneType(int zone)
        {
            ZoneTypeData selectedType = null;

            for (int i = 0; i < _zoneTypes.Count; i++)
            {
                ZoneTypeData zoneType = _zoneTypes[i];

                if (!zoneType || !zoneType.Matches(zone))
                {
                    continue;
                }

                if (!selectedType || zoneType.Priority > selectedType.Priority)
                {
                    selectedType = zoneType;
                }
            }

            if (!selectedType)
            {
                throw new InvalidOperationException($"No zone type matches zone {zone}. Add a zone type with interval 1.");
            }

            return selectedType;
        }

        public int GetScaledAmount(int baseAmount, int zone)
        {
            float multiplier = 1f + (zone - 1) * _rewardIncreaseRate;
            return Mathf.RoundToInt(baseAmount * multiplier);
        }
    }
}