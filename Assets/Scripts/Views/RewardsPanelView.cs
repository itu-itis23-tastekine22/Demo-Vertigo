using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VertigoWheel.Events;

namespace VertigoWheel.Views
{
    public class RewardsPanelView : MonoBehaviour
    {
        [SerializeField] private RewardListView _rewardList;
        [SerializeField, FormerlySerializedAs("leaveButton")] private Button _leaveButton;

        private IEventBus _eventBus;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_rewardList)
            {
                _rewardList = GetComponentInChildren<RewardListView>(true);
            }

            if (!_leaveButton)
            {
                _leaveButton = GetComponentInChildren<Button>(true);
            }
        }
#endif

        private void OnEnable()
        {
            _leaveButton.onClick.AddListener(HandleLeaveButtonClicked);
        }

        private void OnDisable()
        {
            _leaveButton.onClick.RemoveListener(HandleLeaveButtonClicked);
        }

        public void Initialize(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void ShowRewards(IReadOnlyList<RewardListEntry> entries)
        {
            _rewardList.Show(entries);
        }

        public void SetLeaveInteractable(bool isInteractable)
        {
            _leaveButton.interactable = isInteractable;
        }

        private void HandleLeaveButtonClicked()
        {
            _eventBus.Publish(new LeaveRequestedEvent());
        }
    }
}