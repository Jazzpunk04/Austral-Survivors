using System.Collections;
using Combat;
using UnityEngine;

namespace PlayerAnimations
{
    [RequireComponent(typeof(PlayerAttack))]
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerAttack playerAttack;
        [SerializeField, Min(0f)] private float movementDeadZone = 0.1f;
        [SerializeField, Min(0f)] private float minimumAttackDuration = 0.1f;

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

            if (playerAttack == null)
            {
                playerAttack = GetComponent<PlayerAttack>();
            }
        }

        private void OnEnable()
        {
            if (playerAttack != null)
            {
                playerAttack.AttackPerformed += HandleAttackPerformed;
            }
        }

        private void OnDisable()
        {
            if (playerAttack != null)
            {
                playerAttack.AttackPerformed -= HandleAttackPerformed;
            }
        }

        public void SetMovementInput(Vector2 input)
        {
            bool isWalking = input.sqrMagnitude > movementDeadZone * movementDeadZone;

            if (isWalking)
            {
                SetFacingDirection(input);
            }

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
            _facingDirection = SnapToEightDirections(direction);
        }

        private static Vector2 SnapToEightDirections(Vector2 direction)
        {
            if (direction == Vector2.zero)
            {
                return Vector2.down;
            }

            Vector2 normalized = direction.normalized;
            return new Vector2(SnapAxis(normalized.x), SnapAxis(normalized.y));
        }

        private static float SnapAxis(float value)
        {
            const float diagonalThreshold = 0.38268343f;

            if (value > diagonalThreshold)
            {
                return 1f;
            }

            if (value < -diagonalThreshold)
            {
                return -1f;
            }

            return 0f;
        }
    }
}
