#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DebugToolkit.Editor
{
    [InitializeOnLoad]
    public class WelcomePopup : EditorWindow
    {
        private const string DontShowAgainKey = "UDT_Welcome";
        private bool _dontShowAgain;

        [MenuItem("Tools/UDT/Welcome")]
        private static void Open()
        {
            OpenWindow();
        }

        [InitializeOnLoadMethod]
        private static void ShowOnFirstOpen()
        {
            if (!EditorPrefs.HasKey(DontShowAgainKey))
            {
                OpenWindow();
            }
        }

        public static void OpenWindow()
        {
            WelcomePopup window = GetWindow<WelcomePopup>("Welcome");
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 250);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Welcome to UDT", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Thank you for using UDT. Here's how to get started:");
            EditorGUILayout.Space();

            if (GUILayout.Button("Open Documentation"))
            {
                Application.OpenURL("https://4hands2cats.github.io/UnityDebugToolkitDocumentation/#/");
            }

            EditorGUILayout.Space();
            GUILayout.FlexibleSpace();

            // Checkbox for "Don't Show Me Again"
            _dontShowAgain = EditorGUILayout.ToggleLeft("Don't show this again", _dontShowAgain);

            EditorGUILayout.Space();

            if (GUILayout.Button("Close"))
            {
                if (_dontShowAgain)
                {
                    EditorPrefs.SetBool(DontShowAgainKey, true);
                }
                Close();
            }
        }
    }
}

#endif