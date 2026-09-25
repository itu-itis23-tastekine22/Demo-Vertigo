using UnityEngine;

public class UnityRandomProvider : IRandomProvider
{
    public int Range(int minInclusive, int maxExclusive)
    {
        return Random.Range(minInclusive, maxExclusive);
    }
}