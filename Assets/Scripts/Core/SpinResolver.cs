using System;

namespace VertigoWheel.Core
{
    public sealed class SpinResolver
    {
        private readonly IRandomProvider _randomProvider;

        public SpinResolver(IRandomProvider randomProvider)
        {
            if (randomProvider == null)
            {
                throw new ArgumentNullException(nameof(randomProvider));
            }

            _randomProvider = randomProvider;
        }

        public int ResolveSliceIndex(int sliceCount)
        {
            if (sliceCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sliceCount), "A wheel needs at least one slice.");
            }

            return _randomProvider.Range(0, sliceCount);
        }
    }
}