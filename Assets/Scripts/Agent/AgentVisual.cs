using UnityEngine;

public class AgentVisual : MonoBehaviour
{
    [SerializeField] private AgentMovement _agentMovement;
    [SerializeField] private AgentInput _agentInput;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ManageWeaponSpriteFlip();
    }

    private void ManageWeaponlessSpriteFlip()
    {
        if (_agentMovement.GetCurrentDirection().x != 0 && _agentMovement.GetCurrentDirection().x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else
        {
            _spriteRenderer.flipX = true;
        }
    }

    private void ManageWeaponSpriteFlip()
    {
        Vector2 aimPosition = _agentInput.GetAimPosition();

        Vector2 aimDirection = (aimPosition - (Vector2)transform.position).normalized;

        if (aimDirection.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (aimDirection.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
    }
}
