using VertigoWheel.Core;

namespace VertigoWheel.Events
{
    public readonly struct RunStateChangedEvent
    {
        public RunStateChangedEvent(RunState state)
        {
            State = state;
        }

        public RunState State { get; }
    }

    public readonly struct ZoneChangedEvent
    {
        public ZoneChangedEvent(int zone)
        {
            Zone = zone;
        }

        public int Zone { get; }
    }

    public readonly struct SpinStartedEvent
    {
        public SpinStartedEvent(int sliceIndex)
        {
            SliceIndex = sliceIndex;
        }

        public int SliceIndex { get; }
    }

    public readonly struct RunInventoryChangedEvent
    {
    }

    public readonly struct WalletChangedEvent
    {
    }

    public readonly struct SpinRequestedEvent
    {
    }

    public readonly struct SpinAnimationCompletedEvent
    {
    }

    public readonly struct LeaveRequestedEvent
    {
    }

    public readonly struct GiveUpRequestedEvent
    {
    }
}