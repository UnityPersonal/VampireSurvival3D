using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

//[RequireComponent(typeof(NavMeshAgent))]
public class Monster : MonoBehaviour
{
    //Required ,SerializeField] private Animator animator;
    [SerializeField] private int destinationUpdateFramePerCount = 1;
    private int remain = 0;
    [SerializeField, Required] NavMeshAgent agent;
    public NavMeshAgent Agent => agent;
    
    private static Camera mainCamera;
    
    private Transform playerTransform;
    
    [SerializeField, Required] Animator animator;
    [SerializeField] int frameInterval = 5;

    private const float radiusDestination = 0.2f;
    
    private void Start()
    {
        remain = Random.Range(0, 5);
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        playerTransform = Player.Instance.transform;

        agent.avoidancePriority = Random.Range(50, 1000);
    }

    void Update()
    {
        if (Time.frameCount % frameInterval == 0)
        {
            UpdateDestination();
        }
    }

    void UpdateDestination()
    {
        var destination = playerTransform.position;
        var offset = radiusDestination * Random.insideUnitCircle;
        destination.x += offset.x;
        destination.z += offset.y;
        agent.SetDestination(destination);
    }

}
