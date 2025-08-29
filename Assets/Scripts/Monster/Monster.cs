using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

//[RequireComponent(typeof(NavMeshAgent))]
public class Monster : MonoBehaviour
{
    static int monsterIdGenerator = 0;
    
    //Required ,SerializeField] private Animator animator;
    [SerializeField] private int destinationUpdateFramePerCount = 1;
    private int mosternUID = 0;
    private Vector3 lastRequestedPos;
    [SerializeField, Required] NavMeshAgent agent;
    public NavMeshAgent Agent => agent;
    
    private static Camera mainCamera;
    
    private Transform playerTransform;
    
    [SerializeField, Required] Animator animator;
    [SerializeField] int frameInterval = 5;

    private const float radiusDestination = 0.2f;
    
    private void Start()
    {
        mosternUID = monsterIdGenerator;
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        playerTransform = Player.Instance.transform;

        agent.avoidancePriority = Random.Range(50, 1000);
    }

    void Update()
    {
        // 에이전트별로 프레임 분산 (예: 5프레임마다 1회 경로 갱신)
        if ((Time.frameCount + mosternUID) % frameInterval == 0) {
            if ((playerTransform.position - lastRequestedPos).sqrMagnitude > 1.0f) 
            {
                UpdateDestination();
            }
        }

    }

    void UpdateDestination()
    {
        var destination = playerTransform.position;
        var offset = radiusDestination * Random.insideUnitCircle;
        destination.x += offset.x;
        destination.z += offset.y;
        
        lastRequestedPos = destination;
        agent.SetDestination(destination);
    }

}
