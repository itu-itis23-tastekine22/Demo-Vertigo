namespace VertigoWheel.Core
{
    public interface IRandomProvider
    {
        int Range(int minInclusive, int maxExclusive);
    }
}