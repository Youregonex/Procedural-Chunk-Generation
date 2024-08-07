using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

[Serializable]
public class Item : MonoBehaviour, IGenerateSaveData
{
    public event Action<Item> OnDestruction;

    [Header("Item Drop Config")]
    [SerializeField] private float _nodeDropMoveSpeed = 3f;
    [SerializeField] private float _agentDropMoveSpeed = 5f;
    [SerializeField] private float _moveDuration = .2f;

    [Header("Drop Animation")]
    [SerializeField] private float _scaleFrom = 2f;
    [SerializeField] private float _dropAnimationTime = .5f;
    [SerializeField] private float _dropColliderDisableTime = .3f;

    [Header("Debug Fields")]
    [SerializeField] private ItemDataSO _itemDataSO;
    [SerializeField] private int _itemQuantity;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private BoxCollider2D _capsuleCollider;

    private Tween _currentAnimation;

    public ItemDataSO ItemDataSO => _itemDataSO;
    public int ItemQuantity => _itemQuantity;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        StartCoroutine(DisableColliderCoroutine());
    }

    private void OnDestroy()
    {
        _currentAnimation.Kill();

        OnDestruction?.Invoke(this);
    }

    public SaveData GenerateSaveData()
    {
        ItemSaveData itemSaveData = new ItemSaveData(transform.position, _itemDataSO, _itemQuantity);

        return itemSaveData;
    }

    public void LoadFromSaveData(SaveData saveData)
    {
        ItemSaveData itemSaveData = saveData as ItemSaveData;

        transform.position = itemSaveData.position;
        _itemDataSO = itemSaveData.itemDataSO;
        _itemQuantity = itemSaveData.quantity;
    }

    public void DestroyItem()
    {
        Destroy(gameObject);
    }

    public void ChangeItemQuantity(int newQuantity)
    {
        _itemQuantity = newQuantity;
    }

    public void SetItemData(ItemDataSO itemDataSO, int itemQuantity)
    {
        _itemDataSO = itemDataSO;
        _itemQuantity = itemQuantity;
    }

    public void DropInRandomDirection()
    {
        StartCoroutine(MoveInDirectionCoroutine(GetRandomMovementVector(), _nodeDropMoveSpeed));
    }

    public void DropInDirection(Vector2 targetDirection)
    {
        Vector2 moveDirectionNormalized = (targetDirection - (Vector2)transform.position).normalized;
        StartCoroutine(MoveInDirectionCoroutine(moveDirectionNormalized, _agentDropMoveSpeed));
    }

    private IEnumerator MoveInDirectionCoroutine(Vector2 movementVectorNormalized, float dropMoveSpeed)
    {
        _rigidBody.velocity = movementVectorNormalized * dropMoveSpeed;
        PlayDropAnimation();
        yield return new WaitForSeconds(_moveDuration);

        _rigidBody.velocity = Vector2.zero;
    }

    private IEnumerator DisableColliderCoroutine()
    {
        _capsuleCollider.enabled = false;

        yield return new WaitForSeconds(_dropColliderDisableTime);

        _capsuleCollider.enabled = true;
    }

    private Vector2 GetRandomMovementVector()
    {
        return UnityEngine.Random.insideUnitCircle;
    }

    private void PlayDropAnimation()
    {
        _currentAnimation = transform.DOScale(_scaleFrom, _dropAnimationTime)
                                     .From()
                                     .SetEase(Ease.InOutBack);
    }
}