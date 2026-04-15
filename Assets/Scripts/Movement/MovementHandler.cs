using Data;
using UnityEngine;

namespace Movement
{
    public class MovementHandler : MonoBehaviour
    {
        [SerializeField] private SpeedData speedData;

        private Vector2 _currentDirection;

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
            transform.position += movement * speedData.SpeedValue * Time.deltaTime;
        }
    }
}
