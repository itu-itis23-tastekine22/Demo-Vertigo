using VertigoWheel.Data;

namespace VertigoWheel.Core
{
    public readonly struct WheelSlice
    {
        private WheelSlice(RewardData reward, int amount, bool isBomb)
        {
            Reward = reward;
            Amount = amount;
            IsBomb = isBomb;
        }

        public RewardData Reward { get; }

        public int Amount { get; }

        public bool IsBomb { get; }

        public static WheelSlice CreateReward(RewardData reward, int amount)
        {
            return new WheelSlice(reward, amount, false);
        }

        public static WheelSlice CreateBomb()
        {
            return new WheelSlice(null, 0, true);
        }
    }
}