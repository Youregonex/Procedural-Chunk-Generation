using UnityEngine;

public class DemonAbilitySystem : AgentAbilitySystem
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected void Start()
    {
        float cooldown = 2f;
        float duration = .05f;
        float speed = 20f;
        DashAbility ability = new DashAbility(_agentCore, _agentCore.GetAgentComponent<AgentAnimation>(), "DASH", EAbilityType.Movement, cooldown, duration, speed);

        _abilityDictionary.Add(ability.Name, ability);
    }
}
