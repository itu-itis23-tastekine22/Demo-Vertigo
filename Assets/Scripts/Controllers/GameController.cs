using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using VertigoWheel.Core;
using VertigoWheel.Data;
using VertigoWheel.Events;
using VertigoWheel.Infrastructure;
using VertigoWheel.Views;

namespace VertigoWheel.Controllers
{
    public class GameController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField, FormerlySerializedAs("zoneSettings")] private ZoneSettings _zoneSettings;
        [SerializeField] private RewardCatalog _rewardCatalog;

        [Header("Views")]
        [SerializeField, FormerlySerializedAs("wheelView")] private WheelView _wheelView;
        [SerializeField, FormerlySerializedAs("rewardsPanelView")] private RewardsPanelView _rewardsPanelView;
        [SerializeField, FormerlySerializedAs("zoneBarView")] private ZoneBarView _zoneBarView;
        [SerializeField, FormerlySerializedAs("bombPopupView")] private BombPopupView _bombPopupView;
        [SerializeField] private WalletPanelView _walletPanelView;

        private readonly List<RewardListEntry> _entryBuffer = new List<RewardListEntry>();
        private IEventBus _eventBus;
        private IWallet _wallet;
        private GameSession _session;

        private void Awake()
        {
            _eventBus = new EventBus();

            IRandomProvider randomProvider = new UnityRandomProvider();
            RewardInventory inventory = new RewardInventory(_eventBus);
            WheelBuilder wheelBuilder = new WheelBuilder(_zoneSettings, randomProvider);
            SpinResolver spinResolver = new SpinResolver(randomProvider);
            IWalletStorage walletStorage = new PlayerPrefsWalletStorage();

            _wallet = new Wallet(walletStorage, _eventBus);
            _session = new GameSession(wheelBuilder, spinResolver, inventory, _wallet, _eventBus);

            _wheelView.Initialize(_eventBus);
            _rewardsPanelView.Initialize(_eventBus);
            _bombPopupView.Initialize(_eventBus);
        }

        private void OnEnable()
        {
            if (_eventBus == null)
            {
                return;
            }

            _eventBus.Subscribe<SpinRequestedEvent>(HandleSpinRequested);
            _eventBus.Subscribe<SpinAnimationCompletedEvent>(HandleSpinAnimationCompleted);
            _eventBus.Subscribe<LeaveRequestedEvent>(HandleLeaveRequested);
            _eventBus.Subscribe<GiveUpRequestedEvent>(HandleGiveUpRequested);
            _eventBus.Subscribe<ZoneChangedEvent>(HandleZoneChanged);
            _eventBus.Subscribe<RunStateChangedEvent>(HandleRunStateChanged);
            _eventBus.Subscribe<RunInventoryChangedEvent>(HandleInventoryChanged);
            _eventBus.Subscribe<WalletChangedEvent>(HandleWalletChanged);
        }

        private void OnDisable()
        {
            if (_eventBus == null)
            {
                return;
            }
            _eventBus.Unsubscribe<SpinRequestedEvent>(HandleSpinRequested);
            _eventBus.Unsubscribe<SpinAnimationCompletedEvent>(HandleSpinAnimationCompleted);
            _eventBus.Unsubscribe<LeaveRequestedEvent>(HandleLeaveRequested);
            _eventBus.Unsubscribe<GiveUpRequestedEvent>(HandleGiveUpRequested);
            _eventBus.Unsubscribe<ZoneChangedEvent>(HandleZoneChanged);
            _eventBus.Unsubscribe<RunStateChangedEvent>(HandleRunStateChanged);
            _eventBus.Unsubscribe<RunInventoryChangedEvent>(HandleInventoryChanged);
            _eventBus.Unsubscribe<WalletChangedEvent>(HandleWalletChanged);
        }

        private void Start()
        {
            _bombPopupView.Hide();
            _zoneBarView.Initialize(_zoneSettings);
            RefreshWalletPanel();
            _session.StartNewRun();
        }

        private void HandleSpinRequested(SpinRequestedEvent spinRequested)
        {
            if (_session.TryStartSpin(out int sliceIndex))
            {
                _wheelView.PlaySpin(sliceIndex);
            }
        }

        private void HandleSpinAnimationCompleted(SpinAnimationCompletedEvent spinCompleted)
        {
            _session.CompleteSpin();
        }

        private void HandleLeaveRequested(LeaveRequestedEvent leaveRequested)
        {
            if (_session.TryLeave())
            {
                _session.StartNewRun();
            }
        }

        private void HandleGiveUpRequested(GiveUpRequestedEvent giveUpRequested)
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
            _entryBuffer.Clear();

            foreach (KeyValuePair<RewardData, int> item in _session.Inventory.Items)
            {
                _entryBuffer.Add(new RewardListEntry(item.Key.Icon, item.Value));
            }

            _rewardsPanelView.ShowRewards(_entryBuffer);
        }

        private void HandleWalletChanged(WalletChangedEvent walletChanged)
        {
            RefreshWalletPanel();
        }

        private void RefreshWalletPanel()
        {
            _entryBuffer.Clear();

            foreach (KeyValuePair<string, int> balance in _wallet.Balances)
            {
                if (_rewardCatalog.TryGetReward(balance.Key, out RewardData reward))
                {
                    _entryBuffer.Add(new RewardListEntry(reward.Icon, balance.Value));
                }
            }

            _walletPanelView.ShowBalances(_entryBuffer);
        }
    }
}