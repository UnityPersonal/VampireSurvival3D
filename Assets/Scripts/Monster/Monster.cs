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
    private Transform monsterTransform;
    
    private void Start()
    {
        remain = Random.Range(0, 1000);
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        playerTransform = Player.Instance.transform;
        monsterTransform = transform;
    }

    void Update()
    {
        /*if (remain == 0) // 프레임 분산 처리
        {
            remain = destinationUpdateFramePerCount;
            UpdateDestination();
        }
        remain--;*/
    }

    void UpdateDestination()
    {
        agent.SetDestination(playerTransform.position);
    }
}
