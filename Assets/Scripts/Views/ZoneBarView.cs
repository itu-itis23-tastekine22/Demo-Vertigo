using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ZoneBarView : MonoBehaviour
{
    [Header("Containers")]
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;

    [Header("Prefab & Layout")]
    [SerializeField] private ZoneItemView itemPrefab;
    [SerializeField] private HorizontalLayoutGroup layoutGroup;

    [Header("Zone Background Sprites")]
    [SerializeField] private Sprite normalZoneSprite;
    [SerializeField] private Sprite safeZoneSprite;
    [SerializeField] private Sprite superZoneSprite;
    [SerializeField] private Sprite currentZoneSprite;

    [Header("Scroll & Buffer Settings")]
    [SerializeField] private int zonesAhead = 15;
    [SerializeField] private float scrollDuration = 0.3f;

    private readonly List<ZoneItemView> _items = new List<ZoneItemView>();
    private Func<int, WheelZoneType> _getZoneType;
    private int _lastHighlightedZone = -1;

    /// <summary>
    /// Zone türünü sorar.
    /// </summary>
    public void Initialize(Func<int, WheelZoneType> zoneTypeProvider)
    {
        _getZoneType = zoneTypeProvider ?? throw new ArgumentNullException(nameof(zoneTypeProvider));
    }

    /// <summary>
    /// Şeritteki aktif aktif olan zone'u günceller ve şeridi ortaya hizalar.
    /// </summary>
    public void SetCurrentZone(int zone)
    {
        if (_getZoneType == null) return;

        // 1. İhtiyaç oldukça kutu üret 
        while (_items.Count < zone + zonesAhead)
        {
            int nextZoneNumber = _items.Count + 1;
            ZoneItemView newItem = Instantiate(itemPrefab, content);

            WheelZoneType type = _getZoneType(nextZoneNumber);
            newItem.Setup(nextZoneNumber, GetSpriteForZoneType(type));

            _items.Add(newItem);
        }

        // 2. Bir önceki aktif kutuyu kendi doğal zone rengine geri çevir
        if (_lastHighlightedZone > 0 && _lastHighlightedZone <= _items.Count)
        {
            WheelZoneType prevType = _getZoneType(_lastHighlightedZone);
            _items[_lastHighlightedZone - 1].Setup(_lastHighlightedZone, GetSpriteForZoneType(prevType));
        }

        // 3. Yeni aktif zone kutusunu "current" (mavi) sprite ile işaretle
        if (zone > 0 && zone <= _items.Count)
        {
            _items[zone - 1].Setup(zone, currentZoneSprite);
            _lastHighlightedZone = zone;
        }

        // Hedef X pozisyonunu hesaplama işlemi
        float itemWidth = ((RectTransform)itemPrefab.transform).rect.width;
        if (itemWidth <= 0f)
        {
            itemWidth = ((RectTransform)itemPrefab.transform).sizeDelta.x;
        }

        float spacing = layoutGroup != null ? layoutGroup.spacing : 10f;
        float paddingLeft = layoutGroup != null ? layoutGroup.padding.left : 0f;

        float boxCenterFromLeft = paddingLeft + ((zone - 1) * (itemWidth + spacing)) + (itemWidth * 0.5f);
        float targetX = (viewport.rect.width * 0.5f) - boxCenterFromLeft;

        // DOTween ile kaydır
        content.DOKill();
        content.DOAnchorPosX(targetX, scrollDuration).SetEase(Ease.OutCubic);
    }

    private Sprite GetSpriteForZoneType(WheelZoneType type)
    {
        switch (type)
        {
            case WheelZoneType.Super:
                return superZoneSprite;
            case WheelZoneType.Safe:
                return safeZoneSprite;
            case WheelZoneType.Normal:
            default:
                return normalZoneSprite;
        }
    }

    private void OnDestroy()
    {
        if (content != null)
        {
            content.DOKill();
        }
    }
}