using Experience;
using UnityEngine;
using UnityEngine.UI;

namespace Health
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private Image hpBar;

        private int _maxHealth = 10;
        private int _experienceReward = 1;
        private int _currentHealth;
        private bool _isDead;

        public void SetUp(int maxHealth, int experienceReward)
        {
            _maxHealth = maxHealth;
            _experienceReward = experienceReward;
            _currentHealth = _maxHealth;
            _isDead = false;
        }

        public void TakeDamage(int damage)
        {
            TakeDamage(damage, null);
        }

        public void TakeDamage(int damage, Transform damageSource)
        {
            if (_isDead)
            {
                return;
            }

            _currentHealth -= damage;
            hpBar.fillAmount = _currentHealth / (float)_maxHealth;
            hpBar.color = _currentHealth <= 5 ? Color.yellow : Color.green;

            if (_currentHealth <= 0)
            {
                _isDead = true;
                AwardExperience(damageSource);
                Destroy(gameObject);
            }
        }

        private void AwardExperience(Transform damageSource)
        {
            if (damageSource == null)
            {
                return;
            }

            PlayerExperience playerExperience = damageSource.GetComponentInParent<PlayerExperience>();
            if (playerExperience != null)
            {
                playerExperience.AddExperience(_experienceReward);
            }
        }
    }
}
