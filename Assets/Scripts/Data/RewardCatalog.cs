using System.Collections.Generic;
using UnityEngine;

namespace VertigoWheel.Data
{
    [CreateAssetMenu(fileName = "RewardCatalog", menuName = "Wheel Game/Reward Catalog")]
    public class RewardCatalog : ScriptableObject
    {
        [SerializeField] private List<RewardData> _rewards = new List<RewardData>();

        public bool TryGetReward(string rewardId, out RewardData reward)
        {
            for (int i = 0; i < _rewards.Count; i++)
            {
                RewardData candidate = _rewards[i];

                if (candidate && candidate.Id == rewardId)
                {
                    reward = candidate;
                    return true;
                }
            }

            reward = null;
            return false;
        }

#if UNITY_EDITOR
        [ContextMenu("Collect All Rewards In Project")]
        private void CollectAllRewards()
        {
            _rewards.Clear();

            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:RewardData");

            for (int i = 0; i < guids.Length; i++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                RewardData reward = UnityEditor.AssetDatabase.LoadAssetAtPath<RewardData>(path);

                if (reward)
                {
                    _rewards.Add(reward);
                }
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}