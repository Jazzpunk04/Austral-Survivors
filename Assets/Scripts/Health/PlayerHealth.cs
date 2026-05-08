using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Health
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 10;
        [SerializeField] private Image hpBar;
        [SerializeField] private PlayerStats playerStats;

        private int _currentHealth;

        private void Awake()
        {
            if (playerStats == null)
            {
                playerStats = GetComponent<PlayerStats>();
            }

            if (playerStats != null)
            {
                maxHealth = playerStats.MaxHealth;
            }

            _currentHealth = maxHealth;
            UpdateHpBar();
        }

        private void OnEnable()
        {
            if (playerStats == null)
            {
                playerStats = GetComponent<PlayerStats>();
            }

            if (playerStats != null)
            {
                playerStats.StatsChanged += HandleStatsChanged;
            }
        }

        private void OnDisable()
        {
            if (playerStats != null)
            {
                playerStats.StatsChanged -= HandleStatsChanged;
            }
        }

        public void TakeDamage(int damage)
        {
            TakeDamage(damage, null);
        }

        public void TakeDamage(int damage, Transform damageSource)
        {
            if (damage <= 0)
            {
                return;
            }

            _currentHealth -= damage;
            _currentHealth = Mathf.Max(0, _currentHealth);
            UpdateHpBar();

            if (_currentHealth <= 0)
            {
                Debug.Log("Player died");
            }
        }

        public void HealToFull()
        {
            _currentHealth = maxHealth;
            UpdateHpBar();
        }

        private void HandleStatsChanged(PlayerStats stats)
        {
            maxHealth = stats.MaxHealth;
            HealToFull();
        }

        private void UpdateHpBar()
        {
            if (hpBar == null)
            {
                return;
            }

            hpBar.fillAmount = _currentHealth / (float)maxHealth;
            hpBar.color = _currentHealth <= maxHealth * 0.5f ? Color.yellow : Color.green;
        }
    }
}
