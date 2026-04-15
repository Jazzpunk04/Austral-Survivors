using UnityEngine;

namespace MovementPolicies
{
    public abstract class MovementPolicy : ScriptableObject
    {
        public abstract Vector2 GetDirection(Transform self, Transform target);
    }
}