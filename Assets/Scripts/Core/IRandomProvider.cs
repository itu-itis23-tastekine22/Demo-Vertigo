public interface IRandomProvider
{
    /// <summary>
    /// minInclusive dahil, maxExclusive hariç rastgele bir tam sayı döndürür.
    /// </summary>
    int Range(int minInclusive, int maxExclusive);
}