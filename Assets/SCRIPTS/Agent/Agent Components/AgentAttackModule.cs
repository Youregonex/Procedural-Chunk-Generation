using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(AgentInput))]
public class AgentAttackModule : AgentNetworkBehaviourComponent
{
    [Header("Config")]
    [SerializeField] protected AgentItemHoldPoint _itemHoldPointPrefab;

    [Header("Debug Fields")]
    [SerializeField] protected AgentItemHoldPoint _itemHoldPoint;
    [SerializeField] protected Weapon _currentWeapon;
    [SerializeField] protected WeaponItemDataSO _currentWeaponItemDataSO;
    [SerializeField] protected AgentInput _agentInput;

    public bool CanAttack => _currentWeapon.ReadyToAttack;

    protected WeaponFactory _weaponFactory = new();

    public override void Initialize()
    {
        GetAgentCore();

        _agentInput = AgentCore.GetAgentComponent<AgentInput>();
        _itemHoldPoint = AgentCore.GetAgentComponent<AgentItemHoldPoint>();

        _agentInput.OnMousePrimary += AgentInput_OnAgentAttackTrigger;
    }

    public override void OnNetworkDespawn()
    {
        if (_agentInput != null)
            _agentInput.OnMousePrimary -= AgentInput_OnAgentAttackTrigger;
    }

    public void UpdateItemHoldPoint()
    {
        AgentItemHoldPoint agentItemHoldPoint = transform.GetComponentInChildren<AgentItemHoldPoint>();
        agentItemHoldPoint.Initialize();
    }

    public float GetAttackCooldown()
    {
        if(_currentWeapon != null)
            return _currentWeapon.AttackCooldownCurrent;
        else
        {
            Debug.LogError("Current weapon is null!");
            return -1f;
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

    private void AgentInput_OnAgentAttackTrigger(object sender, System.EventArgs e)
    {
        Attack();
    }

    protected virtual void Attack()
    {
        if (!IsOwner)
            return;

        if (_currentWeapon == null || !CanAttack)
            return;

        _currentWeapon.Attack();
    }

    protected void ChangeWeapon(WeaponItemDataSO newWeaponItemDataSO)
    {
        if (!IsOwner || newWeaponItemDataSO == null)
            return;

        HideCurrentWeapon();
        int itemDataSOId = MultiplayerPrefabDatabase.Instance.GetIdWithItemDataSO(newWeaponItemDataSO);

        SpawnWeaponServerRpc(itemDataSOId, AgentCore.NetworkObject, NetworkObject.OwnerClientId);
    }

    [Rpc(SendTo.Server)]
    protected void SpawnWeaponServerRpc(int weaponItemDataSOId, NetworkObjectReference agentCoreNetworkObjectReference, ulong clientId)
    {
        if(agentCoreNetworkObjectReference.TryGet(out NetworkObject agentCoreNetworkObject))
        {
            if(agentCoreNetworkObject.TryGetComponent(out AgentCoreBase agentCore))
            {
                AgentItemHoldPoint agentItemHoldPoint = agentCore.GetAgentComponent<AgentItemHoldPoint>();

                WeaponItemDataSO weaponItemDataSO = MultiplayerPrefabDatabase.Instance.GetItemDataSOWithId(weaponItemDataSOId) as WeaponItemDataSO;
                Weapon weapon = Instantiate(weaponItemDataSO.WeaponPrefab, agentItemHoldPoint.transform.position, Quaternion.identity);
                weapon.NetworkObject.Spawn();
                weapon.transform.SetParent(agentItemHoldPoint.transform);

                EquipWeaponSingleClientRpc(weapon.NetworkObject, weaponItemDataSOId, RpcTarget.Single(clientId, RpcTargetUse.Temp));
                UpdateWeaponPositionAllClientRpc(weapon.NetworkObject);
            }
            else
            {
                Debug.LogError("Couldn't get AgentCoreBase out of agentCoreNetworkObject");
            }
        }
        else
        {
            Debug.LogError("Couldn't unpack agentCoreNetworkObject");
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    protected void EquipWeaponSingleClientRpc(NetworkObjectReference weaponNetworkObjectReference, int weaponItemDataSOId, RpcParams rpcParams)
    {
        if (weaponNetworkObjectReference.TryGet(out NetworkObject weaponNetworkObject) && weaponNetworkObject.TryGetComponent(out Weapon weapon))
        {
            WeaponItemDataSO weaponItemDataSO = MultiplayerPrefabDatabase.Instance.GetItemDataSOWithId(weaponItemDataSOId) as WeaponItemDataSO;
            _currentWeapon = weapon;
            _currentWeaponItemDataSO = weaponItemDataSO;
            weapon.InitializeWeapon(AgentCore, this, _itemHoldPoint);
        }
        else
        {
            Debug.LogError("Couldn't unpack weaponNetworkObject/weaponNetworkObject doesn't contain Weapon script");
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    protected void UpdateWeaponPositionAllClientRpc(NetworkObjectReference weaponNetworkObjectReference)
    {
        if(weaponNetworkObjectReference.TryGet(out NetworkObject weaponNetworkObject) && weaponNetworkObject.TryGetComponent(out Weapon weapon))
        {
            weapon.transform.localPosition = Vector2.zero;
            weapon.transform.localRotation = Quaternion.identity;
            weapon.transform.localScale = Vector2.one;
        }
        else
        {
            Debug.LogError("Couldn't unpack weaponNetworkObject/weaponNetworkObject doesn't contain Weapon script");
        }
    }

    protected void HideCurrentWeapon()
    {
        if (_currentWeapon == null)
            return;

        DespawnWeaponServerRpc(_currentWeapon.NetworkObject, NetworkObject.OwnerClientId);

        _currentWeapon = null;
        _currentWeaponItemDataSO = null;
    }

    [Rpc(SendTo.Server)]
    protected void DespawnWeaponServerRpc(NetworkObjectReference weaponNetworkObjectReference, ulong clientId)
    {
        if(weaponNetworkObjectReference.TryGet(out NetworkObject weaponNetworkObject) && weaponNetworkObject.TryGetComponent(out Weapon weapon))
        {
            weaponNetworkObject.Despawn();
            UnEquipWeaponSingleClientRpc(RpcTarget.Single(clientId, RpcTargetUse.Temp));
        }
        else
        {
            Debug.LogError("Couldn't unpack NetworkObjectReference!");
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void UnEquipWeaponSingleClientRpc(RpcParams rpcParams)
    {
        _currentWeapon = null;
        _currentWeaponItemDataSO = null;
    }
}