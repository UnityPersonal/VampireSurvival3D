using DebugToolkit.Freecam;
using System;
using TMPro;
using UnityEngine;

namespace DebugToolkit.Sample.Navmesh.UI
{
    public class NavMeshInfoUIManager : MonoBehaviour
    {
        [Header("Dependencies - UI")]
        [SerializeField] private TextMeshProUGUI timeLeftToDestination;
        [SerializeField] private TextMeshProUGUI distanceLeftToDestination;

        [Header("Params")]
        [SerializeField] private string timeLeftToDestinationText = "Time left to destination: {0}";
        [SerializeField] private string distanceLeftToDestinationText = "Distance left to destination: {0}m";

        private Transform _tr;
        private FreeCameraManager _freeCameraManager;

        private void Awake()
        {
            _tr = transform;
        }

        internal void Init(FreeCameraManager freeCameraManager)
        {
            _freeCameraManager = freeCameraManager;
        }

        private void LateUpdate()
        {
            if(_freeCameraManager == null) return;
            Quaternion camRotation = _freeCameraManager.IsFreeCamActive ? _freeCameraManager.FreeCamRot : Camera.main.transform.rotation;
            _tr.LookAt(_tr.position + camRotation * Vector3.forward,
                              camRotation * Vector3.up);
        }

        public void SetTimeLeftToDestination(float timeInSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(timeInSeconds);
            string formattedTime = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);

            timeLeftToDestination.text = string.Format(timeLeftToDestinationText, formattedTime);
        }

        public void SetDistanceLeftToDestination(float distance)
        {
            distanceLeftToDestination.text = string.Format(distanceLeftToDestinationText, distance);
        }
    }
}

