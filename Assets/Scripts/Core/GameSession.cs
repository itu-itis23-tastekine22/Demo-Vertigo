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

    // PDF kuralı: Oyuncu yalnızca Safe veya Super zone'larda ödüllerini alıp çekilebilir (Cash out/Leave).
    public bool CanLeave => CurrentZoneType == WheelZoneType.Safe || CurrentZoneType == WheelZoneType.Super;

    public GameSession(ZoneSettings settings, RewardInventory inventory)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
        _currentZone = 1;
    }

    /// <summary>
    /// Çevirme sonucunu çözümler.
    /// Bomba geldiyse true döner (envanter silinmez; pop-up kararına bırakılır).
    /// Ödül geldiyse miktar ölçeklenip envantere eklenir, zone artırılır ve false döner.
    /// </summary>
    public bool ResolveSpin(int sliceIndex)
    {
        var wheel = CurrentWheel;

        if (sliceIndex < 0 || sliceIndex >= wheel.Slices.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(sliceIndex), "Geçersiz dilim indeksi.");
        }

        var slice = wheel.Slices[sliceIndex];

        // NOT: WheelSlice yapındaki alan adlarına göre 'slice.IsBomb', 'slice.Reward' ve 'slice.Amount' isimlerini kontrol edebilirsin.
        if (slice.IsBomb)
        {
            return true;
        }

        int scaledAmount = _settings.GetScaledAmount(slice.Amount, _currentZone);
        _inventory.Add(slice.Data, scaledAmount);

        _currentZone++;
        ZoneChanged?.Invoke();

        return false;
    }

    /// <summary>
    /// Oyunu başlangıç durumuna döndürür: Zone 1'e alınır ve envanter sıfırlanır.
    /// </summary>
    public void Reset()
    {
        _currentZone = 1;
        _inventory.Clear();
        ZoneChanged?.Invoke();
    }

    /// <summary>
    /// Verilen herhangi bir zone numarasının türünü döner.
    /// </summary>
    public WheelZoneType GetZoneType(int zone)
    {
        return _settings.GetZoneType(zone);
    }
}