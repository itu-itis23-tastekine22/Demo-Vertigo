using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BombPopupView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button giveUpButton;
    [SerializeField] private RectTransform popupRoot;

    [Header("Animation Settings")]
    [SerializeField] private float showDuration = 0.3f;

    public event Action GiveUpClicked;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // true parametresi panel inaktif olsa dahi child objelerdeki butonu bulmasını sağlar
        if (giveUpButton == null)
        {
            giveUpButton = GetComponentInChildren<Button>(true);
        }
    }
#endif

    private void OnEnable()
    {
        if (giveUpButton != null)
        {
            giveUpButton.onClick.AddListener(OnGiveUpButtonClicked);
        }
    }

    private void OnDisable()
    {
        if (giveUpButton != null)
        {
            giveUpButton.onClick.RemoveListener(OnGiveUpButtonClicked);
        }
    }

    private void OnGiveUpButtonClicked()
    {
        GiveUpClicked?.Invoke();
    }

    /// <summary>
    /// Paneli aktif eder ve pencereyi OutBack eğrisiyle büyüterek açar.
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);

        if (popupRoot != null)
        {
            popupRoot.DOKill();
            popupRoot.localScale = Vector3.zero;
            popupRoot.DOScale(1f, showDuration).SetEase(Ease.OutBack);
        }
    }

    /// <summary>
    /// Paneli doğrudan kapatır.
    /// </summary>
    public void Hide()
    {
        if (popupRoot != null)
        {
            popupRoot.DOKill();
        }

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (popupRoot != null)
        {
            popupRoot.DOKill();
        }
    }
}