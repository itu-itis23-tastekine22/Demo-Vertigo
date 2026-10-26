using VertigoWheel.Events;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VertigoWheel.Core;

namespace VertigoWheel.Views
{
    public class WheelView : MonoBehaviour
    {
        private const float FullCircleDegrees = 360f;

        [Header("Visual Elements")]
        [SerializeField, FormerlySerializedAs("wheelBaseImage")] private Image _baseImage;
        [SerializeField, FormerlySerializedAs("wheelIndicatorImage")] private Image _indicatorImage;
        [SerializeField, FormerlySerializedAs("wheelRotator")] private RectTransform _rotator;
        [SerializeField, FormerlySerializedAs("spinButton")] private Button _spinButton;
        [SerializeField] private Sprite _bombSprite;

        [Header("Slices")]
        [SerializeField, FormerlySerializedAs("slices")] private WheelSliceView[] _sliceViews;

        [Header("Spin Settings")]
        [SerializeField, FormerlySerializedAs("spinDuration"), Min(0.1f)] private float _spinDuration = 4f;
        [SerializeField, FormerlySerializedAs("fullRotations"), Range(1, 15)] private int _fullRotations = 5;
        [SerializeField, FormerlySerializedAs("spinEase")] private Ease _spinEase = Ease.OutQuart;

        private Tween _spinTween;

        private IEventBus _eventBus;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _sliceViews = GetComponentsInChildren<WheelSliceView>(true);

            if (!_spinButton)
            {
                _spinButton = GetComponentInChildren<Button>(true);
            }
        }
#endif

        private void OnEnable()
        {
            _spinButton.onClick.AddListener(HandleSpinButtonClicked);
        }

        private void OnDisable()
        {
            _spinButton.onClick.RemoveListener(HandleSpinButtonClicked);
        }

        private void OnDestroy()
        {
            KillSpinTween();
        }

        public void Initialize(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void ShowWheel(RuntimeWheel wheel)
        {
            _baseImage.sprite = wheel.Config.BaseSprite;
            _indicatorImage.sprite = wheel.Config.IndicatorSprite;

            int count = Mathf.Min(_sliceViews.Length, wheel.Slices.Count);

            for (int i = 0; i < count; i++)
            {
                WheelSlice slice = wheel.Slices[i];

                if (slice.IsBomb)
                {
                    _sliceViews[i].ShowBomb(_bombSprite);
                }
                else
                {
                    _sliceViews[i].ShowReward(slice.Reward.Icon, AmountFormatter.Format(slice.Amount));
                }
            }
        }

        public void PlaySpin(int targetSliceIndex)
        {
            float sliceAngle = FullCircleDegrees / _sliceViews.Length;
            float targetAngle = targetSliceIndex * sliceAngle;
            float totalRotation = _fullRotations * FullCircleDegrees - targetAngle;

            KillSpinTween();

            _spinTween = _rotator
                .DOLocalRotate(new Vector3(0f, 0f, -totalRotation), _spinDuration, RotateMode.FastBeyond360)
                .SetEase(_spinEase)
                .SetLink(gameObject)
                .OnComplete(HandleSpinTweenCompleted);
        }

        public void SetSpinInteractable(bool isInteractable)
        {
            _spinButton.interactable = isInteractable;
        }

        private void HandleSpinButtonClicked()
        {
            _eventBus.Publish(new SpinRequestedEvent());
        }

        private void HandleSpinTweenCompleted()
        {
            _spinTween = null;
            _eventBus.Publish(new SpinAnimationCompletedEvent());
        }

        private void KillSpinTween()
        {
            if (_spinTween != null)
            {
                _spinTween.Kill();
                _spinTween = null;
            }
        }
    }
}