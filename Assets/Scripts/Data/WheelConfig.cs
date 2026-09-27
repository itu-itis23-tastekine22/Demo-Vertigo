using System.Collections.Generic;
using UnityEngine;

public enum WheelZoneType
{
    Normal,     // Standart çark 
    Safe,       // Her 5. bölge 
    Super       // Her 30. bölge 
}

[CreateAssetMenu(fileName = "WheelConfig_New", menuName = "Wheel Game/Wheel Config")]
public class WheelConfig : ScriptableObject
{
    public const int REQUIRED_SLICE_COUNT = 8;

    [Header("Zone & Theme Settings")]
    [SerializeField] private WheelZoneType zoneType = WheelZoneType.Normal;
    [SerializeField] private Sprite wheelBaseSprite;         // ui_image_spin_base_value
    [SerializeField] private Sprite wheelIndicatorSprite;    // ui_image_spin_indicator_value

    [Header("Slice Contents (Tam 8 Dilim)")]
    [Tooltip("Çark üzerindeki 8 dilimin ödül ve miktar verileri.")]
    [SerializeField] private List<WheelSliceData> slices = new List<WheelSliceData>(REQUIRED_SLICE_COUNT);

    public WheelZoneType ZoneType => zoneType;
    public Sprite WheelBaseSprite => wheelBaseSprite;
    public Sprite WheelIndicatorSprite => wheelIndicatorSprite;
    public IReadOnlyList<WheelSliceData> Slices => slices;

    public bool IsSafeZone => zoneType == WheelZoneType.Safe || zoneType == WheelZoneType.Super;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (slices == null)
        {
            slices = new List<WheelSliceData>(REQUIRED_SLICE_COUNT);
        }

        while (slices.Count < REQUIRED_SLICE_COUNT)
        {
            slices.Add(new WheelSliceData());
        }

        while (slices.Count > REQUIRED_SLICE_COUNT)
        {
            slices.RemoveAt(slices.Count - 1);
        }

        int bombCount = 0;
        for (int i = 0; i < slices.Count; i++)
        {
            if (slices[i] != null && slices[i].IsBomb)
            {
                bombCount++;
            }
        }

        if (IsSafeZone && bombCount > 0)
        {
            Debug.LogWarning($"[{name}] Güvenli/Süper çarkta bomba bulunamaz! Bulunan bomba sayısı: {bombCount}");
        }
        else if (zoneType == WheelZoneType.Normal && bombCount != 1)
        {
            Debug.LogWarning($"[{name}] Standart çarkta tam olarak 1 bomba bulunmalıdır. Mevcut: {bombCount}");
        }
    }
#endif
}