using System;
using System.Collections.Generic;

public class RewardInventory
{
    
    private readonly Dictionary<RewardData, int> _items = new Dictionary<RewardData, int>();
    public IReadOnlyDictionary<RewardData, int> Items => _items;

    // Envanterde herhangi bir değişiklik olduğunda UI'ı haberdar etmek için event.
    public event Action Changed;

    // Envanterin boş olup olmadığını kontrol eden özellik
    public bool IsEmpty => _items.Count == 0;

    /// <summary>
    /// Ödülü envantere ekler. Zaten varsa miktarını artırır, yoksa yeni kayıt açar.
    /// </summary>
    public void Add(RewardData reward, int amount)
    {
        if (reward == null || amount <= 0) return;

        if (_items.ContainsKey(reward))
        {
            _items[reward] += amount;
        }
        else
        {
            _items[reward] = amount;
        }

        Changed?.Invoke();
    }

    /// <summary>
    /// Envanterdeki tüm ödülleri temizler. Bomba veya restart durumlarında kullanılır.
    /// </summary>
    public void Clear()
    {
        _items.Clear();
        Changed?.Invoke();
    }
}