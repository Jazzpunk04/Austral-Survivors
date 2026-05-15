using System.Collections;
using Combat;
using Controllers;
using Health;
using Movement;
using MovementPolicies;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(EnemyController))]
    [RequireComponent(typeof(EnemyAttack))]
    [RequireComponent(typeof(MovementHandler))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyData enemyData;
        [SerializeField] private Transform target;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private EnemyController controller;
        [SerializeField] private EnemyAttack attack;
        [SerializeField] private MovementHandler movementHandler;
        [SerializeField] private MovementPolicy movementPolicy;
        [SerializeField] private EnemyHealth enemyHealth;

        public EnemyData Data => enemyData;
        public Transform Target => target;
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public EnemyController Controller => controller;
        public EnemyAttack Attack => attack;
        public MovementHandler MovementHandler => movementHandler;
        public MovementPolicy MovementPolicy => movementPolicy;

        private void Awake()
        {
            ResolveReferences();
            ApplyData();
            if (enemyHealth != null && enemyData != null)
            {
                enemyHealth.SetUp(enemyData.maxHealth, enemyData.experienceReward);
            }
        }

        private void OnValidate()
        {
            ResolveReferences();
            ApplyData();
        }

        private void ResolveReferences()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            if (controller == null)
            {
                controller = GetComponent<EnemyController>();
            }

            if (attack == null)
            {
                attack = GetComponent<EnemyAttack>();
            }

            if (movementHandler == null)
            {
                movementHandler = GetComponent<MovementHandler>();
            }

            if (enemyHealth == null)
            {
                enemyHealth = GetComponent<EnemyHealth>();
            }
        }

        private void ApplyData()
        {
            if (enemyData == null)
            {
                return;
            }

            if (animator != null)
            {
                animator.runtimeAnimatorController = enemyData.animatorController;
            }

            if ((animator == null || animator.runtimeAnimatorController == null) && spriteRenderer != null && enemyData.sprite != null)
            {
                spriteRenderer.sprite = enemyData.sprite;
            }
        }
        
        private Coroutine _spriteRoutine;

        public void ShowAttackSprite(AttackData attackData)
        {
            if (attackData == null || enemyData.attackSprite == null || spriteRenderer == null)
            {
                return;
            }

            Animator animator = GetComponentInChildren<Animator>();
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                return;
            }

            if (_spriteRoutine != null)
            {
                StopCoroutine(_spriteRoutine);
            }

            _spriteRoutine = StartCoroutine(ShowAttackSpriteRoutine(attackData));
        }

        private IEnumerator ShowAttackSpriteRoutine(AttackData attackData)
        {
            spriteRenderer.sprite = enemyData.attackSprite;
            yield return new WaitForSeconds(attackData.attackSpriteDuration);
            ApplyData();
            _spriteRoutine = null;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

    }
}
