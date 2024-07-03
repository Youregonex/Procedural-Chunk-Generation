using UnityEngine;
using System.Collections;

[System.Serializable]
public class Item : MonoBehaviour
{
    [Header("Item Drop Config")]
    [SerializeField] private float _nodeDropMoveSpeed = 3f;
    [SerializeField] private float _nodeDropMoveTime = .2f;

    [SerializeField] private float _agentDropMoveSpeed = 5f;
    [SerializeField] private float _agentDropMoveTime = .3f;

    [SerializeField] private float _dropColliderDisableTime = .3f;

    [SerializeField] private ItemDataSO _itemDataSO;

    [Header("Debug Fields")]
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private int _itemQuantity;
    [SerializeField] private CapsuleCollider2D _capsuleCollider;

    public ItemDataSO ItemDataSO => _itemDataSO;
    public int ItemQuantity => _itemQuantity;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void OnEnable()
    {
        StartCoroutine(DisableColliderCoroutine());
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

    public void Drop()
    {
        DropInRandomDirection();
    }

    public void DropInRandomDirection()
    {
        StartCoroutine(MoveInDirectionCoroutine(GetRandomMovementVector(), _nodeDropMoveSpeed, _nodeDropMoveTime));
    }

    public void MoveInDirection(Vector2 movementDirection)
    {
        Vector2 movementVectorNormalized = (movementDirection - (Vector2)transform.position).normalized;

        StartCoroutine(MoveInDirectionCoroutine(movementVectorNormalized, _agentDropMoveSpeed, _agentDropMoveTime));
    }

    private IEnumerator MoveInDirectionCoroutine(Vector2 movementVectorNormalized, float dropMoveSpeed, float moveDuration)
    {
        _rigidBody.velocity = movementVectorNormalized * dropMoveSpeed;

        yield return new WaitForSeconds(moveDuration);

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
        float randomX = transform.position.x + UnityEngine.Random.Range(-1f, 1f);
        float randomY = transform.position.y + UnityEngine.Random.Range(-1f, 1f);

        Vector2 RandomMovementVectorNormalized = (new Vector2(randomX, randomY) - (Vector2)transform.position).normalized;

        return RandomMovementVectorNormalized;
    }

}
