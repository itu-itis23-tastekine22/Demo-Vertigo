using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace VertigoWheel.Data
{
    [CreateAssetMenu(fileName = "Wheel_New", menuName = "Wheel Game/Wheel Config")]
    public class WheelConfig : ScriptableObject
    {
        public const int RequiredSliceCount = 8;

        [Header("Visual")]
        [SerializeField, FormerlySerializedAs("wheelBaseSprite")] private Sprite _baseSprite;
        [SerializeField, FormerlySerializedAs("wheelIndicatorSprite")] private Sprite _indicatorSprite;

        [Header("Slices")]
        [Tooltip("Reward slices only. The bomb is placed at runtime by the game session.")]
        [SerializeField, FormerlySerializedAs("slices")] private List<WheelSliceData> _slices = new List<WheelSliceData>();

        public Sprite BaseSprite
        {
            get { return _baseSprite; }
        }

        public Sprite IndicatorSprite
        {
            get { return _indicatorSprite; }
        }

        public IReadOnlyList<WheelSliceData> Slices
        {
            get { return _slices; }
        }

        public bool TryValidate(out string error)
        {
            if (_slices.Count != RequiredSliceCount)
            {
                error = $"{name} must have exactly {RequiredSliceCount} slices but has {_slices.Count}.";
                return false;
            }

            for (int i = 0; i < _slices.Count; i++)
            {
                if (_slices[i] == null || _slices[i].IsEmpty)
                {
                    error = $"{name} slice {i} has no reward.";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            while (_slices.Count < RequiredSliceCount)
            {
                _slices.Add(new WheelSliceData());
            }

            while (_slices.Count > RequiredSliceCount)
            {
                _slices.RemoveAt(_slices.Count - 1);
            }
        }
#endif
    }
}