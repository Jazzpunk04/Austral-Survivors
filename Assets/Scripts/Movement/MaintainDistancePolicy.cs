using MovementPolicies;
using UnityEngine;

namespace Movement
{
    [CreateAssetMenu(menuName = "AI/Movement/MaintainDistance")]
    public class MaintainDistancePolicy : MovementPolicy
    {
        [SerializeField] private float desiredDistance = 3f;
        [SerializeField] private float tolerance = 0.5f;

        public override Vector2 GetDirection(Transform self, Transform target)
        {
            if (target == null) return Vector2.zero;

            float distance = Vector2.Distance(self.position, target.position);

            if (distance > desiredDistance + tolerance)
            {
                return (target.position - self.position).normalized; // acercarse
            }

            if (distance < desiredDistance - tolerance)
            {
                return (self.position - target.position).normalized; // alejarse
            }

            return Vector2.zero; // quedarse
        }
    }
}