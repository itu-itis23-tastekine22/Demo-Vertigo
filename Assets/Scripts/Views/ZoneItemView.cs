using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VertigoWheel.Views
{
    public class ZoneItemView : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("backgroundImage")] private Image _backgroundImage;
        [SerializeField, FormerlySerializedAs("numberText")] private TMP_Text _numberText;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_backgroundImage)
            {
                _backgroundImage = GetComponentInChildren<Image>(true);
            }

            if (!_numberText)
            {
                _numberText = GetComponentInChildren<TMP_Text>(true);
            }
        }
#endif

        public void Setup(int zoneNumber, Sprite background)
        {
            _numberText.text = zoneNumber.ToString(CultureInfo.InvariantCulture);
            _backgroundImage.sprite = background;
        }
    }
}