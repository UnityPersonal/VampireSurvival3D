using DebugToolkit.Freecam;
using DebugToolkit.Gizmos;
using UnityEngine;

namespace DebugToolkit.Sample.Navmesh
{
    public class DebugPathCorner
    {
        private Gizmo_Collider _gizmoShape;
        private Gizmo_RaycastTo _gizmoRaycast;

        private GameObject _debugGo;

        public DebugPathCorner(GameObject go)
        {
            _debugGo = new GameObject("DebugPathCorner");
            _debugGo.transform.SetParent(go.transform);
            _gizmoShape = _debugGo.AddComponent<Gizmo_Collider>();
            _gizmoRaycast = _debugGo.AddComponent<Gizmo_RaycastTo>();
        }

        public void Draw(Vector3 startLocation, Vector3 targetLoaction, FreeCameraManager freeCameraManager)
        {
            _debugGo.transform.position = startLocation;
            _gizmoRaycast.DrawTo(targetLoaction, Color.yellow);
            _gizmoShape.DrawBox(new Vector3(0.2f, 0.2f, 0.2f), targetLoaction - startLocation, Color.yellow);
            _gizmoRaycast.enabled = true;
            _gizmoRaycast.Init(freeCameraManager);
            _gizmoShape.enabled = true;
        }

        public void StopDraw()
        {
            _gizmoRaycast.enabled = false;
            _gizmoShape.enabled = false;
        }
    }
}
