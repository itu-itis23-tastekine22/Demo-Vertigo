using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VertigoWheel.Data;

namespace VertigoWheel.Tests
{
    internal sealed class TestAssetFactory
    {
        private readonly List<ScriptableObject> _createdAssets = new List<ScriptableObject>();

        public RewardData CreateReward(string id, bool isScaledByZone)
        {
            RewardData reward = Create<RewardData>(id);
            SerializedObject serialized = new SerializedObject(reward);

            serialized.FindProperty("_displayName").stringValue = id;
            serialized.FindProperty("_isScaledByZone").boolValue = isScaledByZone;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            return reward;
        }

        public WheelConfig CreateWheel(string name, RewardData reward, int amount)
        {
            WheelConfig wheel = Create<WheelConfig>(name);
            SerializedObject serialized = new SerializedObject(wheel);
            SerializedProperty slices = serialized.FindProperty("_slices");

            slices.arraySize = WheelConfig.RequiredSliceCount;

            for (int i = 0; i < slices.arraySize; i++)
            {
                SerializedProperty slice = slices.GetArrayElementAtIndex(i);
                slice.FindPropertyRelative("_data").objectReferenceValue = reward;
                slice.FindPropertyRelative("_amount").intValue = amount;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            return wheel;
        }

        public ZoneTypeData CreateZoneType(string name, int interval, int priority, bool hasBomb, bool isLeaveAllowed, params (int StartZone, WheelConfig Wheel)[] tiers)
        {
            ZoneTypeData zoneType = Create<ZoneTypeData>(name);
            SerializedObject serialized = new SerializedObject(zoneType);

            serialized.FindProperty("_interval").intValue = interval;
            serialized.FindProperty("_priority").intValue = priority;
            serialized.FindProperty("_hasBomb").boolValue = hasBomb;
            serialized.FindProperty("_isLeaveAllowed").boolValue = isLeaveAllowed;

            SerializedProperty wheelTiers = serialized.FindProperty("_wheelTiers");
            wheelTiers.arraySize = tiers.Length;

            for (int i = 0; i < tiers.Length; i++)
            {
                SerializedProperty tier = wheelTiers.GetArrayElementAtIndex(i);
                tier.FindPropertyRelative("_startZone").intValue = tiers[i].StartZone;
                tier.FindPropertyRelative("_wheel").objectReferenceValue = tiers[i].Wheel;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            return zoneType;
        }

        public ZoneSettings CreateZoneSettings(float rewardIncreaseRate, params ZoneTypeData[] zoneTypes)
        {
            ZoneSettings settings = Create<ZoneSettings>("ZoneSettings_Test");
            SerializedObject serialized = new SerializedObject(settings);

            serialized.FindProperty("_rewardIncreaseRate").floatValue = rewardIncreaseRate;

            SerializedProperty zoneTypeList = serialized.FindProperty("_zoneTypes");
            zoneTypeList.arraySize = zoneTypes.Length;

            for (int i = 0; i < zoneTypes.Length; i++)
            {
                zoneTypeList.GetArrayElementAtIndex(i).objectReferenceValue = zoneTypes[i];
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            return settings;
        }

        public void DestroyAll()
        {
            for (int i = 0; i < _createdAssets.Count; i++)
            {
                Object.DestroyImmediate(_createdAssets[i]);
            }

            _createdAssets.Clear();
        }

        private T Create<T>(string name) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            asset.name = name;
            _createdAssets.Add(asset);
            return asset;
        }
    }
}