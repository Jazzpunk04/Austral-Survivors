using System;
using Experience;
using UnityEngine;

namespace Data
{
    [RequireComponent(typeof(PlayerExperience))]
    public class PlayerStats : MonoBehaviour
    {
        [Header("Base Stats")]
        [SerializeField, Min(1)] private int baseMaxHealth = 10;
        [SerializeField, Min(0f)] private float baseDamageMultiplier = 1f;
        [SerializeField, Min(0f)] private float baseMoveSpeedMultiplier = 1f;

        [Header("Level Growth")]
        [SerializeField, Min(0)] private int maxHealthPerLevel = 2;
        [SerializeField, Min(0f)] private float damageMultiplierPerLevel = 0.1f;
        [SerializeField, Min(0f)] private float moveSpeedMultiplierPerLevel = 0.05f;

        private PlayerExperience _playerExperience;
        private int _level = 1;

        public int Level => _level;
        public int MaxHealth => baseMaxHealth + ((_level - 1) * maxHealthPerLevel);
        public float DamageMultiplier => baseDamageMultiplier + ((_level - 1) * damageMultiplierPerLevel);
        public float MoveSpeedMultiplier => baseMoveSpeedMultiplier + ((_level - 1) * moveSpeedMultiplierPerLevel);

        public event Action<PlayerStats> StatsChanged;

        private void Awake()
        {
            _playerExperience = GetComponent<PlayerExperience>();
            ApplyLevel(_playerExperience.CurrentLevel);
        }

        private void OnEnable()
        {
            if (_playerExperience == null)
            {
                _playerExperience = GetComponent<PlayerExperience>();
            }

            _playerExperience.LeveledUp += HandleLeveledUp;
        }

        private void OnDisable()
        {
            if (_playerExperience != null)
            {
                _playerExperience.LeveledUp -= HandleLeveledUp;
            }
        }

        private void HandleLeveledUp(int newLevel)
        {
            ApplyLevel(newLevel);
            StatsChanged?.Invoke(this);
        }

        private void ApplyLevel(int newLevel)
        {
            _level = Mathf.Max(1, newLevel);
        }
    }
}
