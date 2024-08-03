using UnityEngine;

public class PlayerObjectInteraction : AgentMonobehaviourComponent
{
    [Header("Debug Fields")]
    [SerializeField] private PlayerCore _playerCore;
    [SerializeField] private IInteractable _currentInteractable;
    [SerializeField] private PlayerInput _playerInput;

    private void Awake()
    {
        _playerCore = transform.root.GetComponent<PlayerCore>();
    }

    private void Start()
    {
        _playerInput = _playerCore.GetAgentComponent<PlayerInput>();

        _playerInput.OnInteractKeyPressed += PlayerInput_OnInteractKeyPressed;
    }

    private void Update()
    {
        ManageInteractionColliderAngle();
    }

    private void OnDestroy()
    {
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
        if (_playerCore.IsDead)
            return;

        Vector2 aimPosition = _playerInput.GetAimPosition();

        Vector2 aimDirection = (aimPosition - (Vector2)transform.position).normalized;
        transform.right = aimDirection;

        Vector3 localScale = transform.localScale;

        if (aimDirection.x <= 0)
        {
            localScale.y = -1;
        }
        else if (aimDirection.x > 0)
        {
            localScale.y = 1;
        }

        if (aimDirection.y == 0)
            localScale.y = 1;

        transform.localScale = localScale;
    }

    private void PlayerInput_OnInteractKeyPressed()
    {
        if(_currentInteractable != null)
            _currentInteractable.Interact(gameObject);
    }
}
