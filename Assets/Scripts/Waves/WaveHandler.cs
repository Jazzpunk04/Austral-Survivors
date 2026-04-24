using System.Collections;
using System.Collections.Generic;
using Controllers;
using Enemies;
using Health;
using UnityEngine;

namespace Waves
{
    public class WaveHandler : MonoBehaviour
    {
        [Header("Wave Setup")]
        [SerializeField] private List<WaveData> waves = new();
        [SerializeField] private float delayBetweenEnemies = 1f;
        [SerializeField] private float delayBetweenWaves = 3f;

        [Header("Spawning")]
        [SerializeField] private Transform playerTarget;
        [SerializeField] private Transform spawnCenter;
        [SerializeField] private float spawnRadius = 6f;
        [SerializeField] private List<Transform> spawnPoints = new();

        private readonly HashSet<EnemyHealth> _aliveEnemies = new();

        private void Start()
        {
            ResolvePlayerTarget();
            StartCoroutine(RunWaveLoop());
        }

        private IEnumerator RunWaveLoop()
        {
            for (int i = 0; i < waves.Count; i++)
            {
                StartCoroutine(SpawnWave(waves[i]));

                yield return new WaitUntil(() => _aliveEnemies.Count == 0);

                bool isLastWave = i == waves.Count - 1;
                if (!isLastWave)
                {
                    yield return new WaitForSeconds(delayBetweenWaves);
                }
            }
        }

        private IEnumerator SpawnWave(WaveData wave)
        {
            if (wave == null || wave.enemiesToSpawn <= 0)
            {
                yield break;
            }

            for (int i = 0; i < wave.enemiesToSpawn; i++)
            {
                GameObject enemyPrefab = PickEnemyPrefab(wave);
                if (enemyPrefab == null)
                {
                    continue;
                }

                Vector3 spawnPosition = GetSpawnPosition();
                GameObject instance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                AssignTarget(instance);

                EnemyHealth enemyHealth = instance.GetComponent<EnemyHealth>();
                if (enemyHealth == null)
                {
                    enemyHealth = instance.GetComponentInChildren<EnemyHealth>();
                }

                if (enemyHealth != null)
                {
                    _aliveEnemies.Add(enemyHealth);
                    enemyHealth.Died += OnEnemyDied;
                }
                
                yield return new WaitForSeconds(delayBetweenEnemies);
            }
        }

        private GameObject PickEnemyPrefab(WaveData wave)
        {
            float totalChance = 0f;

            for (int i = 0; i < wave.enemies.Count; i++)
            {
                WaveData.EnemySpawnOption option = wave.enemies[i];
                if (option == null || option.enemyPrefab == null || option.chance <= 0f)
                {
                    continue;
                }

                totalChance += option.chance;
            }

            if (totalChance <= 0f)
            {
                return null;
            }

            float pick = Random.Range(0f, totalChance);
            float accumulator = 0f;

            for (int i = 0; i < wave.enemies.Count; i++)
            {
                WaveData.EnemySpawnOption option = wave.enemies[i];
                if (option == null || option.enemyPrefab == null || option.chance <= 0f)
                {
                    continue;
                }

                accumulator += option.chance;
                if (pick <= accumulator)
                {
                    return option.enemyPrefab;
                }
            }

            return null;
        }

        private Vector3 GetSpawnPosition()
        {
            if (spawnPoints.Count > 0)
            {
                int randomIndex = Random.Range(0, spawnPoints.Count);
                Transform point = spawnPoints[randomIndex];
                if (point != null)
                {
                    return point.position;
                }
            }

            Vector3 center = spawnCenter != null ? spawnCenter.position : transform.position;
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            return center + new Vector3(randomOffset.x, randomOffset.y, 0f);
        }

        private void ResolvePlayerTarget()
        {
            if (playerTarget != null)
            {
                return;
            }

            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                playerTarget = player.transform;
            }
        }

        private void AssignTarget(GameObject enemyInstance)
        {
            if (enemyInstance == null || playerTarget == null)
            {
                return;
            }

            Enemy enemy = enemyInstance.GetComponent<Enemy>();
            if (enemy == null)
            {
                enemy = enemyInstance.GetComponentInChildren<Enemy>();
            }

            if (enemy != null)
            {
                enemy.SetTarget(playerTarget);
            }
        }

        private void OnEnemyDied(EnemyHealth enemyHealth)
        {
            if (enemyHealth == null)
            {
                return;
            }

            enemyHealth.Died -= OnEnemyDied;
            _aliveEnemies.Remove(enemyHealth);
        }

        private void OnDisable()
        {
            foreach (EnemyHealth enemyHealth in _aliveEnemies)
            {
                if (enemyHealth != null)
                {
                    enemyHealth.Died -= OnEnemyDied;
                }
            }

            _aliveEnemies.Clear();
        }
    }
}
