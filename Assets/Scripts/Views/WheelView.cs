using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WheelView : MonoBehaviour
{
    [Header("Visual Elements")]
    [SerializeField] private Image wheelBaseImage;
    [SerializeField] private Image wheelIndicatorImage;
    [SerializeField] private RectTransform wheelRotator;
    [SerializeField] private Button spinButton;

    [Header("Slices")]
    [SerializeField] private WheelSliceView[] slices;

    [Header("Spin Settings")]
    [SerializeField] private float spinDuration = 4f;
    [SerializeField] private int fullRotations = 5;
    [SerializeField] private Ease spinEase = Ease.OutQuart;

    [Header("Debug / Testing")]
    [SerializeField] private WheelConfig testConfig;

    private bool isSpinning;

    public bool IsSpinning => isSpinning;
    public event Action SpinClicked;

#if UNITY_EDITOR
    private void OnValidate()
    {
        slices = GetComponentsInChildren<WheelSliceView>(true);

        if (spinButton == null)
        {
            spinButton = GetComponentInChildren<Button>(true);
        }
    }
#endif

    private void OnEnable()
    {
        if (spinButton != null)
        {
            spinButton.onClick.AddListener(OnSpinButtonClicked);
        }
    }

    private void OnDisable()
    {
        if (spinButton != null)
        {
            spinButton.onClick.RemoveListener(OnSpinButtonClicked);
        }
    }

    private void OnSpinButtonClicked()
    {
        SpinClicked?.Invoke();
    }

    public void SetSpinInteractable(bool value)
    {
        if (spinButton != null)
        {
            spinButton.interactable = value;
        }
    }

    /// <summary>
    /// Çarkı ve dilimleri gelen konfigürasyona göre günceller.
    /// amountCalculator: Her dilim için ekrana yazılacak miktarı dönen delege.
    /// </summary>
    public void ApplyConfig(WheelConfig config, Func<WheelSliceData, int> amountCalculator = null)
    {
        if (config == null)
        {
            Debug.LogWarning($"[{nameof(WheelView)}] ApplyConfig çağrıldı ancak config null!");
            return;
        }

        if (wheelBaseImage != null && config.WheelBaseSprite != null)
        {
            wheelBaseImage.sprite = config.WheelBaseSprite;
        }

        if (wheelIndicatorImage != null && config.WheelIndicatorSprite != null)
        {
            wheelIndicatorImage.sprite = config.WheelIndicatorSprite;
        }

        int count = Mathf.Min(slices.Length, config.Slices.Count);
        for (int i = 0; i < count; i++)
        {
            var sliceData = config.Slices[i];
            
            // Fonksiyon verilmişse hesaplanmış miktarı al, verilmemişse config'teki temel sayıyı kullan
            int displayAmount = amountCalculator != null ? amountCalculator(sliceData) : sliceData.Amount;
            
            slices[i].Setup(sliceData, displayAmount);
        }
    }

    public void Spin(int targetSliceIndex, Action onComplete = null)
    {
        if (isSpinning) return;

        if (slices == null || slices.Length == 0)
        {
            Debug.LogError($"[{nameof(WheelView)}] Dilimler bulunamadı!");
            return;
        }

        isSpinning = true;

        float sliceAngle = 360f / slices.Length;
        float targetAngle = targetSliceIndex * sliceAngle;
        float totalRotation = (fullRotations * 360f) - targetAngle;

        wheelRotator
            .DOLocalRotate(new Vector3(0f, 0f, -totalRotation), spinDuration, RotateMode.FastBeyond360)
            .SetEase(spinEase)
            .OnComplete(() =>
            {
                isSpinning = false;
                onComplete?.Invoke();
            });
    }

    [ContextMenu("Apply Test Config")]
    private void ApplyTestConfig()
    {
        ApplyConfig(testConfig);
    }
}