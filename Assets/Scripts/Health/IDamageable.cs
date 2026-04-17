using UnityEngine;

namespace Health
{
    public interface IDamageable
    {
        void TakeDamage(int damage);
        void TakeDamage(int damage, Transform damageSource);
    }
}
