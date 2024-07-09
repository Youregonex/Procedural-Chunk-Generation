using UnityEngine;

public class PlayerAbilitySystem : AgentAbilitySystem
{
    [SerializeField] private AgentMovement _agentMovement;

    protected override void Awake()
    {
        base.Awake();
    }

    protected void Start()
    {
        _agentMovement = _agentCore.GetAgentComponent<AgentMovement>();

        float cooldown = 2f;
        float duration = .05f;
        float speed = 20f;
        DashAbility ability = new DashAbility(_agentCore, _agentCore.GetAgentComponent<AgentAnimation>(), "DASH", EAbilityType.Movement, cooldown, duration, speed);

        _abilityDictionary.Add(ability.Name, ability);
    }

    protected override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            CastAbility(_abilityDictionary["DASH"], _agentMovement.LastMovementDirection);
        }
    }
}
