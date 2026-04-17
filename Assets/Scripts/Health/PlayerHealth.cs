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
            TakeDamage(damage, null);
        }

        public void TakeDamage(int damage, Transform damageSource)
        {
            _currentHealth -= damage;
            hpBar.fillAmount = _currentHealth / (float)maxHealth;
            hpBar.color = _currentHealth <= 5 ? Color.yellow : Color.green;

            if (_currentHealth <= 0)
            {
                Debug.Log("Player died");
            }
        }
    }
}
