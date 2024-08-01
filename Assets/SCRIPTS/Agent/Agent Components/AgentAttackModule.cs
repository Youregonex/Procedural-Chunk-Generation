using UnityEngine;

[RequireComponent(typeof(AgentInput))]
public class AgentAttackModule : AgentMonobehaviourComponent
{
    [Header("Config")]
    [SerializeField] protected ItemHoldPoint _itemHoldPoint;

    [Header("Debug Fields")]
    [SerializeField, HideInInspector] protected Weapon _currentWeapon;
    [SerializeField, HideInInspector] protected WeaponItemDataSO _currentWeaponItemDataSO;
    [SerializeField, HideInInspector] protected Tool _currentTool;
    [SerializeField, HideInInspector] protected ToolItemDataSO _currentToolItemDataSO;
    [SerializeField, HideInInspector] protected AgentInput _agentInput;
    [SerializeField, HideInInspector] protected EnemyCore _agentCore;


    public bool CanAttack
    {
        get
        {
            if (_currentWeapon != null)
                return _currentWeapon.ReadyToAttack;
            if (_currentTool != null)
                return _currentTool.CanSwing;

            return false;
        }
    }

    protected WeaponFactory _weaponFactory = new WeaponFactory();

    protected virtual void Awake()
    {
        _agentCore = GetComponent<EnemyCore>();
    }

    protected virtual void Start()
    {
        _agentInput = _agentCore.GetAgentComponent<AgentInput>();

        _agentInput.OnAgentAttackTriggered += AgentInput_OnAgentAttackTrigger;
    }

    protected virtual void OnDestroy()
    {
        if(_agentInput != null)
            _agentInput.OnAgentAttackTriggered -= AgentInput_OnAgentAttackTrigger;
    }

    public float GetAttackCooldown()
    {
        if(_currentWeapon != null)
            return _currentWeapon.AttackCooldownCurrent;

        return _currentTool.AttackCooldownCurrent;
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }

    private void AgentInput_OnAgentAttackTrigger(object sender, System.EventArgs e)
    {
        Attack();
    }

    protected virtual void Attack()
    {
        if (_currentWeapon == null || !CanAttack)
            return;

        if (_currentWeapon != null)
            _currentWeapon.Attack();

        if (_currentTool != null)
            _currentTool.Attack();
    }

    protected void ChangeWeapon(WeaponItemDataSO newWeaponItemDataSO)
    {
        if (newWeaponItemDataSO == _currentWeaponItemDataSO)
            return;

        HideCurrentWeapon();
        HideCurrentTool();

        _currentWeaponItemDataSO = newWeaponItemDataSO;

        _currentWeapon = _weaponFactory.CreateWeapon(newWeaponItemDataSO);
        _currentWeapon.transform.SetParent(_itemHoldPoint.transform);

        _currentWeapon.transform.localPosition = Vector3.zero;
        _currentWeapon.transform.localRotation = Quaternion.identity;
        _currentWeapon.transform.localScale = Vector3.one;

        _currentWeapon.SetupWeapon(_agentCore, this, _itemHoldPoint);
    }

    protected void ChangeTool(ToolItemDataSO newToolItemDataSO)
    {
        if (newToolItemDataSO == _currentToolItemDataSO)
            return;

        HideCurrentWeapon();
        HideCurrentTool();

        _currentToolItemDataSO = newToolItemDataSO;

        _currentTool = _weaponFactory.CreateTool(newToolItemDataSO);
        _currentTool.transform.SetParent(_itemHoldPoint.transform);

        _currentTool.transform.localPosition = Vector3.zero;
        _currentTool.transform.localRotation = Quaternion.identity;
        _currentTool.transform.localScale = Vector3.one;

        _currentTool.SetUpTool(this);
    }

    protected void HideCurrentWeapon()
    {
        if (_currentWeapon == null)
            return;

        Destroy(_currentWeapon.gameObject);

        _currentWeapon = null;
        _currentWeaponItemDataSO = null;

    }

    protected void HideCurrentTool()
    {
        if (_currentTool == null)
            return;

        Destroy(_currentTool.gameObject);

        _currentTool = null;
        _currentToolItemDataSO = null;
    }

}