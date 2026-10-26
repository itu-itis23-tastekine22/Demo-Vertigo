using System;
using System.Collections.Generic;
using VertigoWheel.Data;
using VertigoWheel.Events;

namespace VertigoWheel.Core
{
    public sealed class RewardInventory
    {
        private readonly Dictionary<RewardData, int> _items = new Dictionary<RewardData, int>();
        private readonly IEventBus _eventBus;

        public RewardInventory(IEventBus eventBus)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }

            _eventBus = eventBus;
        }

        public IReadOnlyDictionary<RewardData, int> Items
        {
            get { return _items; }
        }

        public bool IsEmpty
        {
            get { return _items.Count == 0; }
        }

        public void Add(RewardData reward, int amount)
        {
            if (!reward || amount <= 0)
            {
                return;
            }

            if (_items.TryGetValue(reward, out int currentAmount))
            {
                _items[reward] = currentAmount + amount;
            }
            else
            {
                _items.Add(reward, amount);
            }

            PublishChanged();
        }

        public void Clear()
        {
            if (_items.Count == 0)
            {
                return;
            }

            _items.Clear();
            PublishChanged();
        }

        private void PublishChanged()
        {
            _eventBus.Publish(new RunInventoryChangedEvent());
        }
    }
}