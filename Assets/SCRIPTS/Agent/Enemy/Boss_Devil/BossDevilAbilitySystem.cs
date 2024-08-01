using UnityEngine;

public class BossDevilAbilitySystem : AgentAbilitySystem
{
    [Header("Config")]
    [SerializeField] private BossDevilBehaviour _bossDevilBehaviour;

    private const string JUMP_ATTACK = "JUMP_ATTACK";

    protected override void BuildAbilityCallbacks()
    {
        AddAbilityCallback(JUMP_ATTACK, (targetTransform) =>
        {
            _bossDevilBehaviour.BulletShooter.SpawnProjctiles(targetTransform);
        });
    }
}
