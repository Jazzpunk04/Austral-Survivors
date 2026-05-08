using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackData", menuName = "Game/Combat/Attack Data")]
public class AttackData : ScriptableObject
{
    [Header("General")]
    public string attackName = "Melee Attack";

    [Header("Damage")]
    [Min(0)] public int damage = 1;
    [Min(0)] public int damagePerLevel = 1;

    [Header("Range")]
    [Min(0f)] public float attackRange = 1.25f;

    [Header("Targeting")]
    public LayerMask targetLayer = ~0;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    [Min(0f)] public float projectileSpeed = 8f;
    [Min(0f)] public float projectileLifetime = 3f;

    [Header("Timing")]
    [Min(0f)] public float cooldown = 1f;
    [Min(0f)] public float cooldownReductionPerLevel = 0.1f;
    [Min(0.05f)] public float minimumCooldown = 0.1f;

    [Header("Visuals")]
    [Min(0f)] public float attackSpriteDuration = 0.15f;

    public int GetDamageAtLevel(int level)
    {
        int levelBonusDamage = Mathf.Max(0, level - 1) * damagePerLevel;
        return damage + levelBonusDamage;
    }

    public float GetCooldownAtLevel(int level)
    {
        float levelReduction = Mathf.Max(0, level - 1) * cooldownReductionPerLevel;
        return Mathf.Max(minimumCooldown, cooldown - levelReduction);
    }
}
