using DebugToolkit.Console.Interaction.AttributeSystem;
using DebugToolkit.Interaction.Commands;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DebugToolkit.Gizmos
{
    public class GizmosManager : MonoBehaviour
    {
        [Header("Command")]
        [SerializeField] BooleanCommand gizmosCommand;
        [SerializeField] BooleanCommand colliderCommand;
        [SerializeField] BooleanCommand collider2DCommand;

        [Header("Params"), ColorUsage(true, true)]
        [SerializeField] private Color gizmosColor;

        private List<Collider> _colliders = new List<Collider>();
        private List<Gizmo_Collider> _gizmoColliders = new List<Gizmo_Collider>();

        private List<Collider2D> _colliders2D = new List<Collider2D>();
        private List<Gizmo_Collider2D> _gizmoColliders2D = new List<Gizmo_Collider2D>();

        private void Awake()
        {
            gizmosCommand.OnIsValid += GizmosCommand_OnIsValid;
            colliderCommand.OnIsValid += ColliderCommand_OnIsValid;
            collider2DCommand.OnIsValid += Collider2DCommand_OnIsValid;
        }

        private void OnDestroy()
        {
            gizmosCommand.OnIsValid -= GizmosCommand_OnIsValid;
            colliderCommand.OnIsValid -= ColliderCommand_OnIsValid;
            collider2DCommand.OnIsValid -= Collider2DCommand_OnIsValid;
        }

        private void GizmosCommand_OnIsValid(bool obj)
        {
            if (this == null) return;
            Gizmo_Base.DrawGizmo = obj;
        }

        // 3D 
        private void ColliderCommand_OnIsValid(bool obj)
        {
            if (this == null) return;

            if (obj)
            {
                DrawGizmosForAllColliders();
            }
            else
            {
                _colliders.Clear();
                for (int i = 0; i < _gizmoColliders.Count; i++)
                {
                    Destroy(_gizmoColliders[i]);
                }
            }
        }

        // 2D 
        private void Collider2DCommand_OnIsValid(bool obj)
        {
            if (this == null) return;

            if (obj)
            {
                DrawGizmosForAllColliders2D();
            }
            else
            {
                _colliders2D.Clear();
                for (int i = 0; i < _gizmoColliders2D.Count; i++)
                {
                    Destroy(_gizmoColliders2D[i]);
                }
            }
        }

        // 3D 
        private void DrawGizmosForAllColliders()
        {
            Gizmo_Collider.DrawGizmo = true;
            if (_colliders.Count == 0)
            {
                _colliders = FindObjectsByType<Collider>(FindObjectsSortMode.None).ToList();
                foreach (var collider in _colliders)
                {
                    if (collider is BoxCollider boxCollider)
                    {
                        _gizmoColliders.Add(Gizmo_Collider.DrawBoxGizmos(boxCollider.gameObject, boxCollider, gizmosColor));
                    }
                    else if (collider is SphereCollider sphereCollider)
                    {
                        _gizmoColliders.Add(Gizmo_Collider.DrawSphereGizmos(sphereCollider.gameObject, sphereCollider, gizmosColor));
                    }
                    else if (collider is CapsuleCollider capsuleCollider)
                    {
                        _gizmoColliders.Add(Gizmo_Collider.DrawCapsuleGizmos(capsuleCollider.gameObject, capsuleCollider, gizmosColor));
                    }
                    else if (collider is MeshCollider meshCollider)
                    {
                        _gizmoColliders.Add(Gizmo_Collider.DrawMeshGizmos(meshCollider.gameObject, meshCollider, gizmosColor));
                    }
                }
            }
        }

        // 2D 
        private void DrawGizmosForAllColliders2D()
        {
            Gizmo_Collider2D.DrawGizmo = true;
            if (_colliders2D.Count == 0)
            {
                _colliders2D = FindObjectsByType<Collider2D>(FindObjectsSortMode.None).ToList();
                foreach (var collider2D in _colliders2D)
                {
                    if (collider2D is BoxCollider2D boxCollider2D)
                    {
                        _gizmoColliders2D.Add(Gizmo_Collider2D.DrawBoxGizmos(boxCollider2D.gameObject, boxCollider2D, gizmosColor));
                    }
                    else if (collider2D is CircleCollider2D circleCollider2D)
                    {
                        _gizmoColliders2D.Add(Gizmo_Collider2D.DrawCircleGizmos(circleCollider2D.gameObject, circleCollider2D, gizmosColor));
                    }
                    else if (collider2D is CapsuleCollider2D capsuleCollider2D)
                    {
                        _gizmoColliders2D.Add(Gizmo_Collider2D.DrawCapsuleGizmos(capsuleCollider2D.gameObject, capsuleCollider2D, gizmosColor));
                    }
                    else if (collider2D is PolygonCollider2D polygonCollider2D)
                    {
                        _gizmoColliders2D.Add(Gizmo_Collider2D.DrawPolygonGizmos(polygonCollider2D.gameObject, polygonCollider2D, gizmosColor));
                    }
                }
            }
        }
    }
}

