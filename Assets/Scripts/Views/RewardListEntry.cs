using UnityEngine;

namespace VertigoWheel.Views
{
    public readonly struct RewardListEntry
    {
        public RewardListEntry(Sprite icon, int amount)
        {
            Icon = icon;
            Amount = amount;
        }

        public Sprite Icon { get; }

        public int Amount { get; }
    }
}