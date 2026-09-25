using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardsPanelView : MonoBehaviour
{
    [Header("List Container & Prefab")]
    [SerializeField] private RectTransform content;
    [SerializeField] private RewardItemView itemPrefab;

    [Header("Buttons")]
    [SerializeField] private Button leaveButton;

    private readonly Dictionary<RewardData, RewardItemView> itemViews = new Dictionary<RewardData, RewardItemView>();

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

    public void Refresh(IReadOnlyDictionary<RewardData, int> items)
    {
        if (items == null || items.Count == 0)
        {
            foreach (var view in itemViews.Values)
            {
                if (view != null)
                {
                    Destroy(view.gameObject);
                }
            }

            itemViews.Clear();
            return;
        }

        foreach (var pair in items)
        {
            RewardData reward = pair.Key;
            int amount = pair.Value;

            if (!itemViews.TryGetValue(reward, out var view))
            {
                view = Instantiate(itemPrefab, content);
                itemViews.Add(reward, view);
            }

            view.Setup(reward, amount);
        }
    }
}