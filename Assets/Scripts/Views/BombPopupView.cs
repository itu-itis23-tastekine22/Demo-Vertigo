using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VertigoWheel.Events;

namespace VertigoWheel.Views
{
    public class BombPopupView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField, FormerlySerializedAs("giveUpButton")] private Button _giveUpButton;
        [SerializeField, FormerlySerializedAs("popupRoot")] private RectTransform _popupRoot;

        [Header("Animation Settings")]
        [SerializeField, FormerlySerializedAs("showDuration"), Min(0f)] private float _showDuration = 0.3f;

        private IEventBus _eventBus;
        private Tween _showTween;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_giveUpButton)
            {
                _giveUpButton = GetComponentInChildren<Button>(true);
            }
        }
#endif

        private void OnEnable()
        {
            _giveUpButton.onClick.AddListener(HandleGiveUpButtonClicked);
        }

        private void OnDisable()
        {
            _giveUpButton.onClick.RemoveListener(HandleGiveUpButtonClicked);
        }

        private void OnDestroy()
        {
            KillShowTween();
        }

        public void Initialize(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            KillShowTween();

            _popupRoot.localScale = Vector3.zero;
            _showTween = _popupRoot
                .DOScale(1f, _showDuration)
                .SetEase(Ease.OutBack)
                .SetLink(gameObject);
        }

        public void Hide()
        {
            KillShowTween();
            gameObject.SetActive(false);
        }

        private void HandleGiveUpButtonClicked()
        {
            _eventBus.Publish(new GiveUpRequestedEvent());
        }

        private void KillShowTween()
        {
            if (_showTween != null)
            {
                _showTween.Kill();
                _showTween = null;
            }
        }
    }
}