using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace DebugToolkit.Sample.Navmesh
{
    public class BasicAgentBehaviour : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField] private Vector2 tickMinMax;

        [Header("Dependencies")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;

        private void Awake()
        {
            Behave();
            agent.updateRotation = true;
        }

        private async void Behave()
        {
            while (true)
            {
                if (this == null) return;
                Vector3 randomPoint = transform.position + UnityEngine.Random.insideUnitSphere * 8f;
                NavMeshHit hit;

                if(NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                    await Awaitable.WaitForSecondsAsync(UnityEngine.Random.Range(tickMinMax.x, tickMinMax.y));
                }
                else
                {
                    await Awaitable.NextFrameAsync();
                }
            }
        }

        private void Update()
        {
            
            animator.SetFloat("Speed", agent.velocity.magnitude * 1.75f);
            animator.SetFloat("MotionSpeed", 1);
        }
    }
}
