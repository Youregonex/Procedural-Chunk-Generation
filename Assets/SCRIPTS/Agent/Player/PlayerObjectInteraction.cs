using UnityEngine;

public class PlayerObjectInteraction : AgentNetworkBehaviourComponent
{
    [Header("Debug Fields")]
    [SerializeField] private IInteractable _currentInteractable;
    [SerializeField] private AgentMovement _playerMovement;
    [SerializeField] private PlayerInput _playerInput;

    public override void Initialize()
    {
        GetAgentCore();
        _playerMovement = AgentCore.GetAgentComponent<AgentMovement>();

        _playerInput = AgentCore.GetAgentComponent<PlayerInput>();
        _playerInput.OnInteractKeyPressed += PlayerInput_OnInteractKeyPressed;
    }

    private void Update()
    {
        ManageInteractionColliderAngle();
    }

    public override void OnNetworkDespawn()
    {
        if (_playerInput != null)
            _playerInput.OnInteractKeyPressed -= PlayerInput_OnInteractKeyPressed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.root.TryGetComponent(out IInteractable interactable))
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.UnhighlightInteractable();
                _currentInteractable.StopInteraction();
            }

            _currentInteractable = interactable;
            _currentInteractable.HighlightInteractable();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_currentInteractable == null)
            return;

        if (collision.transform.root.TryGetComponent(out IInteractable interactable))
        {
            _currentInteractable.UnhighlightInteractable();
            _currentInteractable.StopInteraction();
            _currentInteractable = null;
        }
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }

    private void ManageInteractionColliderAngle()
    {
        if (AgentCore.IsDead)
            return;

        Vector2 aimDirection = _playerMovement.LastMovementDirection;

        transform.right = aimDirection;
    }

    private void PlayerInput_OnInteractKeyPressed()
    {
        if(_currentInteractable != null)
            _currentInteractable.Interact(gameObject);
    }
}
