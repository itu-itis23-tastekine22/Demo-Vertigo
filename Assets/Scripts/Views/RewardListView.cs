using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace VertigoWheel.Views
{
    public class RewardListView : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private RewardItemView _itemPrefab;

        private readonly List<RewardItemView> _activeItems = new List<RewardItemView>();
        private ObjectPool<RewardItemView> _itemPool;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_content)
            {
                _content = (RectTransform)transform;
            }
        }
#endif

        private void Awake()
        {
            _itemPool = new ObjectPool<RewardItemView>(CreateItem, HandleItemTaken, HandleItemReleased, HandleItemDestroyed);
        }

        public void Show(IReadOnlyList<RewardListEntry> entries)
        {
            ReleaseActiveItems();

            for (int i = 0; i < entries.Count; i++)
            {
                RewardItemView item = _itemPool.Get();
                item.Show(entries[i].Icon, entries[i].Amount);
                _activeItems.Add(item);
            }
        }

        private void ReleaseActiveItems()
        {
            for (int i = 0; i < _activeItems.Count; i++)
            {
                _itemPool.Release(_activeItems[i]);
            }

            _activeItems.Clear();
        }

        private RewardItemView CreateItem()
        {
            return Instantiate(_itemPrefab, _content);
        }

        private void HandleItemTaken(RewardItemView item)
        {
            item.gameObject.SetActive(true);
            item.transform.SetAsLastSibling();
        }

        private void HandleItemReleased(RewardItemView item)
        {
            item.gameObject.SetActive(false);
        }

        private void HandleItemDestroyed(RewardItemView item)
        {
            Destroy(item.gameObject);
        }
    }
}