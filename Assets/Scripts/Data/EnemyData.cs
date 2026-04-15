using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game/Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("General")]
    public string enemyName;
    public Sprite sprite;
    public Sprite attackSprite;

    [Header("Stats")]
    [Min(1)] public int maxHealth = 10;

    [Header("Attack")]
    public AttackData attackData;
}