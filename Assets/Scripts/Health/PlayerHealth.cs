using UnityEngine;
using UnityEngine.UI;

namespace Health
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 10;
        [SerializeField] private Image hpBar;

        private int _currentHealth;

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            hpBar.fillAmount = _currentHealth / (float)maxHealth;
            Debug.Log($"Player took {damage} damage. Current HP: {_currentHealth}");

            if (_currentHealth <= 0)
            {
                Debug.Log("Player died");
            }
        }
    }
}