using MovementPolicies;
using UnityEngine;

namespace Movement
{
    [CreateAssetMenu(menuName = "AI/Movement/Flee")]
    public class FleePolicy : MovementPolicy
    {
        public override Vector2 GetDirection(Transform self, Transform target)
        {
            if (target == null) return Vector2.zero;

            return (self.position - target.position).normalized;
        }
    }
}