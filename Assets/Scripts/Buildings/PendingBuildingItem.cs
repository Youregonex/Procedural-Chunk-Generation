using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PendingBuildingItem : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Transform _pendingBuildingItemTransformVisual;
    [SerializeField] private Color _canBuildColor;
    [SerializeField] private Color _canNotBuildColor;

    [Header("Debug Fields")]
    [SerializeField] private List<Collider2D> _collisions = new List<Collider2D>();
    [SerializeField] private SpriteRenderer _spritRenderer;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private BuildingItemDataSO _currentBuildingItemDataSO;
    [SerializeField] private BoxCollider2D _buildingCollider;
    [SerializeField] private float _buildingRange;

    [field: SerializeField] public bool CanPlaceBuilding { get; private set; }


    private void Awake()
    {
        _spritRenderer = _pendingBuildingItemTransformVisual.GetComponent<SpriteRenderer>();
        _buildingCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (_currentBuildingItemDataSO == null)
            return;

        transform.position = GetMouseGridPosition();

        if (CheckForCollisions() && CheckForRange())
        {
            CanPlaceBuilding = true;
            ChangeColor(true);
        }
        else
        {
            CanPlaceBuilding = false;
            ChangeColor(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _collisions.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(_collisions.Contains(collision))
            _collisions.Remove(collision);
    }

    public void RefreshPendingBuildingItem(BuildingItemDataSO buildingItemDataSO, float buildingRange, Transform playerTransform)
    {
        _pendingBuildingItemTransformVisual.gameObject.SetActive(true);
        _buildingCollider.enabled = true;
        _currentBuildingItemDataSO = buildingItemDataSO;
        _spritRenderer.sprite = buildingItemDataSO.Icon;
        _buildingCollider.size = buildingItemDataSO.BuildingSize;
        _buildingRange = buildingRange;
        _playerTransform = playerTransform;
    }

    public void HidePendingBuildingItem()
    {
        if (_currentBuildingItemDataSO == null)
            return;

        _collisions.Clear();
        _buildingCollider.enabled = false;
        _currentBuildingItemDataSO = null;
        _buildingRange = 0f;
        _playerTransform = null;
        _pendingBuildingItemTransformVisual.gameObject.SetActive(false);
    }

    private bool CheckForCollisions()
    {
        if (_collisions.Count == 0)
            return true;

        return false;
    }

    private bool CheckForRange()
    {
        if (Vector2.Distance(GetMouseGridPosition(), _playerTransform.position) > _buildingRange)
            return false;

        return true;
    }

    private void ChangeColor(bool canBuild)
    {
        _spritRenderer.color = canBuild ? _canBuildColor : _canNotBuildColor;
    }

    private Vector2 GetMouseGridPosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        return new Vector2(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y));
    }
}
