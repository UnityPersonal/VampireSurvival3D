using DebugToolkit.Gizmos;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UDT.Gizmos.Inspector
{
    [CustomEditor(typeof(Gizmo_Collider2D))]
    public class GizmosCollider2DInspector : Editor
    {
        private Gizmo_Collider2D _gizmoCollider2D;
        private VisualElement _root;
        private VisualTreeAsset _visualTree;

        private VisualElement _square;
        private VisualElement _circle;
        private VisualElement _capsule;
        private VisualElement _poly;

        private void OnEnable()
        {
            _gizmoCollider2D = (Gizmo_Collider2D)target;

            _root = new VisualElement();
            _visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/4Hands2Cats/DebugToolkit/Editor/Inspectors/GizmosInspectors/Collider2DInspector.uxml");

            StyleSheet ss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/4Hands2Cats/DebugToolkit/Editor/Inspectors/InspectorUss.uss");
            _root.styleSheets.Add(ss);
        }

        public override VisualElement CreateInspectorGUI()
        {
            _root.Clear();
            _visualTree.CloneTree(_root);
            _square = _root.Q<VisualElement>("Square");
            _circle = _root.Q<VisualElement>("Circle");
            _capsule = _root.Q<VisualElement>("Capsule");
            _poly = _root.Q<VisualElement>("Poly");

            var serializedObject = new SerializedObject(_gizmoCollider2D);

            var shapeField = _root.Q<EnumField>("Shape");
            shapeField.RegisterValueChangedCallback(val => EnableUISection((Shape2D)val.newValue));
            shapeField.Bind(serializedObject);
            shapeField.bindingPath = "shape";

            var offsetField = _root.Q<Vector2Field>("Offset");
            offsetField.Bind(serializedObject);
            offsetField.bindingPath = "center";

            var sizeField = _root.Q<Vector2Field>("Size");
            sizeField.Bind(serializedObject);
            sizeField.bindingPath = "size";

            var radiusField = _circle.Q<FloatField>("Radius");
            radiusField.Bind(serializedObject);
            radiusField.bindingPath = "radius";

            var directionField = _capsule.Q<EnumField>("Direction");
            directionField.Bind(serializedObject);
            directionField.bindingPath = "capsuleDirection";

            radiusField = _capsule.Q<FloatField>("Radius");
            radiusField.Bind(serializedObject);
            radiusField.bindingPath = "radius";

            var heightField = _capsule.Q<FloatField>("Height");
            heightField.Bind(serializedObject);
            heightField.bindingPath = "height";

            var meshField = _poly.Q<ObjectField>("Collider");
            meshField.Bind(serializedObject);
            meshField.bindingPath = "poly";

            var colorField = _root.Q<ColorField>();
            colorField.Bind(serializedObject);
            colorField.bindingPath = "color";

            var drawGizmos = _root.Q<Toggle>();
            drawGizmos.Bind(serializedObject);
            drawGizmos.bindingPath = "drawGizmo";

            EnableUISection(_gizmoCollider2D.Shape);  

            return _root;
        }

        private void EnableUISection(Shape2D shape)
        {
            _square.style.display = DisplayStyle.None;
            _circle.style.display = DisplayStyle.None;
            _capsule.style.display = DisplayStyle.None;
            _poly.style.display = DisplayStyle.None;

            switch (shape)
            {
                case Shape2D.Square:
                    _square.style.display = DisplayStyle.Flex;
                    break;
                case Shape2D.Circle:
                    _circle.style.display = DisplayStyle.Flex;
                    break;
                case Shape2D.Capsule:
                    _capsule.style.display = DisplayStyle.Flex; 
                    break;
                case Shape2D.Polygon:
                    _poly.style.display = DisplayStyle.Flex;
                    break;
            }
        }
    }
}

