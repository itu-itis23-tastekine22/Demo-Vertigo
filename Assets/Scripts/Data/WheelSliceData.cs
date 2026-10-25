using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VertigoWheel.Data
{
    [Serializable]
    public class WheelSliceData
    {
        [SerializeField, FormerlySerializedAs("data")] private RewardData _data;
        [SerializeField, FormerlySerializedAs("amount"), Min(0)] private int _amount;

        public RewardData Data
        {
            get { return _data; }
        }

        public int Amount
        {
            get { return _amount; }
        }

        public bool IsEmpty
        {
            get { return _data == null; }
        }

        public bool IsBomb
        {
            get { return !IsEmpty && _data.IsBomb; }
        }
    }
}