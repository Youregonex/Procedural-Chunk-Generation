using UnityEngine;

public class PlayerInteraction : AgentMonobehaviourComponent
{
    [Header("Debug Fields")]
    [SerializeField] private PlayerCore _playerCore;
    [SerializeField] private IInteractable _currentInteractable;
    [SerializeField] private AgentMovement _agentMovement;
    [SerializeField] private PlayerInput _playerInput;

    private void Awake()
    {
        _playerCore = transform.root.GetComponent<PlayerCore>();
    }

    private void Start()
    {
        _agentMovement = _playerCore.GetAgentComponent<AgentMovement>();
        _playerInput = _playerCore.GetAgentComponent<PlayerInput>();

        _playerInput.OnInteractKeyPressed += PlayerInput_OnInteractKeyPressed;
    }

    private void Update()
    {
        transform.localPosition = _agentMovement.LastMovementDirection / 2;
    }

    private void OnDestroy()
    {
        _playerInput.OnInteractKeyPressed -= PlayerInput_OnInteractKeyPressed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteractable interactable))
        {
            _currentInteractable = interactable;
            _currentInteractable.HighlightInteractable();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_currentInteractable == null)
            return;

        if (collision.TryGetComponent(out IInteractable interactable))
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

    private void PlayerInput_OnInteractKeyPressed()
    {
        if(_currentInteractable != null)
            _currentInteractable.Interact(gameObject);
    }
}
