using System;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField,AssetsOnly] Monster[] samples;
    //[SerializeField,AssetsOnly] Monster sample;

    [ShowInInspector] private int currentSpawnCount = 0;
    [SerializeField] private int maxSpawnCount = 1000;
    [SerializeField] private int spawnPerCount = 10;
    [SerializeField] private float spawnRadius = 50f;
    [SerializeField] private float spawnInterval = 1f;

    private float lastSpawnTime = 0f;
        
    private ObjectPool<Monster>[] mosterSamplePools;

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
                
                var sample = samples[Random.Range(0, samples.Length)];
                
                Instantiate(sample, spawnPosition, Quaternion.identity);
            }
            currentSpawnCount +=  spawnPerCount;
        }
    }
}
