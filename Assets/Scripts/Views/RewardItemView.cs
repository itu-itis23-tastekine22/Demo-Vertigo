using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (iconImage == null)
        {
            iconImage = GetComponentInChildren<Image>(true);
        }

        if (amountText == null)
        {
            amountText = GetComponentInChildren<TMP_Text>(true);
        }
    }
#endif

    public void Setup(RewardData reward, int amount)
    {
        if (reward == null)
        {
            Debug.LogWarning($"[{nameof(RewardItemView)}] Gelen RewardData null!");
            return;
        }

        if (iconImage != null && reward.Icon != null)
        {
            iconImage.sprite = reward.Icon;
        }

        if (amountText != null)
        {
            amountText.text = $"x{amount}";
        }
    }
}