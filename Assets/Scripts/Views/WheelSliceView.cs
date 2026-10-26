using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VertigoWheel.Views
{
    public class WheelSliceView : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("iconImage")] private Image _iconImage;
        [SerializeField, FormerlySerializedAs("amountText")] private TMP_Text _amountText;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_iconImage)
            {
                _iconImage = GetComponentInChildren<Image>(true);
            }

            if (!_amountText)
            {
                _amountText = GetComponentInChildren<TMP_Text>(true);
            }
        }
#endif

        public void ShowReward(Sprite icon, string amountLabel)
        {
            _iconImage.sprite = icon;
            _amountText.text = amountLabel;
            _amountText.gameObject.SetActive(true);
        }

        public void ShowBomb(Sprite bombIcon)
        {
            _iconImage.sprite = bombIcon;
            _amountText.gameObject.SetActive(false);
        }
    }
}