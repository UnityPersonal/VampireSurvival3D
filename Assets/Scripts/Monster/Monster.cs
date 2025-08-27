using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class Monster : MonoBehaviour
{
    [Required ,SerializeField] private Animator animator;
    [SerializeField] private int destinationUpdateFramePerCount = 1;
    private int remain = 0;
    [SerializeField, Required] NavMeshAgent agent;
    public NavMeshAgent Agent => agent;
    
    private static Camera mainCamera;
    
    private void Start()
    {
        remain = Random.Range(0, 1000);
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        var vieport = mainCamera.WorldToViewportPoint(transform.position);
        var isOutViewport = vieport.x < 0 || vieport.x > 1  || vieport.y < 0 || vieport.y > 1;
        animator.enabled = !isOutViewport;
        
        if (remain == 0) // 프레임 분산 처리
        {
            remain = destinationUpdateFramePerCount;
            UpdateDestination();
        }
        remain--;
    }

    void UpdateDestination()
    {
        Profiler.BeginSample("Monster.UpdateDestination");
        agent.SetDestination(Player.Instance.transform.position);
    }
}
