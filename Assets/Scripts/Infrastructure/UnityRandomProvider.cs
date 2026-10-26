using UnityEngine;
using VertigoWheel.Core;

namespace VertigoWheel.Infrastructure
{
    public sealed class UnityRandomProvider : IRandomProvider
    {
        public int Range(int minInclusive, int maxExclusive)
        {
            return Random.Range(minInclusive, maxExclusive);
        }
    }
}