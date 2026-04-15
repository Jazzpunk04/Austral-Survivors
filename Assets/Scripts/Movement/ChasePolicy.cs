using MovementPolicies;
using UnityEngine;

namespace Movement
{
    [CreateAssetMenu(menuName = "AI/Movement/Chase")]
    public class ChasePolicy : MovementPolicy
    {
        public override Vector2 GetDirection(Transform self, Transform target)
        {
            if (target == null) return Vector2.zero;

            return (target.position - self.position).normalized;
        }
    }
}