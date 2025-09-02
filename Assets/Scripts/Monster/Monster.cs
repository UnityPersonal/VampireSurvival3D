using System;
using System.Collections;
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
    private Vector3 lastRequestedPos;
    [SerializeField, Required] NavMeshAgent agent;
    [SerializeField, Required] AnimatedMesh animMesh;
    [SerializeField, Required] Collider monsterCollider;
    [SerializeField, Required] Renderer monsterMeshRenderer;

    [SerializeField, Min(0.5f)] private float attackRadius;
    
    private MaterialPropertyBlock mpb;
    public NavMeshAgent Agent => agent;
    
    private static Camera mainCamera;
    
    private Transform playerTransform;
    
   [SerializeField] int frameInterval = 5;

    private const float radiusDestination = 0.2f;

    [SerializeField] private float hp;
    
    private bool isAttacking = false;
    
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
        animMesh.Play("Move");
        
        mpb = new MaterialPropertyBlock();
    }

    private void OnAnimationEnd(string animationName)
    {
        if (animationName.Equals("Attack"))
        {
            isAttacking = false;
            agent.isStopped = false;
            animMesh.Play("Move");
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
        if (isDead) return;
        
        // 에이전트별로 프레임 분산 (예: 5프레임마다 1회 경로 갱신)
        /*if ((Time.frameCount + mosternUID) % frameInterval == 0) {
            if ((playerTransform.position - lastRequestedPos).sqrMagnitude > 1.0f) 
            {
                UpdateDestination();
            }
        }*/

        if (isAttacking == false)
        {
            var myPosition = transform.position;
            var playerPoistion =playerTransform.position;
            var toPlayer = playerPoistion - myPosition;
            if (attackRadius > toPlayer.magnitude)
            {
                BeginAttack();
            }
            else
            {
                UpdateDestination();
            }
        }
    }

    void BeginAttack()
    {
        animMesh.Play("Attack");
        agent.isStopped = true;
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

    public Transform CombatTransform => transform;
    public Collider CombatCollider => monsterCollider;
    
    Coroutine coroutine;

    private bool isDead = false;
    public void TakeDamage(DealEventArgs args)
    {
        if (isDead) return;
        
        hp -= args.DealDamage;
        if (hp <= 0)
        {
            isDead = true;
            monsterCollider.enabled = false;
            agent.enabled = false;
            animMesh.Play("Die");
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
