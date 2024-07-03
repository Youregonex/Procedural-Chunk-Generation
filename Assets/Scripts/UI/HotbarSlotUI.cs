using UnityEngine;
using UnityEngine.UI;

public class HotbarSlotUI : InventorySlotUI
{
    [Header("Config")]
    [SerializeField] private Color _selectColor;
    [SerializeField] private Color _unSelectColor;
    [SerializeField] private Image _selectionBorder;

    [Header("Debug Fields")]
    [SerializeField] private bool _isSelected = false;

    public bool IsSelected => _isSelected;

    public void SelectSlot()
    {
        _selectionBorder.color = _selectColor;
    }

    public void DeselectSlot()
    {
        _selectionBorder.color = _unSelectColor;
    }
}
