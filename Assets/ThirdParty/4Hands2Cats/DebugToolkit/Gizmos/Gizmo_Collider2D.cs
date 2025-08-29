using UnityEngine;

namespace DebugToolkit.Gizmos
{
    public class Gizmo_Collider2D : Gizmo_Base
    {
        [SerializeField] private Shape2D shape;
        public Shape2D Shape => shape;

        Transform colliderTransform;
        [SerializeField] private Vector2 size;
        [SerializeField] private float height;
        [SerializeField] private float radius;
        [SerializeField] private Vector2 center;
        [SerializeField] private CapsuleDirection2D capsuleDirection;
        [SerializeField] private PolygonCollider2D poly;

        public void OnRenderObject()
        {
            if (drawGizmo) ///_canDrawnGizmos
            {
                CreateLineMaterial();
                lineMaterial.SetPass(0);
                try
                {
                    GL.PushMatrix();
                    GL.MultMatrix(transform.localToWorldMatrix);
                    if (shape == Shape2D.Square) DrawSquare();
                    else if (shape == Shape2D.Circle) DrawCircle();
                    else if (shape == Shape2D.Capsule) DrawCapsule();
                    else if (shape == Shape2D.Polygon) DrawPolygon();
                }
                finally
                {
                    GL.PopMatrix();
                }
            }
        }

        public static Gizmo_Collider2D DrawBoxGizmos(GameObject targetObject, BoxCollider2D boxCollider2D, Color colliderColor)
        {
            Gizmo_Collider2D g = targetObject.AddComponent<Gizmo_Collider2D>();
            g.shape = Shape2D.Square;
            g.center = boxCollider2D.offset;
            g.size = boxCollider2D.size;
            g.drawGizmo = true;
            g.color = colliderColor;
            return g;
        }
        public static Gizmo_Collider2D DrawCircleGizmos(GameObject gameObject, CircleCollider2D circleCollider2D, Color gizmosColor)
        {
            Gizmo_Collider2D g = gameObject.AddComponent<Gizmo_Collider2D>();
            g.colliderTransform = gameObject.transform;
            g.shape = Shape2D.Circle;
            g.center = circleCollider2D.offset;
            g.radius = circleCollider2D.radius;
            g.drawGizmo = true;
            g.color = gizmosColor;
            return g;
        }
        public static Gizmo_Collider2D DrawCapsuleGizmos(GameObject gameObject, CapsuleCollider2D capsuleCollider2D, Color gizmosColor)
        {
            Gizmo_Collider2D g = gameObject.AddComponent<Gizmo_Collider2D>();
            g.colliderTransform = gameObject.transform;
            g.shape = Shape2D.Capsule;
            g.center = capsuleCollider2D.offset;
            g.radius = capsuleCollider2D.size.x / 2;
            g.height = capsuleCollider2D.size.y;
            g.capsuleDirection = capsuleCollider2D.direction;
            g.drawGizmo = true;
            g.color = gizmosColor;
            return g;
        }
        public static Gizmo_Collider2D DrawPolygonGizmos(GameObject gameObject, PolygonCollider2D polygonCollider2D, Color gizmosColor)
        {
            Gizmo_Collider2D g = gameObject.AddComponent<Gizmo_Collider2D>();
            g.colliderTransform = gameObject.transform;
            g.shape = Shape2D.Polygon;
            g.poly = polygonCollider2D;
            g.center = g.poly.offset;
            g.drawGizmo = true;
            g.color = gizmosColor;
            return g;
        }

        private void DrawSquare()
        {
            GL.Begin(GL.LINES);
            GL.Color(color);

            Vector2 topLeft = new Vector2(center.x - size.x / 2, center.y + size.y / 2);
            Vector2 topRight = new Vector2(center.x + size.x / 2, center.y + size.y / 2);
            Vector2 bottomRight = new Vector2(center.x + size.x / 2, center.y - size.y / 2);
            Vector2 bottomLeft = new Vector2(center.x - size.x / 2, center.y - size.y / 2);

            DrawLine(topLeft, topRight);
            DrawLine(topRight, bottomRight);
            DrawLine(bottomRight, bottomLeft);
            DrawLine(bottomLeft, topLeft);

            DrawLine(topLeft, bottomRight);
            GL.End();
        }

        private void DrawCircle()
        {
            GL.Begin(GL.LINES);
            GL.Color(color);

            DrawCircle(center, radius, 50);

            GL.End();
        }

        private void DrawCircle(Vector2 center, float radius, int segments)
        {
            float angleStep = 2 * Mathf.PI / segments; // Angle entre chaque point
            Vector2 prevPoint = center + new Vector2(radius, 0); // Premier point

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep;
                Vector2 newPoint = center + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);

                GL.Vertex3(prevPoint.x, prevPoint.y, 0);
                GL.Vertex3(newPoint.x, newPoint.y, 0);

                prevPoint = newPoint; // Met à jour le point précédent
            }
        }

        private void DrawCapsule()
        {
            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);

            DrawCapsule(center, height, radius, 50, capsuleDirection);

            GL.End();
        }

        void DrawCapsule(Vector2 center, float height, float radius, int segments, CapsuleDirection2D direction)
        {
            float bodyLength = height - 2 * radius;
            if (bodyLength < 0) bodyLength = 0;

            Vector2 axis = (direction == CapsuleDirection2D.Vertical) ? Vector2.up : Vector2.right;
            Vector2 ortho = (direction == CapsuleDirection2D.Vertical) ? Vector2.right : Vector2.up;

            Vector2 bottom = center - axis * (bodyLength / 2f);
            Vector2 top = center + axis * (bodyLength / 2f);

            for (int i = 0; i <= segments; i++)
            {
                float angle = Mathf.PI + Mathf.PI * i / segments;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 rotated = dir.x * ortho + dir.y * axis;
                GL.Vertex(bottom + rotated * radius);
            }
            GL.Vertex(top + ortho * radius);

            for (int i = 0; i <= segments; i++)
            {
                float angle = Mathf.PI * i / segments;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 rotated = dir.x * ortho + dir.y * axis;
                GL.Vertex(top + rotated * radius);
            }
            GL.Vertex(bottom - ortho * radius);
        }

        void DrawPolygon()
        {
            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);

            DrawPoly();

            GL.End();
        }

        void DrawPoly()
        {
            if (poly == null) return;

            for (int pathIndex = 0; pathIndex < poly.pathCount; pathIndex++)
            {
                Vector2[] points = poly.GetPath(pathIndex);

                if (points.Length < 2) continue;

                for (int i = 0; i < points.Length; i++)
                {
                    Vector2 p1 =  points[i] + center;
                    Vector2 p2 =  points[(i + 1) % points.Length] + center;

                    GL.Vertex(p1);
                    GL.Vertex(p2);
                }
            }
        }

        private void DrawLine(Vector2 start, Vector2 end)
        {
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(end.x, end.y, 0);
        }
    }

    public enum Shape2D
    {
        Square, Circle, Capsule, Polygon,
    }
}


