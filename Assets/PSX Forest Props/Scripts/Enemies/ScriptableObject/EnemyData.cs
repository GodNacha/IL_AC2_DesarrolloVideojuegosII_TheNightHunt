using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;

    [Header("Stats")]
    public int maxHealth = 100;
    public float moveSpeed = 3f;
}
