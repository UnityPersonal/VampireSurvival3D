using System;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField,AssetsOnly] Monster sample;

    [ShowInInspector] private int currentSpawnCount = 0;
    [SerializeField] private int maxSpawnCount = 1000;
    [SerializeField] private int spawnPerCount = 10;
    [SerializeField] private float spawnRadius = 50f;
    [SerializeField] private float spawnInterval = 1f;

    private float lastSpawnTime = 0f;
        
    [SerializeField] private NavMeshSurface  navMesh;

    private void Start()
    {
        /*for (int i = 0; i < maxSpawnCount; i++)
        {
            var spawnPosition = Player.Instance.transform.position;
            var circle = Random.insideUnitCircle * spawnRadius;
            spawnPosition.x += circle.x;
            spawnPosition.z += circle.y;
                
            Instantiate(sample, spawnPosition, Quaternion.identity);
        }*/
    }

    void Update()
    {
        if (currentSpawnCount >= maxSpawnCount)
        {
            return;
        }
        
        if (lastSpawnTime + spawnInterval <= Time.time)
        {
            lastSpawnTime = Time.time;
            for (int i = 0; i < spawnPerCount; i++)
            {
                var spawnPosition = Player.Instance.transform.position;
                var circle = Random.insideUnitCircle * spawnRadius;
                spawnPosition.x += circle.x;
                spawnPosition.z += circle.y;
                
                Instantiate(sample, spawnPosition, Quaternion.identity);
            }
            currentSpawnCount +=  spawnPerCount;
        }
    }
}
