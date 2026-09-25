using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private ZoneSettings zoneSettings;

    [Header("Views")]
    [SerializeField] private WheelView wheelView;
    [SerializeField] private RewardsPanelView rewardsPanelView;

    private GameSession _session;
    private SpinResolver _resolver;

    private void Awake()
    {
        var inventory = new RewardInventory();
        _session = new GameSession(zoneSettings, inventory);
        _resolver = new SpinResolver(new UnityRandomProvider());
    }

    private void Start()
    {
        // Event abonelikleri
        wheelView.SpinClicked += HandleSpinClicked;
        rewardsPanelView.LeaveClicked += HandleLeaveClicked;
        _session.ZoneChanged += RefreshView;

        // İlk zone görselini ekrana getir
        RefreshView();
    }

    private void OnDestroy()
    {
        // Bellek sızıntılarını önlemek için abonelikleri çözüyoruz
        if (wheelView != null)
        {
            wheelView.SpinClicked -= HandleSpinClicked;
        }

        if (rewardsPanelView != null)
        {
            rewardsPanelView.LeaveClicked -= HandleLeaveClicked;
        }

        if (_session != null)
        {
            _session.ZoneChanged -= RefreshView;
        }
    }

    private void RefreshView()
    {
        wheelView.ApplyConfig(_session.CurrentWheel);

        // Spin butonu her zaman açık, Leave butonu yalnızca güvenli/süper zone'da aktif
        wheelView.SetSpinInteractable(true);
        rewardsPanelView.SetLeaveInteractable(_session.CanLeave);
    }

    private void HandleSpinClicked()
    {
        if (wheelView.IsSpinning) return;

        int targetIndex = _resolver.ResolveSliceIndex(_session.CurrentWheel);

        // Çark dönerken oyuncunun tekrar basması veya güvenli zone'da kaçması engellenir
        wheelView.SetSpinInteractable(false);
        rewardsPanelView.SetLeaveInteractable(false);

        wheelView.Spin(targetIndex, () => HandleSpinCompleted(targetIndex));
    }

    private void HandleSpinCompleted(int targetIndex)
    {
        bool isBomb = _session.ResolveSpin(targetIndex);

        if (isBomb)
        {
            Debug.LogWarning("<color=red>[SONUÇ]</color> BOMBA VURDU! Tüm ödüller kaybedildi.");
            _session.Reset();
        }
        else
        {
            Debug.Log($"<color=green>[SONUÇ]</color> Ödül kazanıldı! Yeni Zone: {_session.CurrentZone} | Farklı Ödül Sayısı: {_session.Inventory.Items.Count}");
            // Session içindeki ZoneChanged tetiklendiği için RefreshView otomatik çağrılır
        }
    }

    private void HandleLeaveClicked()
    {
        if (!_session.CanLeave || wheelView.IsSpinning) return;

        Debug.Log("<color=yellow>[ÇIKIŞ YAPILDI]</color> Kasaya aktarılan ödüller:");
        foreach (var item in _session.Inventory.Items)
        {
            Debug.Log($"- {item.Key.DisplayName}: {item.Value} adet");
        }

        _session.Reset();
    }
}