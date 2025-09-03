using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour , ICombatable
{
    static int monsterIdGenerator = 0;
    
    static readonly int FlickerID = Shader.PropertyToID("_FlickerFactor"); 
    
    private int mosternUID = 0;
    public int MonsterUID => mosternUID;
    private Vector3 lastRequestedPos;
    [SerializeField, Required] NavMeshAgent agent;
    public NavMeshAgent Agent => agent;
    [SerializeField, Required] AnimatedMesh animMesh;
    public AnimatedMesh AnimMesh => animMesh;
    [SerializeField, Required] Collider monsterCollider;
    public Collider MonsterCollider => monsterCollider;
    [SerializeField, Required] Renderer monsterMeshRenderer;
    public Renderer MonsterMeshRenderer => monsterMeshRenderer;

    [SerializeField, Min(0.5f)] private float attackRadius;
    public float AttackRadius => attackRadius;
    
    private MaterialPropertyBlock mpb;
    
    private static Camera mainCamera;
    
    private Transform playerTransform;
    
    private const float RadiusDestination = 0.2f;

    [SerializeField] private float hp;
    
    public bool IsAttacking { get; set; }= false;
    
    private readonly Dictionary<IMonsterState.StateType, IMonsterState> states =  new Dictionary<IMonsterState.StateType, IMonsterState>();
    private IMonsterState currentState = null;

    public void Swap(IMonsterState.StateType type)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = states[type];
        currentState.Enter();
    }
    
    private void Start()
    {
        mosternUID = monsterIdGenerator;
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        playerTransform = Player.Instance.transform;

        agent.avoidancePriority = Random.Range(50, 1000);
        animMesh.OnAnimationEnd += OnAnimationEnd;
        
        mpb = new MaterialPropertyBlock();

        states[IMonsterState.StateType.Move] = new MonsterMoveState(this);
        states[IMonsterState.StateType.Attack] = new MonsterAttackState(this);
        states[IMonsterState.StateType.Die] = new MonsterDieState(this);
        
        Swap(IMonsterState.StateType.Move);
    }

    private void OnAnimationEnd(string animationName)
    {
        if (animationName.Equals("Attack"))
        {
            Swap(IMonsterState.StateType.Move);
        }
    }

    private void OnEnable()
    {
        CombatManager.Regist(monsterCollider, this);
    }

    private void OnDisable()
    {
        CombatManager.Unregist(monsterCollider);
    }

    void Update()
    {
        if (IsDead) return;

        if (currentState != null)
        {
            currentState.Execute();
        }
    }

    public void UpdateDestination()
    {
        var destination = playerTransform.position;
        var offset = RadiusDestination * Random.insideUnitCircle;
        destination.x += offset.x;
        destination.z += offset.y;
        
        lastRequestedPos = destination;
        agent.SetDestination(destination);
    }

    public Transform CombatTransform => transform;
    public Collider CombatCollider => monsterCollider;
    
    Coroutine coroutine;

    public bool IsDead { get; set; } = false;
    public void TakeDamage(DealEventArgs args)
    {
        if (IsDead) return;
        
        hp -= args.DealDamage;
        if (hp <= 0)
        {
            Swap(IMonsterState.StateType.Die);
        }
        else
        {
            if (coroutine is not null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = StartCoroutine(FlickerCoroutine());
            
            GameEventManager.Publish(new TakeDamageEventArgs(this, (int)args.DealDamage));
        }
    }

    IEnumerator FlickerCoroutine()
    {
        monsterMeshRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(FlickerID, 1);
        monsterMeshRenderer.SetPropertyBlock(mpb);
        yield return new WaitForSeconds(0.2f);
        monsterMeshRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(FlickerID, 0);
        monsterMeshRenderer.SetPropertyBlock(mpb);
    }
    
}
