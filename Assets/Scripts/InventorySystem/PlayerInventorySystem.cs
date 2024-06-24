using UnityEngine;
using System;

[Serializable]
[RequireComponent(typeof(PlayerInput))]
public class PlayerInventorySystem : MonoBehaviour
{
    private const int HOTBAR_SIZE = 10;

    public static event EventHandler<OnInventoryDisplayRequestedEventArgs> OnInventoryDisplayRequested;
    public class OnInventoryDisplayRequestedEventArgs : EventArgs
    {
        public Inventory inventory;
    }

    public event EventHandler OnHotbarInventorySlotChanged;

    [SerializeField] private int _mainInventorySize;
    [SerializeField] private Inventory _hotbar;
    [SerializeField] private Inventory _mainInventory;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _hotbar = new Inventory();
        _mainInventory = new Inventory();

        _hotbar.InitializeInventory(HOTBAR_SIZE);
        _mainInventory.InitializeInventory(_mainInventorySize);
    }

    private void Hotbar_OnInventorySlotChanged(object sender, EventArgs e)
    {
        OnHotbarInventorySlotChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        _playerInput.OnInventoryKeyPressed += PlayerInput_OnInventoryKeyPressed;
        _hotbar.OnInventorySlotChanged += Hotbar_OnInventorySlotChanged;
    }

    private void PlayerInput_OnInventoryKeyPressed(object sender, System.EventArgs e)
    {
        OnInventoryDisplayRequested?.Invoke(this, new OnInventoryDisplayRequestedEventArgs
        {
            inventory = _mainInventory
        });
    }

    private void OnDestroy()
    {
        _playerInput.OnInventoryKeyPressed -= PlayerInput_OnInventoryKeyPressed;
        _hotbar.OnInventorySlotChanged -= Hotbar_OnInventorySlotChanged;
    }

    public bool AddItemToInventory(Item item)
    {
        if(_hotbar.AddItemToInventory(item))
        {
            return true;
        }

        return _mainInventory.AddItemToInventory(item);
    }

    public int GetHotbarSize() => HOTBAR_SIZE;
    public Inventory GetHotbarInventory() => _hotbar;
}
