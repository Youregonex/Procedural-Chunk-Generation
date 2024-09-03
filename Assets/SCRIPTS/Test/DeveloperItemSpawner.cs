using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class DeveloperItemSpawner : NetworkBehaviour
{
    [Header("Config")]
    [SerializeField] private Image _itemSpawnerWindow;
    [SerializeField] private Image _buttonContainer;
    [SerializeField] private Button _buttonPrefab;

    [Header("Debug Field")]
    [SerializeField] private List<Button> _buttonList;
    [SerializeField] private List<ItemDataSO> _itemList;
    [SerializeField] private bool _isOpened = false;

    private void Start()
    {
        _itemList = MultiplayerPrefabDatabase.Instance.ItemDataSOList;

        for (int i = 0; i < _itemList.Count; i++)
        {
            Button button = Instantiate(_buttonPrefab);
            button.transform.SetParent(_buttonContainer.transform);
            button.transform.GetChild(0).GetComponent<Image>().sprite = _itemList[i].Icon;
            button.GetComponent<SpawnButton>().SetItemDataSO(_itemList[i]);

            button.onClick.AddListener(() =>
            {
                ButtonPressed(button);
            });

            _buttonList.Add(button);
        }

        Hide();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q))
        {
            Show();
        }
    }

    private void ButtonPressed(Button button)
    {
        SpawnButton spawnButton = button.GetComponent<SpawnButton>();

        int itemDataSOIndex = MultiplayerPrefabDatabase.Instance.GetIdWithItemDataSO(spawnButton.ButtonItemDataSO);
        SpawnItemServerRpc(itemDataSOIndex);
    }


    [Rpc(SendTo.Server)]
    private void SpawnItemServerRpc(int itemDataSOIndex)
    {
        ItemDataSO itemDataSO = MultiplayerPrefabDatabase.Instance.GetItemDataSOWithId(itemDataSOIndex);
        Item item = Instantiate(itemDataSO.ItemPrefab).GetComponent<Item>();
        item.NetworkObject.Spawn();
        item.transform.position = Vector2.zero;
    }


    private void Show()
    {
        if(_isOpened)
        {
            Hide();
            return;
        }

        _isOpened = true;
        _itemSpawnerWindow.gameObject.SetActive(true);
    }

    private void Hide()
    {
        _isOpened = false;
        _itemSpawnerWindow.gameObject.SetActive(false);
    }
}
