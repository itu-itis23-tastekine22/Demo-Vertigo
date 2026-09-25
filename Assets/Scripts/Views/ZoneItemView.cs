using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZoneItemView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text numberText;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (backgroundImage == null)
        {
            backgroundImage = GetComponentInChildren<Image>(true);
        }

        if (numberText == null)
        {
            numberText = GetComponentInChildren<TMP_Text>(true);
        }
    }
#endif

    public void Setup(int zoneNumber, Sprite background)
    {
        if (numberText != null)
        {
            numberText.text = zoneNumber.ToString();
        }

        if (backgroundImage != null && background != null)
        {
            backgroundImage.sprite = background;
        }
    }
}