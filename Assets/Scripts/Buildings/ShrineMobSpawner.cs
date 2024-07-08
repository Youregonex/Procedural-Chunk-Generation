using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class ShrineMobSpawner : InteractableBuilding
{
    private const string SPAWN = "SPAWN";
    private const string DESTROY = "DESTROY";

    [Header("Config")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private int _numberOfWaves;
    [SerializeField] private int _numberOfEnemiesPerWaveMin;
    [SerializeField] private int _numberOfEnemiesPerWaveMax;
    [SerializeField] private Vector2 _enemySpawnArea;
    [SerializeField] private bool _addExtraEnemiesEveryWave;

    [Header("Debug Fields")]
    [SerializeField] private List<AgentHealthSystem> _aliveEnemies = new List<AgentHealthSystem>();
    [SerializeField] private int _currentWave = 1;
    [SerializeField] private Transform _currentTarget;
    [SerializeField] private bool _isActive = false;
    [SerializeField] private bool _wavesComplited = false;
    [SerializeField] private Animator _animator;

    protected override void Awake()
    {
        base.Awake();

        _animator = _spriteRenderer.GetComponent<Animator>();
    }

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
        _animator.SetTrigger(DESTROY);

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
        _animator.SetTrigger(SPAWN);

        int enemiesPerWave = UnityEngine.Random.Range(_numberOfEnemiesPerWaveMin, _numberOfEnemiesPerWaveMax);

        if (_addExtraEnemiesEveryWave)
            enemiesPerWave += UnityEngine.Random.Range(0, _currentWave);

        List<Vector2> validPositions = new List<Vector2>();

        for (int i = 0; i < enemiesPerWave; i++)
        {
            bool positionValid = false;
            int pickPositionTries = 0;
            Vector2 randomPositionAroundShrine = Vector2.zero;

            while (!positionValid)
            {
                pickPositionTries++;

                Vector2 randomOffset = new Vector2(UnityEngine.Random.Range(-_enemySpawnArea.x, _enemySpawnArea.x),
                                                   UnityEngine.Random.Range(-_enemySpawnArea.y, _enemySpawnArea.y));

                randomPositionAroundShrine = (Vector2)transform.position + randomOffset;

                positionValid = !TilePlacer.Instance.HasObstaclAtPosition(randomPositionAroundShrine);

                if (positionValid)
                    validPositions.Add(randomPositionAroundShrine);

                if (pickPositionTries > 20)
                {
                    Debug.LogError($"Couldn't find new valid position for enemy {i}");
                    randomPositionAroundShrine = validPositions[UnityEngine.Random.Range(0, validPositions.Count)];
                    break;
                }
            }

            GameObject enemyGameObject = Instantiate(_enemyPrefab, randomPositionAroundShrine, Quaternion.identity);
            AgentCoreBase agentCore = enemyGameObject.GetComponent<AgentCoreBase>();
            AgentHealthSystem agentHealthSystem = agentCore.GetAgentComponent<AgentHealthSystem>();
            BaseEnemyBehaviour baseEnemyBehaviour = agentCore.GetAgentComponent<BaseEnemyBehaviour>();
            baseEnemyBehaviour.SetCurrentTarget(enemyTarget);
            _aliveEnemies.Add(agentHealthSystem);

            agentHealthSystem.OnDeath += AgentHealthSystem_OnDeath;

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