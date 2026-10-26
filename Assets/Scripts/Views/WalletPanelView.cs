using System.Collections.Generic;
using UnityEngine;

namespace VertigoWheel.Views
{
    public class WalletPanelView : MonoBehaviour
    {
        [SerializeField] private RewardListView _rewardList;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_rewardList)
            {
                _rewardList = GetComponentInChildren<RewardListView>(true);
            }
        }
#endif

        public void ShowBalances(IReadOnlyList<RewardListEntry> entries)
        {
            _rewardList.Show(entries);
        }
    }
}