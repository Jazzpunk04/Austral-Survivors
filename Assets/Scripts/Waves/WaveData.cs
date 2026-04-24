using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waves
{
    [CreateAssetMenu(fileName = "NewWaveData", menuName = "Game/Waves/Wave Data")]
    public class WaveData : ScriptableObject
    {
        [Serializable]
        public class EnemySpawnOption
        {
            [Min(0f)] public float chance = 1f;
            public GameObject enemyPrefab;
        }

        [Min(0)] public int enemiesToSpawn = 5;
        public List<EnemySpawnOption> enemies = new();
    }
}
