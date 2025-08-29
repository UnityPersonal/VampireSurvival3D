using UnityEditor;
using UnityEngine;
using DebugToolkit.Freecam;

namespace DebugToolkit.Gizmos
{
    public class Gizmo_RaycastTo : Gizmo_Base
    {
        public enum ERaycastToMode
        {
            Gameobject,
            Position
        }

        [SerializeField] private Vector3 targetPosition;
        public Vector3 TargetPosition { get { return targetPosition; } set { targetPosition = value; } }
        [SerializeField] private GameObject target;
        public GameObject Target { get { return target; } set { target = value; } }
        [SerializeField] private ERaycastToMode raycastToMode;
        public ERaycastToMode RaycastToMode => raycastToMode;

        private Transform _tr;
        private FreeCameraManager _freeCameraManager;

        private void Awake()
        {
            _tr = transform;
        }

        public void Init(FreeCameraManager freeCameraManager)
        {
            _freeCameraManager = freeCameraManager;
        }

        public void OnRenderObject()
        {
            if (_canDrawnGizmos)
            {
                CreateLineMaterial();
                lineMaterial.SetPass(0);
                GL.PushMatrix();

                if (_freeCameraManager.IsFreeCamActive) GL.modelview = _freeCameraManager.Camera.worldToCameraMatrix;
                else GL.modelview = Camera.main.worldToCameraMatrix;

                GL.Begin(GL.LINES);
                GL.Color(color);
                GL.Vertex3(_tr.position.x, _tr.position.y, _tr.position.z);

                switch (raycastToMode)
                {
                    case ERaycastToMode.Gameobject:
                        GL.Vertex3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
                        break;
                    case ERaycastToMode.Position:
                        GL.Vertex3(TargetPosition.x, TargetPosition.y, TargetPosition.z);
                        break;
                }

                GL.End();
                GL.PopMatrix();
            }
        }

        public void DrawTo(GameObject target, Color color)
        {
            raycastToMode = ERaycastToMode.Gameobject;
            this.target = target;
            this.color = color;
            drawGizmo = true;
        }

        public void DrawTo(Vector3 targetPosition, Color color)
        {
            raycastToMode = ERaycastToMode.Position;
            this.targetPosition = targetPosition;
            this.color = color;
            drawGizmo = true;
        }
    }
}
