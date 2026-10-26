using System;
using VertigoWheel.Events;
using System.Collections.Generic;
using VertigoWheel.Data;

namespace VertigoWheel.Core
{
    public sealed class GameSession
    {
        private const int FirstZone = 1;
        private const int NoPendingSlice = -1;

        private readonly WheelBuilder _wheelBuilder;
        private readonly SpinResolver _spinResolver;
        private readonly RewardInventory _inventory;
        private readonly IEventBus _eventBus;
        private readonly IWallet _wallet;

        private int _currentZone;
        private RunState _state;
        private RuntimeWheel _currentWheel;
        private int _pendingSliceIndex = NoPendingSlice;

        public GameSession(WheelBuilder wheelBuilder, SpinResolver spinResolver, RewardInventory inventory, IWallet wallet, IEventBus eventBus)
        {
            if (wheelBuilder == null)
            {
                throw new ArgumentNullException(nameof(wheelBuilder));
            }

            if (spinResolver == null)
            {
                throw new ArgumentNullException(nameof(spinResolver));
            }

            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            if (wallet == null)
            {
                throw new ArgumentNullException(nameof(wallet));
            }

            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }

            _wheelBuilder = wheelBuilder;
            _spinResolver = spinResolver;
            _inventory = inventory;
            _wallet = wallet;
            _eventBus = eventBus;
        }

        public int CurrentZone
        {
            get { return _currentZone; }
        }

        public RunState State
        {
            get { return _state; }
        }

        public RuntimeWheel CurrentWheel
        {
            get { return _currentWheel; }
        }

        public RewardInventory Inventory
        {
            get { return _inventory; }
        }

        public bool CanSpin
        {
            get { return _state == RunState.Idle; }
        }

        public bool CanLeave
        {
            get { return _state == RunState.Idle && _currentWheel.ZoneType.IsLeaveAllowed; }
        }

        public void StartNewRun()
        {
            _inventory.Clear();
            _currentZone = FirstZone;
            EnterCurrentZone();
        }

        public bool TryStartSpin(out int sliceIndex)
        {
            if (!CanSpin)
            {
                sliceIndex = NoPendingSlice;
                return false;
            }

            sliceIndex = _spinResolver.ResolveSliceIndex(_currentWheel.Slices.Count);
            _pendingSliceIndex = sliceIndex;

            SetState(RunState.Spinning);
            _eventBus.Publish(new SpinStartedEvent(sliceIndex));
            return true;
        }

        public void CompleteSpin()
        {
            if (_state != RunState.Spinning)
            {
                throw new InvalidOperationException("CompleteSpin can only be called while the wheel is spinning.");
            }

            WheelSlice landedSlice = _currentWheel.Slices[_pendingSliceIndex];
            _pendingSliceIndex = NoPendingSlice;

            if (landedSlice.IsBomb)
            {
                EndRunWithBomb();
                return;
            }

            CollectReward(landedSlice);
            AdvanceZone();
        }

        public bool TryLeave()
{
            if (!CanLeave)
            {
                return false;
            }

            BankRunRewards();
            _inventory.Clear();
            SetState(RunState.Left);
            return true;
        }

        private void BankRunRewards()
        {
            List<KeyValuePair<string, int>> deposits = new List<KeyValuePair<string, int>>(_inventory.Items.Count);

            foreach (KeyValuePair<RewardData, int> item in _inventory.Items)
            {
                deposits.Add(new KeyValuePair<string, int>(item.Key.Id, item.Value));
            }

            _wallet.DepositAll(deposits);
        }

        private void EndRunWithBomb()
        {
            _inventory.Clear();
            SetState(RunState.Dead);
        }

        private void CollectReward(WheelSlice slice)
        {
            _inventory.Add(slice.Reward, slice.Amount);
        }

        private void AdvanceZone()
        {
            _currentZone++;
            EnterCurrentZone();
        }

        private void EnterCurrentZone()
        {
            _currentWheel = _wheelBuilder.Build(_currentZone);
            _eventBus.Publish(new ZoneChangedEvent(_currentZone));
            SetState(RunState.Idle);
        }

        private void SetState(RunState state)
        {
            _state = state;
            _eventBus.Publish(new RunStateChangedEvent(state));
        }
    }
}