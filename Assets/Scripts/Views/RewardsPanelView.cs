using System;
using UnityEngine;
using UnityEngine.UI;

public class RewardsPanelView : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button leaveButton;

    public event Action LeaveClicked;

    private void OnValidate()
    {
        if (leaveButton == null)
        {
            leaveButton = GetComponentInChildren<Button>();
        }
    }

    private void OnEnable()
    {
        if (leaveButton != null)
        {
            leaveButton.onClick.AddListener(OnLeaveButtonClicked);
        }
    }

    private void OnDisable()
    {
        if (leaveButton != null)
        {
            leaveButton.onClick.RemoveListener(OnLeaveButtonClicked);
        }
    }

    private void OnLeaveButtonClicked()
    {
        LeaveClicked?.Invoke();
    }

    public void SetLeaveInteractable(bool value)
    {
        if (leaveButton != null)
        {
            leaveButton.interactable = value;
        }
    }
}