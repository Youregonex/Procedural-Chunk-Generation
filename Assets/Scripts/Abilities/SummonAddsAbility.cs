using System.Collections.Generic;
using UnityEngine;

public class SummonAddsAbility : Ability
{
    private int _maxAddsCount;
    private int _addsSummonPerCast;
    private List<AgentCoreBase> _addsPrefabList;

    private List<AgentHealthSystem> _currentAddsList;

    public SummonAddsAbility(AgentCoreBase caster,
                             AgentAnimation casterAnimator,
                             string name,
                             EAbilityType abilityType,
                             float cooldown,
                             GameObject abilityParticles,
                             int addsSummonPerCast,
                             int maxAddsCount,
                             List<AgentCoreBase> addsPrefabList) : base(caster, casterAnimator, name, abilityType, cooldown, abilityParticles)
    {
        _maxAddsCount = maxAddsCount;
        _addsSummonPerCast = addsSummonPerCast;
        _addsPrefabList = addsPrefabList;

        _currentAddsList = new List<AgentHealthSystem>();
    }

    public override void StartCast(Vector2 targetPosition)
    {
        base.StartCast(targetPosition);

        SummonAdds();
    }

    private void SummonAdds()
    {
        int addsSummonAmount = _addsSummonPerCast;

        if (_addsSummonPerCast + _currentAddsList.Count > _maxAddsCount)
            addsSummonAmount = _maxAddsCount - _currentAddsList.Count;

        List<Vector2> validPositions = new List<Vector2>();

        for (int i = 0; i < addsSummonAmount; i++)
        {
            bool positionValid = false;
            int pickPositionTries = 0;
            int maxPositionPickTries = 20;
            Vector2 randomPositionAroundCaster = Vector2.zero;

            while (!positionValid)
            {
                pickPositionTries++;

                Vector2 enemySpawnArea = new Vector2(3f, 3f);
                Vector2 randomOffset = new Vector2(Random.Range(-enemySpawnArea.x, enemySpawnArea.x),
                                                   Random.Range(-enemySpawnArea.y, enemySpawnArea.y));

                randomPositionAroundCaster = (Vector2)Caster.transform.position + randomOffset;

                positionValid = !TilePlacer.Instance.HasObstaclAtPosition(randomPositionAroundCaster);

                if (positionValid)
                    validPositions.Add(randomPositionAroundCaster);

                if (pickPositionTries > maxPositionPickTries)
                {
                    Debug.LogError($"Couldn't find new valid position for enemy {i}");
                    randomPositionAroundCaster = validPositions[Random.Range(0, validPositions.Count)];
                    break;
                }
            }

            int randomAddIndex = Random.Range(0, _addsPrefabList.Count);
            AgentHealthSystem addHealthSystem = GameObject.Instantiate(_addsPrefabList[randomAddIndex],
                                                                     randomPositionAroundCaster,
                                                                     Quaternion.identity).
                                                                     GetComponent<AgentHealthSystem>();
            addHealthSystem.OnDeath += AgentHealthSystem_OnDeath;
            _currentAddsList.Add(addHealthSystem);
        }

        StopCast();
    }

    private void AgentHealthSystem_OnDeath(AgentHealthSystem addHealthSystem)
    {
        addHealthSystem.OnDeath -= AgentHealthSystem_OnDeath;
        _currentAddsList.Remove(addHealthSystem);
    }
}
