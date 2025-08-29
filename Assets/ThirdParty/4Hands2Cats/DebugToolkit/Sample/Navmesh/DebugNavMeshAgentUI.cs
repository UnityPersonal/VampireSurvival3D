using DebugToolkit.Freecam;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace DebugToolkit.Sample.Navmesh
{
    public class DebugNavMeshAgentUI
    {
        public DebugNavMeshAgentUI(NavMeshAgent navMeshAgent, GameObject prefab, FreeCameraManager freeCameraManager)
        {
            _agent = navMeshAgent;
            _prefab = prefab;
            _freeCameraManager = freeCameraManager;
        }

        private NavMeshAgent _agent;
        private GameObject _prefab;
        private UI.NavMeshInfoUIManager _navMeshInfoUIManager;
        private FreeCameraManager _freeCameraManager;
        private bool _updateUI = false;

        internal void ToggleUI(bool isEnable)
        {
            _updateUI = false;
            if (isEnable)
            {
                _navMeshInfoUIManager = GameObject.Instantiate(_prefab, _agent.transform).GetComponent<UI.NavMeshInfoUIManager>();
                _navMeshInfoUIManager.Init(_freeCameraManager);
                TickUI();
            }
            else
            {
                GameObject.Destroy(_navMeshInfoUIManager.gameObject);
            }
        }

        private async void TickUI()
        {
            if (_updateUI) return;
            _updateUI = true;

            while (_updateUI)
            {
                float distance = CalculateDistanceLeft();
                _navMeshInfoUIManager.SetTimeLeftToDestination(CalculateTimeLeft(distance));
                _navMeshInfoUIManager.SetDistanceLeftToDestination(distance);
                await Awaitable.WaitForSecondsAsync(0.1f);
            }
        }

        private float CalculateDistanceLeft()
        {
            if (!_agent.hasPath || _agent.pathStatus != NavMeshPathStatus.PathComplete)
                return 0f;

            float distance = 0f;
            Vector3[] corners = _agent.path.corners;

            distance += Vector3.Distance(_agent.transform.position, corners[0]);

            for (int i = 0; i < corners.Length - 1; i++)
            {
                distance += Vector3.Distance(corners[i], corners[i + 1]);
            }

            return distance;
        }

        private float CalculateTimeLeft(float distance)
        {
            if (_agent.speed <= 0f)
                return Mathf.Infinity;

            return distance / _agent.speed;
        }
    }
}

