using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game/Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("General")]
    public string enemyName;

    [Header("Visuals")]
    public Sprite sprite;
    public Sprite attackSprite;
    public RuntimeAnimatorController animatorController;

    [Header("Stats")]
    [Min(1)] public int maxHealth = 10;
    [Min(0)] public int experienceReward = 1;

    [Header("Attack")]
    public AttackData attackData;
}
