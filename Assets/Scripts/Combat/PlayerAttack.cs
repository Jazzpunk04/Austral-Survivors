using System;
using System.Collections.Generic;
using Data;
using Health;
using UnityEngine;

namespace Combat
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private List<AttackData> equippedAttacks = new();
        [SerializeField] private PlayerStats playerStats;

        private readonly List<float> _nextAttackTimes = new();
        private readonly List<int> _attackLevels = new();

        public event Action<Vector2, float> AttackPerformed;

        private void Awake()
        {
            if (playerStats == null)
            {
                playerStats = GetComponent<PlayerStats>();
            }

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
                _nextAttackTimes[i] = Time.time + GetAttackCooldown(attackData);
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
            _attackLevels.Add(1);
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
            _attackLevels.RemoveAt(index);
        }

        public void UpgradeAttack(AttackData attackData)
        {
            int index = equippedAttacks.IndexOf(attackData);
            if (index < 0)
            {
                EquipAttack(attackData);
                return;
            }

            _attackLevels[index]++;
        }

        public bool IsAttackEquipped(AttackData attackData)
        {
            return equippedAttacks.Contains(attackData);
        }

        public int GetAttackLevel(AttackData attackData)
        {
            int index = equippedAttacks.IndexOf(attackData);
            return index >= 0 && index < _attackLevels.Count ? _attackLevels[index] : 0;
        }

        public int GetAttackDamage(AttackData attackData)
        {
            return GetAttackDamageAtLevel(attackData, Mathf.Max(1, GetAttackLevel(attackData)));
        }

        public int GetAttackDamageAtLevel(AttackData attackData, int attackLevel)
        {
            if (attackData == null)
            {
                return 0;
            }

            float playerMultiplier = playerStats != null ? playerStats.DamageMultiplier : 1f;
            return Mathf.Max(0, Mathf.RoundToInt(attackData.GetDamageAtLevel(attackLevel) * playerMultiplier));
        }

        public float GetAttackCooldown(AttackData attackData)
        {
            return GetAttackCooldownAtLevel(attackData, Mathf.Max(1, GetAttackLevel(attackData)));
        }

        public float GetAttackCooldownAtLevel(AttackData attackData, int attackLevel)
        {
            if (attackData == null)
            {
                return 0f;
            }

            return attackData.GetCooldownAtLevel(attackLevel);
        }

        private bool TryAttack(AttackData attackData)
        {
            MonoBehaviour target = FindClosestTarget(attackData);
            if (target == null)
            {
                return false;
            }

            Vector2 attackDirection = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            if (attackDirection == Vector2.zero)
            {
                attackDirection = Vector2.right;
            }

            if (attackData.projectilePrefab != null)
            {
                ShootProjectile(attackData, target.transform, GetScaledDamage(attackData));
                AttackPerformed?.Invoke(attackDirection, attackData.attackSpriteDuration);
                return true;
            }

            ((IDamageable)target).TakeDamage(GetScaledDamage(attackData), transform);
            AttackPerformed?.Invoke(attackDirection, attackData.attackSpriteDuration);
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

        private void ShootProjectile(AttackData attackData, Transform target, int damage)
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
                damage,
                attackData.projectileSpeed,
                attackData.projectileLifetime,
                attackData.targetLayer,
                transform);
        }

        private int GetScaledDamage(AttackData attackData)
        {
            return GetAttackDamage(attackData);
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

            while (_attackLevels.Count < equippedAttacks.Count)
            {
                _attackLevels.Add(1);
            }

            while (_attackLevels.Count > equippedAttacks.Count)
            {
                _attackLevels.RemoveAt(_attackLevels.Count - 1);
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
