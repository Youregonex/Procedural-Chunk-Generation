using UnityEngine;

public class WeaponHoldPoint : MonoBehaviour
{
    [Header("Debug Fields")]
    [SerializeField] private AgentCoreBase _agentCore;
    [SerializeField] private AgentInput _agentInput;
    [SerializeField] private SpriteRenderer _weaponSpriteRenderer;

    private void Awake()
    {
        _agentCore = transform.root.GetComponent<AgentCoreBase>();
    }

    private void Start()
    {
        _agentInput = _agentCore.GetAgentComponent<AgentInput>();
    }

    private void Update()
    {
        ManageWeaponHoldPointPosition();
    }

    private void ManageWeaponHoldPointPosition()
    {
        Vector2 aimPosition = _agentInput.GetAimPosition();

        Vector2 aimDirection = (aimPosition - (Vector2)transform.position).normalized;
        transform.right = aimDirection;

        Vector3 localScale = transform.localScale;
        if (aimDirection.x < 0)
        {
            localScale.y = -1;
        }
        else if (aimDirection.x > 0)
        {
            localScale.y = 1;
        }

        transform.localScale = localScale;
    }
}
