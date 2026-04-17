using MovementPolicies;
using Enemies;
using UnityEngine;

namespace Controllers
{
    [RequireComponent(typeof(Enemy))]
    public class EnemyController : MonoBehaviour
    {
        private Enemy enemy;

        private void Awake()
        {
            if (enemy == null)
            {
                enemy = GetComponent<Enemy>();
            }

        }

        private void Update()
        {

            if (enemy == null || enemy.MovementHandler == null)
            {
                return;
            }

            if (enemy.Attack != null && enemy.Attack.IsTargetInRange())
            {
                enemy.MovementHandler.SetDirection(Vector2.zero);
                enemy.Attack.TryAttack();
                return;
            }

            if (enemy.MovementPolicy == null)
            {
                enemy.MovementHandler.SetDirection(Vector2.zero);
                return;
            }
           
            Vector2 direction = enemy.MovementPolicy.GetDirection(transform, enemy.Target);

            enemy.MovementHandler.SetDirection(direction);
        }
    }
}
