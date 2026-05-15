using System;
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

        public event Action<Vector2, float> AttackPerformed;

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

            Vector2 attackDirection = ((Vector2)enemy.Target.position - (Vector2)transform.position).normalized;
            if (attackDirection == Vector2.zero)
            {
                attackDirection = Vector2.down;
            }

            damageable.TakeDamage(attackData.damage);
            enemy.ShowAttackSprite(attackData);
            AttackPerformed?.Invoke(attackDirection, attackData.attackSpriteDuration);
            _nextAttackTime = Time.time + attackData.cooldown;
            return true;
        }

        private AttackData GetAttackData()
        {
            return enemy != null && enemy.Data != null ? enemy.Data.attackData : null;
        }
    }
}
