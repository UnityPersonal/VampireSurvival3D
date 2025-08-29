using DebugToolkit.Gizmos;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UDT.Gizmos.Inspector
{
    [CustomEditor(typeof(Gizmo_Collider))]
    public class GizmosColliderInspector : Editor
    {
        private Gizmo_Collider _gizmoCollider;
        private VisualElement _root;
        private VisualTreeAsset _visualTree;

        private VisualElement _box;
        private VisualElement _sphere;
        private VisualElement _capsule;
        private VisualElement _mesh;

        private void OnEnable()
        {
            _gizmoCollider = (Gizmo_Collider)target;

            _root = new VisualElement();
            _visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/4Hands2Cats/DebugToolkit/Editor/Inspectors/GizmosInspectors/ColliderInspector.uxml");

            StyleSheet ss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/4Hands2Cats/DebugToolkit/Editor/Inspectors/InspectorUss.uss");
            _root.styleSheets.Add(ss);
        }

        public override VisualElement CreateInspectorGUI()
        {
            _root.Clear();
            _visualTree.CloneTree(_root);
            _box = _root.Q<VisualElement>("Box");
            _sphere = _root.Q<VisualElement>("Sphere");
            _capsule = _root.Q<VisualElement>("Capsule");
            _mesh = _root.Q<VisualElement>("Mesh");

            var serializedObject = new SerializedObject(_gizmoCollider);

            var shapeField = _root.Q<EnumField>("Shape");
            shapeField.RegisterValueChangedCallback(val => EnableUISection((Shape)val.newValue));
            shapeField.Bind(serializedObject);
            shapeField.bindingPath = "shape";

            var offsetField = _root.Q<Vector3Field>("Offset");
            offsetField.Bind(serializedObject);
            offsetField.bindingPath = "center";

            var sizeField = _root.Q<Vector3Field>("Size");
            sizeField.Bind(serializedObject);
            sizeField.bindingPath = "size";

            var radiusField = _sphere.Q<FloatField>("Radius");
            radiusField.Bind(serializedObject);
            radiusField.bindingPath = "radius";

            radiusField = _capsule.Q<FloatField>("Radius");
            radiusField.Bind(serializedObject);
            radiusField.bindingPath = "radius";

            var heightField = _capsule.Q<FloatField>("Height");
            heightField.Bind(serializedObject);
            heightField.bindingPath = "height";

            var meshField = _mesh.Q<ObjectField>("Collider");
            meshField.Bind(serializedObject);
            meshField.bindingPath = "mesh";

            var colorField = _root.Q<ColorField>();
            colorField.Bind(serializedObject);
            colorField.bindingPath = "color";

            var drawGizmos = _root.Q<Toggle>();
            drawGizmos.Bind(serializedObject);
            drawGizmos.bindingPath = "drawGizmo";

            EnableUISection(_gizmoCollider.Shape);  

            return _root;
        }

        private void EnableUISection(Shape shape)
        {
            _box.style.display = DisplayStyle.None;
            _sphere.style.display = DisplayStyle.None;
            _capsule.style.display = DisplayStyle.None;
            _mesh.style.display = DisplayStyle.None;

            switch (shape)
            {
                case Shape.Box:
                    _box.style.display = DisplayStyle.Flex;
                    break;
                case Shape.Sphere:
                    _sphere.style.display = DisplayStyle.Flex;
                    break;
                case Shape.Capsule:
                    _capsule.style.display = DisplayStyle.Flex; 
                    break;
                case Shape.Mesh:
                    _mesh.style.display = DisplayStyle.Flex;
                    break;
            }
        }
    }
}

