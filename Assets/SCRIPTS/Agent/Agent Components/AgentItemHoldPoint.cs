using UnityEngine;

public class AgentItemHoldPoint : AgentNetworkBehaviourComponent
{
    [Header("Debug Fields")]
    [SerializeField] protected AgentInput _agentInput;

    protected bool _isInitialized = false;

    private Vector2 _aimPosition;
    private Vector2 _aimDirection;

    public override void Initialize()
    {
        GetAgentCore();
        _agentInput = AgentCore.GetAgentComponent<AgentInput>();

        _isInitialized = true;
    }

    public override void OnNetworkSpawn() {}

    private void Update()
    {
        if (!_isInitialized)
            return;

        ManageItemHoldPointRotation();
    }

    public override void DisableComponent()
    {
        enabled = false;
    }

    public override void EnableComponent()
    {
        enabled = true;
    }

    private void ManageItemHoldPointRotation()
    {
        if (AgentCore.IsDead)
            return;

        _aimPosition = _agentInput.GetAimPosition();

        _aimDirection = (_aimPosition - (Vector2)transform.position).normalized;
        transform.right = _aimDirection;

        Vector3 localScale = transform.localScale;

        if (_aimDirection.x <= 0)
        {
            localScale.y = -1;
        }
        else if (_aimDirection.x > 0)
        {
            localScale.y = 1;
        }

        if (_aimDirection.y == 0)
            localScale.y = 1;

        transform.localScale = localScale;
    }
}
