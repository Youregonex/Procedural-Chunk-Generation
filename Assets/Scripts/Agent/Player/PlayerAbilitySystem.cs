using UnityEngine;

public class PlayerAbilitySystem : AgentAbilitySystem
{
    private AgentMovement _agentMovement;

    protected override void Awake()
    {
        base.Awake();

        DashAbility ability = new DashAbility(_agentCore, _agentCore.GetAgentComponent<AgentAnimation>(), "DASH", EAbilityType.Movement, 5f, 3f, 30f);

        _abilityList.Add(ability);
    }

    protected void Start()
    {
        _agentMovement = _agentCore.GetAgentComponent<AgentMovement>();
    }

    protected override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            CastAbility(_abilityList[0], _agentMovement.LastMovementDirection);
        }
    }
}
