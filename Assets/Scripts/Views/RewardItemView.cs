using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VertigoWheel.Views
{
    public class RewardItemView : MonoBehaviour
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

        public void Show(Sprite icon, int amount)
        {
            _iconImage.sprite = icon;
            _amountText.text = AmountFormatter.Format(amount);
        }
    }
}