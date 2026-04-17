using System.Collections.Generic;
using Health;
using UnityEngine;

namespace Combat
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private List<AttackData> equippedAttacks = new();

        private readonly List<float> _nextAttackTimes = new();

        private void Awake()
        {
            SyncCooldownSlots();
        }

        private void Update()
        {
            SyncCooldownSlots();

            for (int i = 0; i < equippedAttacks.Count; i++)
            {
                AttackData attackData = equippedAttacks[i];
                if (attackData == null || Time.time < _nextAttackTimes[i])
                {
                    continue;
                }

                TryAttack(attackData);
                _nextAttackTimes[i] = Time.time + attackData.cooldown;
            }
        }

        public void EquipAttack(AttackData attackData)
        {
            if (attackData == null || equippedAttacks.Contains(attackData))
            {
                return;
            }

            equippedAttacks.Add(attackData);
            _nextAttackTimes.Add(0f);
        }

        public void UnequipAttack(AttackData attackData)
        {
            int index = equippedAttacks.IndexOf(attackData);
            if (index < 0)
            {
                return;
            }

            equippedAttacks.RemoveAt(index);
            _nextAttackTimes.RemoveAt(index);
        }

        private bool TryAttack(AttackData attackData)
        {
            MonoBehaviour target = FindClosestTarget(attackData);
            if (target == null)
            {
                return false;
            }

            if (attackData.projectilePrefab != null)
            {
                ShootProjectile(attackData, target.transform);
                return true;
            }

            ((IDamageable)target).TakeDamage(attackData.damage, transform);
            return true;
        }

        private MonoBehaviour FindClosestTarget(AttackData attackData)
        {
            MonoBehaviour closestTarget = null;
            float closestSqrDistance = attackData.attackRange * attackData.attackRange;
            MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is not IDamageable damageable || IsOwnComponent(behaviour))
                {
                    continue;
                }

                if (!IsInTargetLayer(behaviour.gameObject, attackData))
                {
                    continue;
                }

                float sqrDistance = ((Vector2)behaviour.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (sqrDistance > closestSqrDistance)
                {
                    continue;
                }

                closestSqrDistance = sqrDistance;
                closestTarget = behaviour;
            }

            return closestTarget;
        }

        private void ShootProjectile(AttackData attackData, Transform target)
        {
            Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
            if (direction == Vector2.zero)
            {
                direction = Vector2.right;
            }

            GameObject projectileObject = Instantiate(attackData.projectilePrefab, transform.position, Quaternion.identity);
            PlayerProjectile projectile = projectileObject.GetComponent<PlayerProjectile>();
            if (projectile == null)
            {
                Destroy(projectileObject);
                Debug.LogWarning($"{attackData.projectilePrefab.name} is missing a PlayerProjectile component.", this);
                return;
            }

            projectile.Initialize(
                direction,
                attackData.damage,
                attackData.projectileSpeed,
                attackData.projectileLifetime,
                attackData.targetLayer,
                transform);
        }

        private bool IsOwnComponent(Component component)
        {
            return component.transform == transform || component.transform.IsChildOf(transform);
        }

        private static bool IsInTargetLayer(GameObject target, AttackData attackData)
        {
            return (attackData.targetLayer.value & (1 << target.layer)) != 0;
        }

        private void SyncCooldownSlots()
        {
            while (_nextAttackTimes.Count < equippedAttacks.Count)
            {
                _nextAttackTimes.Add(0f);
            }

            while (_nextAttackTimes.Count > equippedAttacks.Count)
            {
                _nextAttackTimes.RemoveAt(_nextAttackTimes.Count - 1);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (equippedAttacks == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            foreach (AttackData attackData in equippedAttacks)
            {
                if (attackData != null)
                {
                    Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
                }
            }
        }
    }
}
