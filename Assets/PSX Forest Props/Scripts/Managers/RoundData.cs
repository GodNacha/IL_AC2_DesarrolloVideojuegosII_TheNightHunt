using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public EnemyData enemyData;
    public int amount = 1;
}

[System.Serializable]
public class ItemSpawnInfo
{
    public GameObject itemPrefab;

    [Min(0)]
    public int weight = 1;
}

[CreateAssetMenu(fileName = "NewRound", menuName = "Game/Round Data")]
public class RoundData : ScriptableObject
{
    [Header("Enemies")]
    public EnemySpawnInfo[] enemies;

    [Header("Items")]
    public ItemSpawnInfo[] possibleItems;
    public int maxItems = 2;

    [Header("Timing")]
    public float spawnInterval = 1f;
    public float timeBeforeNextRound = 5f;
}