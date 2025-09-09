using System;
using System.Collections.Generic;
using Unity.AppUI.UI;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem.LowLevel;
using AnimationMode = UnityEditor.AnimationMode;

public class VATTool : EditorWindow
{
    private const int MAX_TEXTURE_SIZE = 4096;

    private VATBakeCommandSO vatBakeCommandSO;
    private float minSamplingRate = 60.0f;
    private bool powerOfTwo = true;

    private bool hasResults = false;
    private Texture2D results_texture;
    private Vector2 results_bounds;
    
    private const string BASE_PATH = "Assets/02.VAT/";
    private const string DEFAULT_NAME = "VAT_Animation";

    private string animationName = DEFAULT_NAME;

    [UnityEditor.MenuItem("Tools/Vertex Animation Texture Tool")]
    static void Init()
    {
        VATTool window = (VATTool)GetWindow(typeof(VATTool));
        window.Show();
        
    }

    private void OnGUI()
    {
        animationName = EditorGUILayout.TextField("Animation Name", animationName);
        
        vatBakeCommandSO = (VATBakeCommandSO)EditorGUILayout.ObjectField("BakeCommand",vatBakeCommandSO, typeof(VATBakeCommandSO), false);
        EditorGUILayout.Space();
        minSamplingRate = EditorGUILayout.FloatField("Sampling rate (per sec.)", minSamplingRate);
        powerOfTwo = EditorGUILayout.Toggle("Power of two", powerOfTwo);

        GUI.enabled =
            vatBakeCommandSO &&
            vatBakeCommandSO.isValid && 
            minSamplingRate > 0;

        if (GUILayout.Button("Generate"))
        {
            EditorCoroutineUtility.StartCoroutine(GenerateTexture(), this);
        }

        GUI.enabled = true;


        if (hasResults)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Last results:");
            EditorGUI.indentLevel++;
            EditorGUILayout.ObjectField("Asset: ", results_texture, typeof(Texture), false);
            EditorGUILayout.Vector2Field("Bounds: ", results_bounds);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }
    
    public System.Collections.IEnumerator GenerateTexture()
    {
        var animatedGameObject = vatBakeCommandSO.animatedGameObject;
        var skinnedMeshRenderer = vatBakeCommandSO.skinnedMeshRenderer;
        Vector3[] defaultVertexPositions = skinnedMeshRenderer.sharedMesh.vertices; //the vertex positions when the mesh is not animated
        int textureHeight = defaultVertexPositions.Length;
        if (powerOfTwo)
            textureHeight = GetNearestPowerOfTwo(textureHeight);
        
        if (textureHeight > MAX_TEXTURE_SIZE) // vertex 사이즈 최대 텍스처 사이즈보다 작아야 한다.
        {
            EditorUtility.DisplayDialog(
                "Error",
                string.Format("Vertices count of {0} exceeds the max texture size ({1})",
                    skinnedMeshRenderer.sharedMesh.name, MAX_TEXTURE_SIZE),
                "OK");

            yield break;
        }
        
        AnimationMode.StartAnimationMode();

        byte[] pngData = null;
        Texture2D texture = null;
        string path = string.Empty;
        int textureWidth = 0;
        
        var so = CreateInstance<VATAnimationSO>();
        so.clips = new VATAnimationSO.VATClip[VATAnimationSO.CLIP_COUNT];
        var bounds = new Vector2(float.PositiveInfinity,
            float.NegativeInfinity); //minimum and maximum x, y or z values of each vertex positions, bounds.x is min / bounds.y is max
        try
        {
            AssetDatabase.StartAssetEditing();
            var clips = vatBakeCommandSO.clips;
            
            for (int i = 0; i < clips.Length; i++)
            {
                var clip = clips[i];
                var clipTextureWidth = Mathf.CeilToInt(clip.length * minSamplingRate);
                textureWidth += clipTextureWidth;
            }
            
            if (powerOfTwo)
                textureWidth = GetNearestPowerOfTwo(textureWidth);

            // validate texture size
            if (textureWidth > MAX_TEXTURE_SIZE || textureHeight > MAX_TEXTURE_SIZE)
            {
                string msg = string.Format(
                    "Animation clip is too long to be sampled at {0}FPS for a max texture size of {1}!",
                    minSamplingRate, MAX_TEXTURE_SIZE);
                EditorUtility.DisplayDialog("Error",
                    msg, "OK");
                throw new Exception();
            }
            
            // create texture
            texture = new Texture2D(textureWidth,
                textureHeight,
                TextureFormat.RGBA32,
                false);

            Mesh bakedMesh = new Mesh(); //we need to bake the skinned mesh to a regular mesh in order to get its vertex positions on each frame
            
            Undo.RegisterFullObjectHierarchyUndo(animatedGameObject,
                "Sample animation"); //remember the current "pose" of the gameobject to be animated, horrible but necessary

            List<Vector3> tmpVPos = new List<Vector3>(); //tmp list to store the vertex positions of the baked mesh
            
            int currentIndex = 0;
            
            Vector3[][] frames = new Vector3[textureWidth][];
            
            for (int i = 0; i < clips.Length; i++)
            {
                var clip = clips[i];
                
                var clipTextureWidth = Mathf.CeilToInt(clip.length * minSamplingRate);
                var clipBeginIndex = currentIndex;
                currentIndex += clipTextureWidth;
               
                for (int x = 0; x < clipTextureWidth; x++)
                {
                    float t = (x / (float)clipTextureWidth) * clip.length;

                    clip.SampleAnimation(animatedGameObject, t);
                    skinnedMeshRenderer.BakeMesh(bakedMesh, false);
                    bakedMesh.GetVertices(tmpVPos);

                    for (int y = 0; y < tmpVPos.Count; y++)
                    {
                        tmpVPos[y] -=
                            defaultVertexPositions
                                [y]; //get the offset of the vertex position on THIS frame from its "default" position when the mesh is still

                        bounds.x = Mathf.Min(bounds.x, tmpVPos[y].x, tmpVPos[y].y, tmpVPos[y].z);
                        bounds.y = Mathf.Max(bounds.y, tmpVPos[y].x, tmpVPos[y].y, tmpVPos[y].z);
                    }

                    frames[clipBeginIndex + x] = tmpVPos.ToArray();
                }
                
                Undo.PerformUndo(); //reset the animated pose, i hate this
            }


            so.bound = bounds;
            currentIndex = 0;
            for (int i = 0; i < clips.Length; i++)
            {
                var clip = clips[i];
                
                var clipTextureWidth = Mathf.CeilToInt(clip.length * minSamplingRate);
                var clipBeginIndex = currentIndex;
                currentIndex += clipTextureWidth;

                for (int x = 0; x < clipTextureWidth; x++)
                {
                    var frame = frames[clipBeginIndex + x];
                    
                    for (int y = 0; y < frame.Length; y++)
                    {
                        var vpos = frame[y];
                        Color col = new Color(
                            Mathf.InverseLerp(bounds.x, bounds.y, vpos.x),
                            Mathf.InverseLerp(bounds.x, bounds.y, vpos.y),
                            Mathf.InverseLerp(bounds.x, bounds.y, vpos.z)
                        );

                        texture.SetPixel(clipBeginIndex + x, y, col);
                    } // vertices
                } // clip frame

                so.clips[i] = new VATAnimationSO.VATClip(
                    clipBeginIndexInTexture: clipBeginIndex,
                    clipLength:clipTextureWidth,
                    duration:clip.length,
                    originalClip:clip
                    );
            }

            texture.Apply();
            
            

            path = EditorUtility.SaveFilePanelInProject("Save Texture", animationName, "png",
                "Select destination");

            if (string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("Error", "Path is invalid!", "OK");
                throw new Exception();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AnimationMode.StopAnimationMode();
        }
        
        pngData = texture.EncodeToPNG();
        

        if (pngData != null)
        {
            System.IO.File.WriteAllBytes(path, pngData);
            AssetDatabase.Refresh();

            yield return new EditorWaitForSeconds(0.5f);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Default;
            importer.textureShape = TextureImporterShape.Texture2D;
            importer.sRGBTexture = false;
            importer.alphaSource = TextureImporterAlphaSource.None;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = Mathf.RoundToInt(Mathf.Max(GetNearestPowerOfTwo(textureWidth), GetNearestPowerOfTwo(textureHeight)));
            importer.npotScale = powerOfTwo ? TextureImporterNPOTScale.ToNearest : TextureImporterNPOTScale.None;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
            AssetDatabase.Refresh();

            yield return new EditorWaitForSeconds(0.5f); // wait for asset update

            hasResults = true;
            results_bounds = bounds;
            results_texture = (Texture2D)AssetDatabase.LoadMainAssetAtPath(path);
            
            so.clipTexture = results_texture;
            var soPath = $"{BASE_PATH}{animationName}.asset";
            Debug.Log($"Creating assset : {soPath} (animations={so.clips.Length})");
            AssetDatabase.CreateAsset(so, soPath);
            EditorUtility.SetDirty(so);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            
            yield return new EditorWaitForSeconds(0.5f); // wait for asset update
            

        }

    }

    private int GetNearestPowerOfTwo(int x)
    {
        if (x < 0) { return 0; }
        --x;
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        return x + 1;
    }
}
