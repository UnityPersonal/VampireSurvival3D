using DebugToolkit.Gizmos;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace UDT.Gizmos.Inspector
{
    [CustomEditor(typeof(Gizmo_Raycast))]
    public class GizmosRaycastInspector : Editor
    {
        private Gizmo_Raycast _gizmoRaycast;
        private VisualElement _root;
        private VisualTreeAsset _visualTree;

        private VisualElement _custom;

        private void OnEnable()
        {
            _gizmoRaycast = (Gizmo_Raycast)target;

            _root = new VisualElement();
            _visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/4Hands2Cats/DebugToolkit/Editor/Inspectors/GizmosInspectors/RaycastInspector.uxml");

            StyleSheet ss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/4Hands2Cats/DebugToolkit/Editor/Inspectors/InspectorUss.uss");
            _root.styleSheets.Add(ss);
        }

        public override VisualElement CreateInspectorGUI()
        {
            _root.Clear();
            _visualTree.CloneTree(_root);
            _custom = _root.Q<VisualElement>("Custom");


            var serializedObject = new SerializedObject(_gizmoRaycast);

            var directionField = _root.Q<EnumField>("Direction");
            directionField.RegisterValueChangedCallback(val => EnableUISection((Direction)val.newValue));
            directionField.Bind(serializedObject);
            directionField.bindingPath = "direction";

            var offsetField = _root.Q<Vector3Field>("Direction");
            offsetField.Bind(serializedObject);
            offsetField.bindingPath = "customDirection";

            var distanceField = _root.Q<Slider>();
            distanceField.Bind(serializedObject);
            distanceField.bindingPath = "distance";

            var colorField = _root.Q<ColorField>();
            colorField.Bind(serializedObject);
            colorField.bindingPath = "color";

            var drawGizmos = _root.Q<Toggle>();
            drawGizmos.Bind(serializedObject);
            drawGizmos.bindingPath = "drawGizmo";

            EnableUISection(_gizmoRaycast.Direction);

            return _root;
        }

        private void EnableUISection(Direction dir)
        {
            if (dir == Direction.Custom)
            {
                _custom.style.display = DisplayStyle.Flex;
            }
            else
            {
                _custom.style.display = DisplayStyle.None;
            }
        }
    }
}


