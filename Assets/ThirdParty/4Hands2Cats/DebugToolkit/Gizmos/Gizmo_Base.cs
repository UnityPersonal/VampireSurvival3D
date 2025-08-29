using UnityEngine;

namespace DebugToolkit.Gizmos
{
    public class Gizmo_Base : MonoBehaviour
    {
        [SerializeField] protected bool drawGizmo;
        public static bool DrawGizmo { get; set; }
        protected bool _canDrawnGizmos => drawGizmo && DrawGizmo;

        [SerializeField, ColorUsage(true, true)] protected Color color;

        protected static Material lineMaterial;
        protected static void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }
    }
}

