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

    // Dışarıdan Controller'ın dinleyeceği buton event'i
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

    /// <summary>
    /// Çark dönerken butonun tekrar tıklanmasını engellemek için kullanılır.
    /// </summary>
    public void SetSpinInteractable(bool value)
    {
        if (spinButton != null)
        {
            spinButton.interactable = value;
        }
    }

    public void ApplyConfig(WheelConfig config)
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
            slices[i].Setup(config.Slices[i]);
        }
    }

    public void Spin(int targetSliceIndex, Action onComplete = null)
    {
        if (isSpinning)
        {
            return;
        }

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

    [ContextMenu("Test Spin")]
    private void TestSpin()
    {
        if (slices == null || slices.Length == 0)
        {
            Debug.LogWarning($"[{nameof(WheelView)}] Çarkta dilim bulunmuyor!");
            return;
        }

        int randomSlice = UnityEngine.Random.Range(0, slices.Length);
        Spin(randomSlice, () =>
        {
            Debug.Log($"[WheelView] Çark durdu! Kazanan dilim indeksi: {randomSlice}");
        });
    }
}