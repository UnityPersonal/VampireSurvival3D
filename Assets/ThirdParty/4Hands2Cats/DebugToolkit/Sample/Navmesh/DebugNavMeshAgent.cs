using DebugToolkit.Freecam;
using DebugToolkit.Gizmos;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

namespace DebugToolkit.Sample.Navmesh
{
    public class DebugNavMeshAgent
    {
        public DebugNavMeshAgent(NavMeshAgent navMeshAgent, ObjectPool<DebugPathCorner> pathCornerPool, FreeCameraManager freeCameraManager)
        {
            _navMeshAgent = navMeshAgent;
            _go = _navMeshAgent.gameObject;
            _pathCornerPool = pathCornerPool;
            _freeCameraManager = freeCameraManager;
        }

        private NavMeshAgent _navMeshAgent;
        private GameObject _go;

        private ObjectPool<DebugPathCorner> _pathCornerPool;
        private bool _isTicking;

        private List<Vector3> _corners = new List<Vector3>();
        private List<DebugPathCorner> activateDebbugers = new List<DebugPathCorner>();

        private FreeCameraManager _freeCameraManager;

        internal void ToggleGizmos(bool isValid)
        {
            _isTicking = false;
            if (isValid)
                Tick();
        }

        private async void Tick()
        {
            if (_isTicking) return;
            _isTicking = true;

            while (_isTicking)
            {
                _corners = _navMeshAgent.path.corners.ToList();

                for (int i = 0; i < activateDebbugers.Count; i++)
                {
                    _pathCornerPool.Release(activateDebbugers[i]);
                }
                activateDebbugers.Clear();

                if (_corners.Count == 1)
                {
                    await Awaitable.WaitForSecondsAsync(0.1f);
                    continue;
                }

                for (int i = 0; i < _corners.Count; i++)
                {
                    activateDebbugers.Add(_pathCornerPool.Get());
                    activateDebbugers.Last().Draw(i > 0 ? _corners[i-1] : _go.transform.position, _corners[i], _freeCameraManager);

                    if (i >= 10)
                        break;
                }

                await Awaitable.WaitForSecondsAsync(0.1f);
            }
        }
    }
}

