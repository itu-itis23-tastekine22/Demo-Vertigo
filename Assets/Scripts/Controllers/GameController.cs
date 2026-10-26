using UnityEngine;
using UnityEngine.Serialization;
using VertigoWheel.Core;
using VertigoWheel.Data;
using VertigoWheel.Events;
using VertigoWheel.Views;
using VertigoWheel.Infrastructure;
using System.Collections.Generic;

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
            IWallet wallet = new Wallet(walletStorage, _eventBus);

            _wallet = new Wallet(walletStorage, _eventBus);
            _session = new GameSession(wheelBuilder, spinResolver, inventory, _wallet, _eventBus);
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<ZoneChangedEvent>(HandleZoneChanged);
            _eventBus.Subscribe<RunStateChangedEvent>(HandleRunStateChanged);
            _eventBus.Subscribe<RunInventoryChangedEvent>(HandleInventoryChanged);
            _eventBus.Subscribe<WalletChangedEvent>(HandleWalletChanged);

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
            _eventBus.Unsubscribe<WalletChangedEvent>(HandleWalletChanged);

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
            RefreshWalletPanel();
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