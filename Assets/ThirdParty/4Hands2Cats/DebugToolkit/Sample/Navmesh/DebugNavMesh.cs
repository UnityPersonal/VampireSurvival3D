using DebugToolkit.Freecam;
using DebugToolkit.Gizmos;
using DebugToolkit.Interaction.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

namespace DebugToolkit.Sample.Navmesh
{
    public class DebugNavMesh : MonoBehaviour
    {
        private List<DebugNavMeshAgent> _debugNavMeshAgents = new List<DebugNavMeshAgent>();
        private List<DebugNavMeshAgentUI> _debugNavMeshAgentUIs = new List<DebugNavMeshAgentUI>();
        private ObjectPool<DebugPathCorner> _pathCornerPool;

        [Header("Command")]
        [SerializeField] private BooleanCommand navMeshAgentGizmosCommand;
        [SerializeField] private BooleanCommand navMeshAgentInfoUI;
        [SerializeField] private BooleanCommand navMeshAgentAllCommand;

        [Header("Prefabs - UI")]
        [SerializeField] private GameObject navmeshAgentUIPrefab;

        [Header("Dependencies")]
        [SerializeField] private FreeCameraManager freeCameraManager;

        private void Awake()
        {
            navMeshAgentGizmosCommand.OnIsValid += NavMeshAgentGizmosCommand_OnIsValid;
            navMeshAgentInfoUI.OnIsValid += NavMeshAgentInfoUI_OnIsValid;
            navMeshAgentAllCommand.OnIsValid += NavMeshAgentAllCommand_OnIsValid;

            _pathCornerPool = new ObjectPool<DebugPathCorner>(
                () => new DebugPathCorner(gameObject),
                null,
                pc => pc.StopDraw(),
                null,
                true,
                10,
                200
            );
        }

        private void OnDestroy()
        {
            navMeshAgentGizmosCommand.OnIsValid -= NavMeshAgentGizmosCommand_OnIsValid;
            navMeshAgentInfoUI.OnIsValid -= NavMeshAgentInfoUI_OnIsValid;
            navMeshAgentAllCommand.OnIsValid -= NavMeshAgentAllCommand_OnIsValid;
        }

        private void Init()
        {
            if (_debugNavMeshAgents.Count == 0)
            {
                List<NavMeshAgent> _agents = FindObjectsByType<NavMeshAgent>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

                for (int i = 0; i < _agents.Count; i++)
                {
                    _debugNavMeshAgents.Add(new DebugNavMeshAgent(_agents[i], _pathCornerPool, freeCameraManager));
                    _debugNavMeshAgentUIs.Add(new DebugNavMeshAgentUI(_agents[i], navmeshAgentUIPrefab, freeCameraManager));
                }
            }
        }

        private void NavMeshAgentAllCommand_OnIsValid(bool isEnable)
        {
            NavMeshAgentGizmosCommand_OnIsValid(isEnable);
            NavMeshAgentInfoUI_OnIsValid(isEnable);
        }

        private void NavMeshAgentGizmosCommand_OnIsValid(bool isEnable)
        {
            if(this == null) return;
            Init();

            Gizmo_Base.DrawGizmo = true;

            foreach (var agent in _debugNavMeshAgents)
            {
                agent.ToggleGizmos(isEnable);
            }
        }

        private void NavMeshAgentInfoUI_OnIsValid(bool isEnable)
        {
            if (this == null) return;
            Init();

            foreach (var agent in _debugNavMeshAgentUIs)
            {
                agent.ToggleUI(isEnable);
            }
        }
    }
}
