using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentUpdateSystem : SingletonOnlyScene<NavAgentUpdateSystem>
{
    public int maxCountPerCollection = 500;
    class UpdateCollection
    {
        public List<Monster> monsters = new List<Monster>();
    }

    private int currentUpdateIndex = 0;
    List<UpdateCollection> collections = new List<UpdateCollection>();

    public void RegisterNavMeshAgent(Monster monster)
    {
        if (collections.Count == 0 || collections.Last().monsters.Count  >= maxCountPerCollection)
        {
            var collection =  new UpdateCollection();
            collection.monsters.Add(monster);
            collections.Add(collection);
        }
        else
        {
            collections.Last().monsters.Add(monster);
        }
    }

    public void UnregisterNavMeshAgent(Monster monster)
    {
    }
    
    private void Update()
    {
        /*var collection = collections[currentUpdateIndex];

        var destination = Player.Instance.transform.position;
        foreach (var monster in collection.monsters)
        {
            monster.Agent.SetDestination(destination);
        }
        
        currentUpdateIndex++;
        currentUpdateIndex %= collections.Count;*/
    }

    protected override void InitializeSingleton()
    {
        
    }
}
