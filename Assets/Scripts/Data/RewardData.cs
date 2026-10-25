using UnityEngine;
using UnityEngine.Serialization;

namespace VertigoWheel.Data
{
    [CreateAssetMenu(fileName = "Reward_New", menuName = "Wheel Game/Reward Data")]
    public class RewardData : ScriptableObject
    {
        [Header("Definition")]
        [SerializeField, FormerlySerializedAs("displayName")] private string _displayName;
        [SerializeField, FormerlySerializedAs("rewardType")] private RewardType _rewardType;

        [Header("Visual")]
        [SerializeField, FormerlySerializedAs("icon")] private Sprite _icon;

        [Header("Progression")]
        [Tooltip("Stackable rewards grow with the zone multiplier. Single items such as weapons, cosmetics and chests must stay off.")]
        [SerializeField, FormerlySerializedAs("scalesWithZone")] private bool _isScaledByZone;

        public string DisplayName
        {
            get { return _displayName; }
        }

        public RewardType Type
        {
            get { return _rewardType; }
        }

        public Sprite Icon
        {
            get { return _icon; }
        }

        public bool IsScaledByZone
        {
            get { return _isScaledByZone; }
        }

        public bool IsBomb
        {
            get { return _rewardType == RewardType.Bomb; }
        }
    }
}