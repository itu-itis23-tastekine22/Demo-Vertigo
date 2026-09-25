using System;
using UnityEngine;

public enum RewardType
{
    Bomb,
    Cash,
    Gold,
    WeaponPoints,
    Armor,
    Consumable,
    Chest,
    Weapon,     // YENİ
    Cosmetic    // YENİ
}

[CreateAssetMenu(fileName = "Reward_New", menuName = "Wheel Game/Reward Data")]
public class RewardData : ScriptableObject
{
    [Header("Reward Definition")]
    
    [SerializeField] private string displayName;
    [SerializeField] private RewardType rewardType;

    [Header("Visual")]
    [SerializeField] private Sprite icon;

    
    public string DisplayName => displayName;
    public RewardType Type => rewardType;
    public Sprite Icon => icon;

    /// <summary>
    /// Bu ödülün bomba olup olmadığını kontrol eder.
    /// </summary>
    public bool IsBomb => rewardType == RewardType.Bomb;


}

/// <summary>
/// Çark dilimlerinde (Wheel Slice) veya kazanılan envanter listesinde 
/// RewardData ile miktarı eşleştirmek için kullanılan serileştirilebilir model.
/// </summary>

[Serializable]
public class WheelSliceData
{
    [SerializeField] private RewardData data;     // DEĞİŞTİ: public → [SerializeField] private
    [SerializeField] private int amount;          // DEĞİŞTİ: public → [SerializeField] private

    public RewardData Data => data;               // YENİ
    public int Amount => amount;                  // YENİ

    public bool IsEmpty => data == null;
    public bool IsBomb => data != null && data.IsBomb;
}