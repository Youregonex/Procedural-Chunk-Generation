using UnityEngine;

public class ItemHoldPoint : AgentMonobehaviourComponent
{
    [Header("Debug Fields")]
    [SerializeField] protected EnemyCore _agentCore;
    [SerializeField] private AgentInput _agentInput;

    private Vector2 _aimPosition;
    private Vector2 _aimDirection;

    protected virtual void Awake()
    {
        _agentCore = transform.root.GetComponent<EnemyCore>();
    }

    protected virtual void Start()
    {
        _agentInput = _agentCore.GetAgentComponent<AgentInput>();
    }

    private void Update()
    {
        ManageItemHoldPointPosition();
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }

    private void ManageItemHoldPointPosition()
    {
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
