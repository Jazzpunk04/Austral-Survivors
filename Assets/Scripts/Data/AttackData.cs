using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackData", menuName = "Game/Combat/Attack Data")]
public class AttackData : ScriptableObject
{
    [Header("General")]
    public string attackName = "Melee Attack";

    [Header("Damage")]
    [Min(0)] public int damage = 1;

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

    [Header("Visuals")]
    [Min(0f)] public float attackSpriteDuration = 0.15f;
}
