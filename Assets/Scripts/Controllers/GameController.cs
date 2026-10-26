using UnityEngine;
using UnityEngine.Serialization;
using VertigoWheel.Core;
using VertigoWheel.Data;
using VertigoWheel.Events;
using VertigoWheel.Views;
using VertigoWheel.Infrastructure;

namespace VertigoWheel.Controllers
{
    public class GameController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField, FormerlySerializedAs("zoneSettings")] private ZoneSettings _zoneSettings;

        [Header("Views")]
        [SerializeField, FormerlySerializedAs("wheelView")] private WheelView _wheelView;
        [SerializeField, FormerlySerializedAs("rewardsPanelView")] private RewardsPanelView _rewardsPanelView;
        [SerializeField, FormerlySerializedAs("zoneBarView")] private ZoneBarView _zoneBarView;
        [SerializeField, FormerlySerializedAs("bombPopupView")] private BombPopupView _bombPopupView;

        private IEventBus _eventBus;
        private GameSession _session;

        private void Awake()
        {
            _eventBus = new EventBus();

            IRandomProvider randomProvider = new UnityRandomProvider();
            RewardInventory inventory = new RewardInventory(_eventBus);
            WheelBuilder wheelBuilder = new WheelBuilder(_zoneSettings, randomProvider);
            SpinResolver spinResolver = new SpinResolver(randomProvider);
            IWalletStorage walletStorage = new PlayerPrefsWalletStorage();
            IWallet wallet = new Wallet(walletStorage, _eventBus);

            _session = new GameSession(wheelBuilder, spinResolver, inventory, wallet, _eventBus);
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<ZoneChangedEvent>(HandleZoneChanged);
            _eventBus.Subscribe<RunStateChangedEvent>(HandleRunStateChanged);
            _eventBus.Subscribe<RunInventoryChangedEvent>(HandleInventoryChanged);

            _wheelView.SpinClicked += HandleSpinClicked;
            _wheelView.SpinAnimationCompleted += HandleSpinAnimationCompleted;
            _rewardsPanelView.LeaveClicked += HandleLeaveClicked;
            _bombPopupView.GiveUpClicked += HandleGiveUpClicked;
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<ZoneChangedEvent>(HandleZoneChanged);
            _eventBus.Unsubscribe<RunStateChangedEvent>(HandleRunStateChanged);
            _eventBus.Unsubscribe<RunInventoryChangedEvent>(HandleInventoryChanged);

            if (_wheelView)
            {
                _wheelView.SpinClicked -= HandleSpinClicked;
                _wheelView.SpinAnimationCompleted -= HandleSpinAnimationCompleted;
            }

            if (_rewardsPanelView)
            {
                _rewardsPanelView.LeaveClicked -= HandleLeaveClicked;
            }

            if (_bombPopupView)
            {
                _bombPopupView.GiveUpClicked -= HandleGiveUpClicked;
            }
        }

        private void Start()
        {
            _bombPopupView.Hide();
            _zoneBarView.Initialize(_zoneSettings);
            _session.StartNewRun();
        }

        private void HandleSpinClicked()
        {
            if (_session.TryStartSpin(out int sliceIndex))
            {
                _wheelView.PlaySpin(sliceIndex);
            }
        }

        private void HandleSpinAnimationCompleted()
        {
            _session.CompleteSpin();
        }

        private void HandleLeaveClicked()
        {
            if (_session.TryLeave())
            {
                _session.StartNewRun();
            }
        }

        private void HandleGiveUpClicked()
        {
            _bombPopupView.Hide();
            _session.StartNewRun();
        }

        private void HandleZoneChanged(ZoneChangedEvent zoneChanged)
        {
            _wheelView.ShowWheel(_session.CurrentWheel);
            _zoneBarView.SetCurrentZone(zoneChanged.Zone);
        }

        private void HandleRunStateChanged(RunStateChangedEvent stateChanged)
        {
            _wheelView.SetSpinInteractable(_session.CanSpin);
            _rewardsPanelView.SetLeaveInteractable(_session.CanLeave);

            if (stateChanged.State == RunState.Dead)
            {
                _bombPopupView.Show();
            }
        }

        private void HandleInventoryChanged(RunInventoryChangedEvent inventoryChanged)
        {
            _rewardsPanelView.Refresh(_session.Inventory.Items);
        }
    }
}