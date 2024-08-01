using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ShrineMobSpawner : InteractableBuilding
{
    [Header("Config")]
    [SerializeField] private List<EnemyCore> _enemyPrefabList;
    [SerializeField] private EnemyCore _bossPrefab; 
    [SerializeField] private int _numberOfWaves;
    [SerializeField] private int _numberOfEnemiesPerWaveMin;
    [SerializeField] private int _numberOfEnemiesPerWaveMax;
    [SerializeField] private Vector2 _enemySpawnArea;
    [SerializeField] private bool _addExtraEnemiesEveryWave;
    [SerializeField] private bool _spawnBoss;

    [Header("Debug Fields")]
    [SerializeField] private List<AgentHealthSystem> _aliveEnemies = new List<AgentHealthSystem>();
    [SerializeField] private int _currentWave = 1;
    [SerializeField] private Transform _currentTarget;
    [SerializeField] private bool _isActive = false;
    [SerializeField] private bool _wavesComplited = false;
    [SerializeField] private bool _bossSpawned = false;


    public override void Interact(GameObject initiator)
    {
        if (_isActive)
            return;

        _isActive = true;
        _currentTarget = initiator.transform.root;
        StartCoroutine(SpawnEnemies(_currentTarget));
    }

    public override void StopInteraction()
    {
        if (!_wavesComplited)
            return;

        float destructionDelay = 1f;

        StartCoroutine(DestroyShrine(destructionDelay));
    }

    private IEnumerator DestroyShrine(float destructionDelay)
    {
        yield return new WaitForSeconds(destructionDelay);

        Destroy(gameObject);
    }

    private void AllWaveEnemiesDied()
    {
        _currentWave++;

        if (_currentWave > _numberOfWaves)
        {
            _wavesComplited = true;
            StopInteraction();
        }
        else
            StartCoroutine(SpawnEnemies(_currentTarget));
    }

    private IEnumerator SpawnEnemies(Transform enemyTarget = null)
    {

        int enemiesPerWave = Random.Range(_numberOfEnemiesPerWaveMin, _numberOfEnemiesPerWaveMax);

        if (_addExtraEnemiesEveryWave)
            enemiesPerWave += Random.Range(0, _currentWave);

        List<Vector2> validPositions = new List<Vector2>();

        for (int i = 0; i < enemiesPerWave; i++)
        {
            bool positionValid = false;
            int pickPositionTries = 0;
            int maxPositionPickTries = 20;
            Vector2 randomPositionAroundShrine = Vector2.zero;

            while (!positionValid)
            {
                pickPositionTries++;

                Vector2 randomOffset = new Vector2(Random.Range(-_enemySpawnArea.x, _enemySpawnArea.x),
                                                   Random.Range(-_enemySpawnArea.y, _enemySpawnArea.y));

                randomPositionAroundShrine = (Vector2)transform.position + randomOffset;

                positionValid = !TilePlacer.Instance.HasObstaclAtPosition(randomPositionAroundShrine);

                if (positionValid)
                    validPositions.Add(randomPositionAroundShrine);

                if (pickPositionTries > maxPositionPickTries)
                {
                    Debug.LogError($"Couldn't find new valid position for enemy {i}");
                    randomPositionAroundShrine = validPositions[Random.Range(0, validPositions.Count)];
                    break;
                }
            }

            EnemyCore enemyCore;

            if (_currentWave == _numberOfWaves && _spawnBoss && !_bossSpawned)
            {
                enemyCore = Instantiate(_bossPrefab, randomPositionAroundShrine, Quaternion.identity);
                _bossSpawned = true;
            }
            else
            {
                int randomEnemy = Random.Range(0, _enemyPrefabList.Count);
                enemyCore = Instantiate(_enemyPrefabList[randomEnemy], randomPositionAroundShrine, Quaternion.identity);
            }

            AgentHealthSystem agentHealthSystem = enemyCore.GetAgentComponent<AgentHealthSystem>();
            agentHealthSystem.OnDeath += AgentHealthSystem_OnDeath;

            BaseEnemyBehaviour baseEnemyBehaviour = enemyCore.GetAgentComponent<BaseEnemyBehaviour>();
            baseEnemyBehaviour.SetCurrentTarget(enemyTarget);

            _aliveEnemies.Add(agentHealthSystem);

            yield return new WaitForEndOfFrame();
        }
    }

    private void AgentHealthSystem_OnDeath(AgentHealthSystem agentHealthSystem)
    {
        agentHealthSystem.OnDeath -= AgentHealthSystem_OnDeath;
        _aliveEnemies.Remove(agentHealthSystem);

        if (_aliveEnemies.Count == 0)
            AllWaveEnemiesDied();
    }
}