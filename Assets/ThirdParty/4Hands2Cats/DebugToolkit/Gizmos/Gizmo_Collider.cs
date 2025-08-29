using System;
using UnityEngine;

namespace DebugToolkit.Gizmos
{
    public class Gizmo_Collider : Gizmo_Base
    {
        [SerializeField] private Shape shape;

        public Shape Shape => shape;

        Transform colliderTransform;

        [SerializeField] private Vector3 size;
        [SerializeField] private float height;
        [SerializeField] private float radius;
        [SerializeField] private Vector3 center;
        [SerializeField] private Mesh mesh;
        private bool isReadable;

        public void OnRenderObject()
        {
            if (_canDrawnGizmos)
            {
                CreateLineMaterial();
                lineMaterial.SetPass(0);
                try
                {
                    GL.PushMatrix();
                    GL.MultMatrix(transform.localToWorldMatrix);

                    if (shape == Shape.Box) DrawBox();
                    else if (shape == Shape.Sphere) DrawSphere();
                    else if (shape == Shape.Capsule) DrawCapsule();
                    else if (shape == Shape.Mesh && isReadable) DrawMesh();
                }
                finally
                {
                    GL.PopMatrix();
                }
            }
        }

        public static Gizmo_Collider DrawBoxGizmos(GameObject targetObject, BoxCollider boxCollider, Color colliderColor)
        {
            Gizmo_Collider g = targetObject.AddComponent<Gizmo_Collider>();
            g.shape = Shape.Box;
            g.center = boxCollider.center;
            g.size = boxCollider.size;
            g.drawGizmo = true;
            g.color = colliderColor;
            return g;
        }

        public static Gizmo_Collider DrawSphereGizmos(GameObject gameObject, SphereCollider sphereCollider, Color gizmosColor)
        {
            Gizmo_Collider g = gameObject.AddComponent<Gizmo_Collider>();
            g.colliderTransform = gameObject.transform;
            g.shape = Shape.Sphere;
            g.center = sphereCollider.center;
            g.radius = sphereCollider.radius;
            g.drawGizmo = true;
            g.color = gizmosColor;
            return g;
        }

        public static Gizmo_Collider DrawCapsuleGizmos(GameObject gameObject, CapsuleCollider capsuleCollider, Color gizmosColor)
        {
            Gizmo_Collider g = gameObject.AddComponent<Gizmo_Collider>();
            g.shape = Shape.Capsule;
            g.center = capsuleCollider.center;
            g.radius = capsuleCollider.radius;
            g.height = capsuleCollider.height;
            g.drawGizmo = true;
            g.color = gizmosColor;
            return g;
        }

        public static Gizmo_Collider DrawMeshGizmos(GameObject gameObject, MeshCollider meshCollider, Color gizmosColor)
        {
            Gizmo_Collider g = gameObject.AddComponent<Gizmo_Collider>();
            g.shape = Shape.Mesh;
            g.mesh = meshCollider.sharedMesh;
            g.isReadable = g.mesh.isReadable;
            g.drawGizmo = true;
            g.color = gizmosColor;
            return g;
        }

        public void DrawBox(Vector3 size, Vector3 center, Color color)
        {
            this.size = size;
            this.center = center;
            shape = Shape.Box;
            this.color = color;
            drawGizmo = true;
        }

        private void DrawBox()
        {
            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);
            GL.Vertex3((-size.x / 2) + center.x, (-size.y / 2) + center.y, (-size.z / 2) + center.z);
            GL.Vertex3((-size.x / 2) + center.x, (-size.y / 2) + center.y, (size.z / 2) + center.z);
            GL.Vertex3((size.x / 2) + center.x, (-size.y / 2) + center.y, (size.z / 2) + center.z);
            GL.Vertex3((size.x / 2) + center.x, (-size.y / 2) + center.y, (-size.z / 2) + center.z);
            GL.Vertex3((-size.x / 2) + center.x, (-size.y / 2) + center.y, (-size.z / 2) + center.z);
            GL.End();

            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);
            GL.Vertex3((-size.x / 2) + center.x, (size.y / 2) + center.y, (-size.z / 2) + center.z);
            GL.Vertex3((-size.x / 2) + center.x, (size.y / 2) + center.y, (size.z / 2) + center.z);
            GL.Vertex3((size.x / 2) + center.x, (size.y / 2) + center.y, (size.z / 2) + center.z);
            GL.Vertex3((size.x / 2) + center.x, (size.y / 2) + center.y, (-size.z / 2) + center.z);
            GL.Vertex3((-size.x / 2) + center.x, (size.y / 2) + center.y, (-size.z / 2) + center.z);
            GL.End();

            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex3((-size.x / 2) + center.x, (-size.y / 2) + center.y, (-size.z / 2) + center.z);
            GL.Vertex3((-size.x / 2) + center.x, (size.y / 2) + center.y, (-size.z / 2) + center.z);
            GL.Vertex3((size.x / 2) + center.x, (-size.y / 2) + center.y, (-size.z / 2) + center.z);
            GL.Vertex3((size.x / 2) + center.x, (size.y / 2) + center.y, (-size.z / 2) + center.z);
            GL.Vertex3((-size.x / 2) + center.x, (-size.y / 2) + center.y, (size.z / 2) + center.z);
            GL.Vertex3((-size.x / 2) + center.x, (size.y / 2) + center.y, (size.z / 2) + center.z);
            GL.Vertex3((size.x / 2) + center.x, (-size.y / 2) + center.y, (size.z / 2) + center.z);
            GL.Vertex3((size.x / 2) + center.x, (size.y / 2) + center.y, (size.z / 2) + center.z);
            GL.End();
        }

        private void DrawSphere()
        {
            Vector3 position = center;

            if (Camera.main.orthographic)
            {
                Vector3 normal = position - Camera.main.transform.forward;
                float sqrMagnitude = normal.sqrMagnitude;
                float num0 = radius * radius;

                Vector3 center = position - (num0 * normal / sqrMagnitude);
                DrawWireDisc(center, normal, radius, color);
            }
            else
            {
                Vector3 normal = colliderTransform == null ? transform.position - Camera.main.transform.position : colliderTransform.position - Camera.main.transform.position;
                float sqrMagnitude = normal.sqrMagnitude;
                float num0 = radius * radius;
                float num1 = num0 * num0 / sqrMagnitude;
                float num2 = Mathf.Sqrt(num0 - num1);

                Vector3 center = position - (num0 * normal / sqrMagnitude);
                DrawWireDisc(center, normal, num2, color);
            }

            DrawWireDisc(position, Vector3.right, radius, color);
            DrawWireDisc(position, Vector3.up, radius, color);
            DrawWireDisc(position, Vector3.forward, radius, color);
        }

        private void DrawWireDisc(Vector3 center, Vector3 normal, float radius, Color color)
        {
            const int segments = 100;
            Vector3 tangent = Vector3.Cross(normal, Vector3.up).normalized;
            if (tangent.magnitude < 0.01f) tangent = Vector3.Cross(normal, Vector3.right).normalized;

            Vector3 bitangent = Vector3.Cross(normal, tangent).normalized;

            GL.Begin(GL.LINE_STRIP);

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                Vector3 point = center + radius * (Mathf.Cos(angle) * tangent + Mathf.Sin(angle) * bitangent);

                GL.Color(color);
                GL.Vertex(point);
            }

            GL.End();
        }

        private void DrawCapsule()
        {
            DrawHemisphere(Vector3.up * ((height / 2) - radius) + center, radius, 4, true);
            DrawHemisphere(Vector3.down * ((height / 2) - radius) + center, radius, 4, false);
            
            DrawCylinder(
                Vector3.down * ((height / 2) - radius) + center,
                Vector3.up * ((height / 2) - radius) + center,
                radius, 4);
        }

        private void DrawHemisphere(Vector3 center, float radius, int segments, bool isTop)
        {
            GL.Begin(GL.LINES);
            GL.Color(color);
            float angleStepHorizontal = 2 * Mathf.PI / segments;
            float angleStepVertical = Mathf.PI / 2 / segments;

            for (int i = 0; i < segments; i++)
            {
                float phi1 = i * angleStepHorizontal;
                float phi2 = (i + 1) * angleStepHorizontal;

                for (int j = 0; j < segments * 2; j++)
                {
                    float theta1 = j * angleStepVertical / 2;
                    float theta2 = (j + 1) * angleStepVertical / 2;

                    Vector3 p1 = SphericalToCartesian(radius, theta1, phi1, center, isTop);
                    Vector3 p2 = SphericalToCartesian(radius, theta2, phi1, center, isTop);

                    GL.Vertex(p1); GL.Vertex(p2);
                }
            }
            GL.End();
        }

        private Vector3 SphericalToCartesian(float radius, float theta, float phi, Vector3 center, bool isTop)
        {
            float y = radius * Mathf.Cos(theta);
            float x = radius * Mathf.Sin(theta) * Mathf.Cos(phi);
            float z = radius * Mathf.Sin(theta) * Mathf.Sin(phi);

            return center + new Vector3(x, isTop ? y : -y, z);
        }

        private void DrawCylinder(Vector3 bottomCenter, Vector3 topCenter, float radius, int segments)
        {
            float angleStep = 2 * Mathf.PI / segments;

            for (int i = 0; i < segments; i++)
            {
                float theta1 = i * angleStep;
                float theta2 = (i + 1) * angleStep;

                Vector3 p1 = bottomCenter + new Vector3(
                    radius * Mathf.Cos(theta1),
                    0,
                    radius * Mathf.Sin(theta1)
                );

                Vector3 p2 = bottomCenter + new Vector3(
                    radius * Mathf.Cos(theta2),
                    0,
                    radius * Mathf.Sin(theta2)
                );

                Vector3 p3 = topCenter + new Vector3(
                    radius * Mathf.Cos(theta1),
                    0,
                    radius * Mathf.Sin(theta1)
                );

                Vector3 p4 = topCenter + new Vector3(
                    radius * Mathf.Cos(theta2),
                    0,
                    radius * Mathf.Sin(theta2)
                );

                GL.Begin(GL.LINE_STRIP);
                GL.Color(color);
                GL.Vertex(p1);
                GL.Vertex(p3);
                GL.End();
                GL.Begin(GL.LINE_STRIP);
                GL.Color(color);
                GL.Vertex(p4);
                GL.Vertex(p2);
                GL.End();

                DrawWireDisc(topCenter, Vector3.up, radius, color);
                DrawWireDisc(bottomCenter, Vector3.up, radius, color);
            }
        }

        private void DrawMesh()
        {
            if (mesh == null)
                return;

            GL.Begin(GL.LINES);
            GL.Color(color);

            var vertices = mesh.vertices;
            var triangles = mesh.triangles;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 v0 = vertices[triangles[i]];
                Vector3 v1 = vertices[triangles[i + 1]];
                Vector3 v2 = vertices[triangles[i + 2]];

                DrawLine(v0, v1);
                DrawLine(v1, v2);
                DrawLine(v2, v0);
            }
            GL.End();
        }

        private void DrawLine(Vector3 start, Vector3 end)
        {
            GL.Vertex(start);
            GL.Vertex(end);
        }
    }

    public enum Shape
    {
        Box, Sphere, Capsule, Mesh,
    }
}

