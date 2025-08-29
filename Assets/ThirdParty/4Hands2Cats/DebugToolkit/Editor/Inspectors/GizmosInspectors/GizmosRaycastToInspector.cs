using DebugToolkit.Gizmos;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static DebugToolkit.Gizmos.Gizmo_RaycastTo;

namespace UDT.Gizmos.Inspector
{
    [CustomEditor(typeof(Gizmo_RaycastTo))]
    public class GizmosRaycastToInspector : Editor
    {
        private Gizmo_RaycastTo _gizmoRaycast;
        private VisualElement _root;
        private VisualTreeAsset _visualTree;

        private VisualElement _Position;
        private VisualElement _Gameobject;

        private void OnEnable()
        {
            _gizmoRaycast = (Gizmo_RaycastTo)target;

            _root = new VisualElement();
            _visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/4Hands2Cats/DebugToolkit/Editor/Inspectors/GizmosInspectors/RaycastToInspector.uxml");

            StyleSheet ss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/4Hands2Cats/DebugToolkit/Editor/Inspectors/InspectorUss.uss");
            _root.styleSheets.Add(ss);
        }

        public override VisualElement CreateInspectorGUI()
        {
            _root.Clear();
            _visualTree.CloneTree(_root);
            _Position = _root.Q<VisualElement>("Position");
            _Gameobject = _root.Q<VisualElement>("Gameobject");


            var serializedObject = new SerializedObject(_gizmoRaycast);

            var directionField = _root.Q<EnumField>("TargetType");
            directionField.RegisterValueChangedCallback(val => EnableUISection((ERaycastToMode)val.newValue));
            directionField.Bind(serializedObject);
            directionField.bindingPath = "raycastToMode";

            var offsetField = _root.Q<ObjectField>();
            offsetField.Bind(serializedObject);
            offsetField.bindingPath = "target";

            var posField = _root.Q<Vector3Field>("TargetPos");
            posField.Bind(serializedObject);
            posField.bindingPath = "targetPosition";

            var colorField = _root.Q<ColorField>();
            colorField.Bind(serializedObject);
            colorField.bindingPath = "color";

            var drawGizmos = _root.Q<Toggle>();
            drawGizmos.Bind(serializedObject);
            drawGizmos.bindingPath = "drawGizmo";

            EnableUISection(_gizmoRaycast.RaycastToMode);

            return _root;
        }

        private void EnableUISection(ERaycastToMode targetType)
        {
            if (targetType == ERaycastToMode.Position)
            {
                _Position.style.display = DisplayStyle.Flex;
                _Gameobject.style.display = DisplayStyle.None;
            }
            else
            {
                _Gameobject.style.display = DisplayStyle.Flex;
                _Position.style.display = DisplayStyle.None;
            }
        }
    }
}


