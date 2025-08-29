using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace DebugToolkit.Gizmos
{
    public class Gizmo_Raycast : Gizmo_Base
    {
        [SerializeField] private Direction direction;
        [SerializeField] private Vector3 customDirection;
        public Vector3 CustomDirection { get { return customDirection; } set { customDirection = value; } }
        [SerializeField, Range(1, 1000)] private float distance;
        public Direction Direction => direction;

        public void OnRenderObject()
        {
            if (_canDrawnGizmos)
            {
                CreateLineMaterial();
                lineMaterial.SetPass(0);
                GL.PushMatrix();
                GL.MultMatrix(transform.localToWorldMatrix);
                GL.Begin(GL.LINES);
                GL.Color(color);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(ReturnDir(direction).x * distance,
                    ReturnDir(direction).y * distance,
                    ReturnDir(direction).z * distance);
                GL.End();

                GL.PopMatrix();
            }
        }

        private Vector3 ReturnDir(Direction direction)
        {
            switch (direction)
            {
                case Direction.Custom: return customDirection;
                case Direction.Forward: return customDirection = transform.forward;
                case Direction.Backward: return customDirection = -transform.forward;
                case Direction.Right: return customDirection = transform.right;
                case Direction.Left: return customDirection = -transform.right;
                case Direction.Up: return customDirection = transform.up;
                case Direction.Down: return customDirection = -transform.up;
                default: return Vector3.zero;
            }
        }
    }

    public enum Direction
    {
        Custom, Forward, Backward, Left, Right, Up, Down,
    }
}

