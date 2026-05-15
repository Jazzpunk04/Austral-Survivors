using System.Collections;
using Combat;
using UnityEngine;

namespace EnemyAnimations
{
    [RequireComponent(typeof(EnemyAttack))]
    public class EnemyAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private EnemyAttack enemyAttack;
        [SerializeField, Min(0f)] private float movementDeadZone = 0.1f;
        [SerializeField, Min(0f)] private float minimumAttackDuration = 0.1f;
        [SerializeField] private bool flipWhenFacingLeft = true;

        private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
        private static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");
        private static readonly int DirectionXHash = Animator.StringToHash("directionX");
        private static readonly int DirectionYHash = Animator.StringToHash("directionY");

        private Vector2 _facingDirection = Vector2.down;
        private Coroutine _attackRoutine;

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (enemyAttack == null)
            {
                enemyAttack = GetComponent<EnemyAttack>();
            }
        }

        private void OnEnable()
        {
            if (enemyAttack != null)
            {
                enemyAttack.AttackPerformed += HandleAttackPerformed;
            }
        }

        private void OnDisable()
        {
            if (enemyAttack != null)
            {
                enemyAttack.AttackPerformed -= HandleAttackPerformed;
            }
        }

        public void SetMovementInput(Vector2 input)
        {
            bool isWalking = input.sqrMagnitude > movementDeadZone * movementDeadZone;

            if (isWalking)
            {
                SetFacingDirection(input);
            }

            ApplySpriteFlip();

            if (animator == null)
            {
                return;
            }

            animator.SetBool(IsWalkingHash, isWalking);
            animator.SetFloat(DirectionXHash, _facingDirection.x);
            animator.SetFloat(DirectionYHash, _facingDirection.y);
        }

        private void HandleAttackPerformed(Vector2 attackDirection, float attackDuration)
        {
            if (attackDirection != Vector2.zero)
            {
                SetFacingDirection(attackDirection);
            }

            ApplySpriteFlip();

            if (animator == null)
            {
                return;
            }

            animator.SetFloat(DirectionXHash, _facingDirection.x);
            animator.SetFloat(DirectionYHash, _facingDirection.y);

            if (_attackRoutine != null)
            {
                StopCoroutine(_attackRoutine);
            }

            _attackRoutine = StartCoroutine(SetAttackingForDuration(Mathf.Max(minimumAttackDuration, attackDuration)));
        }

        private IEnumerator SetAttackingForDuration(float duration)
        {
            animator.SetBool(IsAttackingHash, true);
            yield return new WaitForSeconds(duration);
            animator.SetBool(IsAttackingHash, false);
            _attackRoutine = null;
        }

        private void SetFacingDirection(Vector2 direction)
        {
            _facingDirection = direction.normalized;
        }

        private void ApplySpriteFlip()
        {
            if (!flipWhenFacingLeft || spriteRenderer == null || Mathf.Approximately(_facingDirection.x, 0f))
            {
                return;
            }

            spriteRenderer.flipX = _facingDirection.x < 0f;
        }
    }
}
