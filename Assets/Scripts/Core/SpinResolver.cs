using System;

public class SpinResolver
{
    private readonly IRandomProvider _randomProvider;
    public SpinResolver(IRandomProvider randomProvider)
    {
        _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
    }

    /// <summary>
    /// Çarkın dilim sayısına göre rastgele bir dilim indeksi (0 ile SliceCount - 1 arası) belirler.
    /// </summary>
    public int ResolveSliceIndex(WheelConfig config)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));


        int sliceCount = config.Slices.Count;

        if (sliceCount <= 0)
        {
            throw new InvalidOperationException("WheelConfig içerisinde en az 1 dilim bulunmalıdır.");
        }

        return _randomProvider.Range(0, sliceCount);
    }
}