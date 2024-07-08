using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Debug Fields")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileRange;
    [SerializeField] private DamageStruct _projectileDamage;
    [SerializeField] private Vector2 _movementVector;
    [SerializeField] private Vector2 _startPosition;
    [SerializeField] private Rigidbody2D _rigidBody;


    private void Update()
    {
        if (Vector2.Distance(transform.position, _startPosition) > _projectileRange)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<AgentHitbox>(out AgentHitbox agentHitbox))
        {
            agentHitbox.TakeDamage(_projectileDamage);
        }

        Destroy(gameObject);
    }

    public void SetupProjectile(float projectileSpeed, float projectileRange, DamageStruct projectileDamage)
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        _startPosition = transform.position;
        _projectileSpeed = projectileSpeed;
        _projectileRange = projectileRange;
        _projectileDamage = projectileDamage;

        _rigidBody.velocity = transform.right * _projectileSpeed;
    }
}
