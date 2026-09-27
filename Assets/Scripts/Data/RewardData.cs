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
    [SerializeField] private bool scalesWithZone = true;
    public bool ScalesWithZone => scalesWithZone;

    
    public string DisplayName => displayName;
    public RewardType Type => rewardType;
    public Sprite Icon => icon;

    //Bomba Kontrolü
    public bool IsBomb => rewardType == RewardType.Bomb;


}

/// <summary>
/// Çark dilimlerinde veya kazanılan envanter listesinde ödül verileriyle miktari eşleme
/// </summary>

[Serializable]
public class WheelSliceData
{
    [SerializeField] private RewardData data;     
    [SerializeField] private int amount;          

    public RewardData Data => data;               
    public int Amount => amount;                  

    public bool IsEmpty => data == null;
    public bool IsBomb => data != null && data.IsBomb;
}