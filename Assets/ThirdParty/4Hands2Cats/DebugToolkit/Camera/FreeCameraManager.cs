using DebugToolkit.Interaction.Commands;
using UnityEngine;

namespace DebugToolkit.Freecam
{
    public class FreeCameraManager : MonoBehaviour
    {
        [Header("Command")]
        [SerializeField] BooleanCommand freeCamCommand;

        [Header("Params")]
        [SerializeField] GameObject freeCamPrefab;
        private GameObject freeCam;

        public bool IsFreeCamActive => freeCam != null;
        public Camera Camera { get; private set; }
        public Quaternion FreeCamRot => freeCam.transform.rotation;

        private void Awake()
        {
            freeCamCommand.OnIsValid += FreeCamCommand_OnIsValid;
        }

        private void OnDestroy()
        {
            freeCam = null;
            freeCamCommand.OnIsValid -= FreeCamCommand_OnIsValid;
        }

        private void FreeCamCommand_OnIsValid(bool obj)
        {
            if (this == null) return;

            if (obj)
            {
                InstantiateFreeCam();
            }
            else
            {
                Destroy(freeCam);
            }
        }

        private void InstantiateFreeCam()
        {
            if (freeCam == null)
            {
                freeCam = Instantiate(freeCamPrefab, Camera.main.transform.position, Camera.main.transform.rotation);
                Camera = freeCam.GetComponent<Camera>();
            }
        }
    }
}

