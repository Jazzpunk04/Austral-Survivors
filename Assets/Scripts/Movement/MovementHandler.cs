using Data;
using UnityEngine;

namespace Movement
{
    public class MovementHandler : MonoBehaviour
    {
        [SerializeField] private SpeedData speedData;
        [SerializeField] private PlayerStats playerStats;

        private Vector2 _currentDirection;

        private void Awake()
        {
            if (playerStats == null)
            {
                playerStats = GetComponent<PlayerStats>();
            }
        }

        public void SetDirection(Vector2 direction)
        {
            _currentDirection = direction.normalized;
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            Vector3 movement = new Vector3(_currentDirection.x, _currentDirection.y, 0f);
            float baseSpeed = speedData != null ? speedData.SpeedValue : 0f;
            float multiplier = playerStats != null ? playerStats.MoveSpeedMultiplier : 1f;
            transform.position += movement * (baseSpeed * multiplier) * Time.deltaTime;
        }
    }
}
