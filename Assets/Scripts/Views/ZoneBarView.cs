using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VertigoWheel.Data;

namespace VertigoWheel.Views
{
    public class ZoneBarView : MonoBehaviour
    {
        private const int NoHighlightedZone = -1;

        [Header("Containers")]
        [SerializeField, FormerlySerializedAs("viewport")] private RectTransform _viewport;
        [SerializeField, FormerlySerializedAs("content")] private RectTransform _content;

        [Header("Prefab & Layout")]
        [SerializeField, FormerlySerializedAs("itemPrefab")] private ZoneItemView _itemPrefab;
        [SerializeField, FormerlySerializedAs("layoutGroup")] private HorizontalLayoutGroup _layoutGroup;

        [Header("Highlight")]
        [SerializeField, FormerlySerializedAs("currentZoneSprite")] private Sprite _currentZoneSprite;

        [Header("Scroll Settings")]
        [SerializeField, FormerlySerializedAs("zonesAhead"), Min(1)] private int _zonesAhead = 15;
        [SerializeField, FormerlySerializedAs("scrollDuration"), Min(0f)] private float _scrollDuration = 0.3f;

        private readonly List<ZoneItemView> _items = new List<ZoneItemView>();
        private ZoneSettings _zoneSettings;
        private int _highlightedZone = NoHighlightedZone;
        private Tween _scrollTween;

        private void OnDestroy()
        {
            KillScrollTween();
        }

        public void Initialize(ZoneSettings zoneSettings)
        {
            _zoneSettings = zoneSettings;
        }

        public void SetCurrentZone(int zone)
        {
            EnsureItemCount(zone + _zonesAhead);
            RestoreHighlightedZone();
            HighlightZone(zone);
            ScrollToZone(zone);
        }

        private void EnsureItemCount(int requiredCount)
        {
            while (_items.Count < requiredCount)
            {
                int zoneNumber = _items.Count + 1;
                ZoneItemView item = Instantiate(_itemPrefab, _content);
                item.Setup(zoneNumber, GetZoneSprite(zoneNumber));
                _items.Add(item);
            }
        }

        private void RestoreHighlightedZone()
        {
            if (_highlightedZone == NoHighlightedZone)
            {
                return;
            }

            _items[_highlightedZone - 1].Setup(_highlightedZone, GetZoneSprite(_highlightedZone));
            _highlightedZone = NoHighlightedZone;
        }

        private void HighlightZone(int zone)
        {
            _items[zone - 1].Setup(zone, _currentZoneSprite);
            _highlightedZone = zone;
        }

        private void ScrollToZone(int zone)
        {
            RectTransform itemRect = (RectTransform)_itemPrefab.transform;
            float itemWidth = itemRect.rect.width;
            float step = itemWidth + _layoutGroup.spacing;
            float itemCenter = _layoutGroup.padding.left + (zone - 1) * step + itemWidth * 0.5f;
            float targetX = _viewport.rect.width * 0.5f - itemCenter;

            KillScrollTween();

            _scrollTween = _content
                .DOAnchorPosX(targetX, _scrollDuration)
                .SetEase(Ease.OutCubic)
                .SetLink(gameObject);
        }

        private Sprite GetZoneSprite(int zone)
        {
            return _zoneSettings.GetZoneType(zone).ZoneBarSprite;
        }

        private void KillScrollTween()
        {
            if (_scrollTween != null)
            {
                _scrollTween.Kill();
                _scrollTween = null;
            }
        }
    }
}