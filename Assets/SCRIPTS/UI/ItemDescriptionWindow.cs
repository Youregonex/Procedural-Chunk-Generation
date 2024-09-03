using UnityEngine;
using TMPro;

public class ItemDescriptionWindow : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private RectTransform _itemDescriptionUIPanel;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;

    private RectTransform _selfRectTransform;
    private Vector2 _anchorPosition;
    private Vector2 _pivotPosition;

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

        if (_selfRectTransform.anchoredPosition.y >= 0 &&
            _selfRectTransform.anchoredPosition.x <= 0) // Upper left part of screen
        {
            _anchorPosition = new(1, 0);    // Bottom-right
            _pivotPosition = new(0, 1);     // Top-left
        }
        else if(_selfRectTransform.anchoredPosition.y < 0 &&
                _selfRectTransform.anchoredPosition.x <= 0) // Bottom left part of screen
        {
            _anchorPosition = new(1, 1);    // Top-right
            _pivotPosition = new(0, 0);     // Bottom-left
        }
        else if (_selfRectTransform.anchoredPosition.y >= 0 &&
                 _selfRectTransform.anchoredPosition.x > 0) // Upper right part of screen
        {
            _anchorPosition = new(0, 0);    // Bottom-left
            _pivotPosition = new(1, 1);     // Top-right
        }
        else if (_selfRectTransform.anchoredPosition.y < 0 &&
                 _selfRectTransform.anchoredPosition.x > 0) // Bottom right part of screen
        {
            _anchorPosition = new(0, 1);    // Top-left
            _pivotPosition = new(1, 0);     //Bottom-right
        }

        panelRect.anchorMin = _anchorPosition;
        panelRect.anchorMax = _anchorPosition;

        panelRect.pivot = _pivotPosition;

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
