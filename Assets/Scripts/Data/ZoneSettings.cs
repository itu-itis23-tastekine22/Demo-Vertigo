using UnityEngine;

[CreateAssetMenu(fileName = "ZoneSettings", menuName = "Wheel Game/Zone Settings")]
public class ZoneSettings : ScriptableObject
{
    [Header("Interval Settings")]
    [SerializeField, Min(1)] private int safeZoneInterval = 5;
    [SerializeField, Min(1)] private int superZoneInterval = 30;

    [Header("Wheel Configurations")]
    [SerializeField] private WheelConfig normalWheel;
    [SerializeField] private WheelConfig safeWheel;
    [SerializeField] private WheelConfig superWheel;

    [Header("Reward Scaling")]
    [SerializeField, Min(0f)] private float rewardIncreaseRate = 0.1f;

    /// <summary>
    /// Verilen zone indeksine göre bölge türünü belirler.
    /// Süper aralık önceliklidir; ardından güvenli aralık kontrol edilir.
    /// </summary>
    public WheelZoneType GetZoneType(int zone)
    {
        if (zone > 0 && zone % superZoneInterval == 0)
        {
            return WheelZoneType.Super;
        }

        if (zone > 0 && zone % safeZoneInterval == 0)
        {
            return WheelZoneType.Safe;
        }

        return WheelZoneType.Normal;
    }

    /// <summary>
    /// Zone türüne karşılık gelen WheelConfig referansını döner.
    /// </summary>
    public WheelConfig GetWheelConfig(int zone)
    {
        WheelZoneType zoneType = GetZoneType(zone);

        switch (zoneType)
        {
            case WheelZoneType.Super:
                return superWheel;

            case WheelZoneType.Safe:
                return safeWheel;

            case WheelZoneType.Normal:
            default:
                return normalWheel;
        }
    }

    /// <summary>
    /// Zone seviyesine göre temel ödül miktarını ölçeklendirir.
    /// Formül: baseAmount * (1 + (zone - 1) * rewardIncreaseRate)
    /// </summary>
    public int GetScaledAmount(int baseAmount, int zone)
    {
        float multiplier = 1f + (zone - 1) * rewardIncreaseRate;
        return Mathf.RoundToInt(baseAmount * multiplier);
    }
}