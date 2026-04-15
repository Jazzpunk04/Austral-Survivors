using Enemies;
using Health;
using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(Enemy))]
    public class EnemyAttack : MonoBehaviour
    {
        private Enemy enemy;

        private float _nextAttackTime;

        private void Awake()
        {
            if (enemy == null)
            {
                enemy = GetComponent<Enemy>();
            }
        }

        public bool IsTargetInRange()
        {
            AttackData attackData = GetAttackData();

            if (attackData == null || enemy == null || enemy.Target == null)
            {
                return false;
            }

            float distance = Vector2.Distance(transform.position, enemy.Target.position);
            return distance <= attackData.attackRange;
        }

        public bool TryAttack()
        {
            AttackData attackData = GetAttackData();

            if (attackData == null || enemy == null || enemy.Target == null)
            {
                return false;
            }

            if (!IsTargetInRange() || Time.time < _nextAttackTime)
            {
                return false;
            }

            IDamageable damageable = enemy.Target.GetComponentInParent<IDamageable>();
            if (damageable == null)
            {
                return false;
            }

            damageable.TakeDamage(attackData.damage);
            enemy.ShowAttackSprite(attackData);
            _nextAttackTime = Time.time + attackData.cooldown;
            return true;
        }

        private AttackData GetAttackData()
        {
            return enemy != null && enemy.Data != null ? enemy.Data.attackData : null;
        }
    }
}
