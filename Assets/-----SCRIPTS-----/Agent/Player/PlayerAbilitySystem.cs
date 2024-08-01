using UnityEngine;

public class PlayerAbilitySystem : AgentAbilitySystem
{
    [Header("Config")]
    [SerializeField] private AgentMovement _agentMovement;

    protected override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            CastAbility(_abilityDictionary["DASH"], _agentMovement.LastMovementDirection);
        }
    }
}
