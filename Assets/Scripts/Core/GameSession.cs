using System;

public class GameSession
{
    private readonly ZoneSettings _settings;
    private readonly RewardInventory _inventory;
    private int _currentZone;

    public event Action ZoneChanged;

    public int CurrentZone => _currentZone;
    public WheelZoneType CurrentZoneType => _settings.GetZoneType(_currentZone);
    public WheelConfig CurrentWheel => _settings.GetWheelConfig(_currentZone);
    public RewardInventory Inventory => _inventory;

    public bool CanLeave => CurrentZoneType == WheelZoneType.Safe || CurrentZoneType == WheelZoneType.Super;

    public GameSession(ZoneSettings settings, RewardInventory inventory)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
        _currentZone = 1;
    }

    /// <summary>
    /// Verilen dilimin mevcut zone'a göre nihai miktarını hesaplar.
    /// Ödül ölçeklenmiyorsa (silah vb.) temel miktar döner; ölçekleniyorsa zone çarpanı uygulanır.
    /// </summary>
    public int GetSliceAmount(WheelSliceData slice)
    {
        if (slice == null || slice.IsBomb || slice.Data == null)
        {
            return 0;
        }

        if (!slice.Data.ScalesWithZone)
        {
            return slice.Amount;
        }

        return _settings.GetScaledAmount(slice.Amount, _currentZone);
    }

    public bool ResolveSpin(int sliceIndex)
    {
        var wheel = CurrentWheel;

        if (sliceIndex < 0 || sliceIndex >= wheel.Slices.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(sliceIndex), "Geçersiz dilim indeksi.");
        }

        var slice = wheel.Slices[sliceIndex];

        if (slice.IsBomb)
        {
            return true;
        }

        // Dilimin hesaplanmış gerçek miktarı ekleniyor
        int finalAmount = GetSliceAmount(slice);
        _inventory.Add(slice.Data, finalAmount);

        _currentZone++;
        ZoneChanged?.Invoke();

        return false;
    }

    public void Reset()
    {
        _currentZone = 1;
        _inventory.Clear();
        ZoneChanged?.Invoke();
    }

    public WheelZoneType GetZoneType(int zone)
    {
        return _settings.GetZoneType(zone);
    }
}