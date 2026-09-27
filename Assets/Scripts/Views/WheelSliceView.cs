using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelSliceView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (iconImage == null)
        {
            iconImage = GetComponentInChildren<Image>();
        }

        if (amountText == null)
        {
            amountText = GetComponentInChildren<TMP_Text>();
        }
    }
#endif

    public void Setup(WheelSliceData sliceData, int displayAmount)
    {
        
        if (sliceData == null || sliceData.Data == null)
        {
            return;
        }

        iconImage.sprite = sliceData.Data.Icon;

        if (sliceData.IsBomb)
        {
            amountText.gameObject.SetActive(false);
        }
        else
        {
            amountText.gameObject.SetActive(true);
            amountText.text = "x" + displayAmount;
        }
    }
}