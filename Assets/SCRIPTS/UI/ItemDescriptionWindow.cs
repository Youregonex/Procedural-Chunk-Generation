using UnityEngine;
using TMPro;

public class ItemDescriptionWindow : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private RectTransform _itemDescriptionUIPanel;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;

    private RectTransform _selfRectTransform;

    private void Awake()
    {
        _selfRectTransform = transform.GetComponent<RectTransform>();

        HideItemDescription();
    }

    public void HideItemDescription() => _itemDescriptionUIPanel.gameObject.SetActive(false);

    public void DisplayItemDescription(ItemDataSO itemDataSO, Vector2 position)
    {
        if (itemDataSO == null)
            return;

        SetItemDescriptionData(itemDataSO);
        transform.position = position;
        RectTransform panelRect = _itemDescriptionUIPanel.GetComponent<RectTransform>();

        if (_selfRectTransform.anchoredPosition.y >= 0 && _selfRectTransform.anchoredPosition.x <= 0) // Upper left part of screen
        {
            Vector2 bottomRightAnchor = new(1, 0);
            Vector2 topLeftPivot = new(0, 1);

            panelRect.anchorMin = bottomRightAnchor;
            panelRect.anchorMax = bottomRightAnchor;

            panelRect.pivot = topLeftPivot;
        }
        else if(_selfRectTransform.anchoredPosition.y < 0 && _selfRectTransform.anchoredPosition.x <= 0) // Bottom left part of screen
        {
            Vector2 topRightAnchor = new(1, 1);
            Vector2 bottomLeftPivot = new(0, 0);

            panelRect.anchorMin = topRightAnchor;
            panelRect.anchorMax = topRightAnchor;

            panelRect.pivot = bottomLeftPivot;
        }
        else if (_selfRectTransform.anchoredPosition.y >= 0 && _selfRectTransform.anchoredPosition.x > 0) // Upper right part of screen
        {
            Vector2 bottomLeftAnchor = new(0, 0);
            Vector2 topRightPivot = new(1, 1);

            panelRect.anchorMin = bottomLeftAnchor;
            panelRect.anchorMax = bottomLeftAnchor;

            panelRect.pivot = topRightPivot;

        }
        else if (_selfRectTransform.anchoredPosition.y < 0 && _selfRectTransform.anchoredPosition.x > 0) // Bottom right part of screen
        {
            Vector2 topLeftAnchor = new(0, 1);
            Vector2 bottomRightPivot = new(1, 0);

            panelRect.anchorMin = topLeftAnchor;
            panelRect.anchorMax = topLeftAnchor;

            panelRect.pivot = bottomRightPivot;
        }

        panelRect.sizeDelta = Vector2.zero;

        _itemDescriptionUIPanel.gameObject.SetActive(true);
    }

    private void SetItemDescriptionData(ItemDataSO itemData)
    {
        if (itemData == null)
            return;

        _itemNameText.text = itemData.Name;
        _itemDescriptionText.text = itemData.Description;
    }
}
