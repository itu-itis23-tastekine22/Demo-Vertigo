using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private ZoneSettings zoneSettings;

    [Header("Views")]
    [SerializeField] private WheelView wheelView;
    [SerializeField] private RewardsPanelView rewardsPanelView;
    [SerializeField] private ZoneBarView zoneBarView;
    [SerializeField] private BombPopupView bombPopupView;

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
        wheelView.SpinClicked += HandleSpinClicked;
        rewardsPanelView.LeaveClicked += HandleLeaveClicked;
        _session.ZoneChanged += RefreshView;
        _session.Inventory.Changed += HandleInventoryChanged;

        if (bombPopupView != null)
        {
            bombPopupView.GiveUpClicked += HandleGiveUpClicked;
            bombPopupView.Hide();
        }

        zoneBarView.Initialize(_session.GetZoneType);

        RefreshView();
    }

    private void OnDestroy()
    {
        if (wheelView != null)
        {
            wheelView.SpinClicked -= HandleSpinClicked;
        }

        if (rewardsPanelView != null)
        {
            rewardsPanelView.LeaveClicked -= HandleLeaveClicked;
        }

        if (bombPopupView != null)
        {
            bombPopupView.GiveUpClicked -= HandleGiveUpClicked;
        }

        if (_session != null)
        {
            _session.ZoneChanged -= RefreshView;
            _session.Inventory.Changed -= HandleInventoryChanged;
        }
    }

    private void HandleInventoryChanged()
    {
        rewardsPanelView.Refresh(_session.Inventory.Items);
    }

    private void RefreshView()
    {
        // Session'ın hesaplama metodunu çarka geçiyoruz
        wheelView.ApplyConfig(_session.CurrentWheel, _session.GetSliceAmount);

        wheelView.SetSpinInteractable(true);
        rewardsPanelView.SetLeaveInteractable(_session.CanLeave);

        zoneBarView.SetCurrentZone(_session.CurrentZone);
    }

    private void HandleSpinClicked()
    {
        if (wheelView.IsSpinning) return;

        int targetIndex = _resolver.ResolveSliceIndex(_session.CurrentWheel);

        wheelView.SetSpinInteractable(false);
        rewardsPanelView.SetLeaveInteractable(false);

        wheelView.Spin(targetIndex, () => HandleSpinCompleted(targetIndex));
    }

    private void HandleSpinCompleted(int targetIndex)
    {
        bool isBomb = _session.ResolveSpin(targetIndex);

        if (isBomb)
        {
            Debug.LogWarning("<color=red>[SONUÇ]</color> BOMBA VURDU! Pop-up açılıyor.");
            bombPopupView.Show();
        }
        else
        {
            Debug.Log($"<color=green>[SONUÇ]</color> Ödül kazanıldı! Yeni Zone: {_session.CurrentZone} | Farklı Ödül Sayısı: {_session.Inventory.Items.Count}");
        }
    }

    private void HandleGiveUpClicked()
    {
        bombPopupView.Hide();
        _session.Reset();
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