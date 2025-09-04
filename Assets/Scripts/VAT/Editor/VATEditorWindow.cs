using System;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class VATEditorWindow : EditorWindow
{
    [MenuItem("Tools/VAT Editor")]
    public static void CreateEditorWindow()
    {
        var window = EditorWindow.GetWindow<VATEditorWindow>();
        window.titleContent = new GUIContent("VAT Editor");
    }

    private GameObject AnimatedModel;
    private int AnimationFPS = 30;
    private string Name;

    private const string BASE_PATH = "Assets/VAT/";

    private AnimationClip AnimationClip;

    void OnGUI()
    {
        var newAnimatedModel = (GameObject)EditorGUILayout.ObjectField("Animated Model", AnimatedModel, typeof(GameObject), true);
        var newClip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", AnimationClip, typeof(AnimationClip), true);
        if (newAnimatedModel != AnimatedModel && newAnimatedModel != null)
        {
            Name = newAnimatedModel.name + " VAT";
        }
        
        AnimatedModel = newAnimatedModel;
        
        var animator = AnimatedModel ? AnimatedModel.GetComponent<Animator>() : null;
        
        Name = EditorGUILayout.TextField("Name", Name);
        AnimationFPS = EditorGUILayout.IntSlider("Animation FPS", AnimationFPS, 1, 120);


        int vertexCount = 0;
        if (AnimatedModel && animator&& animator.runtimeAnimatorController)
        {
            var smr =AnimatedModel.transform.GetComponentInChildren<SkinnedMeshRenderer>();
            var mesh = smr.sharedMesh;
            vertexCount = mesh.vertexCount;
        }
        GUILayout.TextField($"vertex Count : {vertexCount}");
        
        int frameCount = 0;
        if (newClip)
        {
            AnimationClip = newClip;
            // 타이밍/프레임 계산
            float dt = 1f / Mathf.Max(1, AnimationFPS);
            float length = Mathf.Max(newClip.length, dt);
            // 양 끝 포함을 위해 +1 (프레임 0, 마지막 프레임 포함)
            frameCount = Mathf.CeilToInt(length / dt) + 1;
        }
        GUILayout.TextField($"clip sample Cout : {frameCount}");
        
        
        GUI.enabled = (AnimatedModel && animator && animator.runtimeAnimatorController && newClip);
        if (GUILayout.Button("Generate VAT"))
        {
            if(string.IsNullOrEmpty(Name)) Name = "VAT";
            GenerateFolderPaths($"{BASE_PATH}{Name}");
            EditorCoroutineUtility.StartCoroutine(GenerateVAT(animator), this);
        }
    }

    
    
    private void GenerateFolderPaths(string fullPath)
    {
        var parts = fullPath.Split('/');
        var path = "";
        for (int i = 0; i < parts.Length; i++)
        {
            if (string.IsNullOrEmpty(parts[i])) continue;
            path = (i == 0) ? parts[i] : $"{path}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parent = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
                var folder = System.IO.Path.GetFileName(path);
                if (!string.IsNullOrEmpty(parent) && AssetDatabase.IsValidFolder(parent))
                    AssetDatabase.CreateFolder(parent, folder);
            }
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
    
    private IEnumerator GenerateVAT(Animator animator)
    {
        if (AnimatedModel == null) yield break;

        AnimationMode.StartAnimationMode();

        // 모든 SMR가 화면 밖에서도 업데이트되도록
        var smrs = AnimatedModel.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        var prevUpdateOffscreen = new bool[smrs.Length];
        for (int i = 0; i < smrs.Length; i++)
        {
            prevUpdateOffscreen[i] = smrs[i].updateWhenOffscreen;
            smrs[i].updateWhenOffscreen = true;
        }

        var prevCulling = animator.cullingMode;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        var clip = AnimationClip;
        var parentFolder = $"{BASE_PATH}{Name}/";
        GenerateFolderPaths(parentFolder);

        try
        {
            AssetDatabase.StartAssetEditing();

            // 메인 SMR 기준 (필요하면 다중 SMR 합성 로직으로 확장)
            var smr = AnimatedModel.transform.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr == null || smr.sharedMesh == null)
            {
                Debug.LogError("SkinnedMeshRenderer 또는 sharedMesh가 없습니다.");
                yield break;
            }

            int vertexCount = smr.sharedMesh.vertexCount;
            if (vertexCount <= 0)
            {
                Debug.LogError("vertexCount가 0입니다.");
                yield break;
            }
            
            var textureWidth = GetNearestPowerOfTwo(vertexCount);

            // 타이밍/프레임 계산
            float dt = 1f / Mathf.Max(1, AnimationFPS);
            float length = Mathf.Max(clip.length, dt);
            // 양 끝 포함을 위해 +1 (프레임 0, 마지막 프레임 포함)
            int frameCount = Mathf.CeilToInt(length / dt) + 1;

            // VAT 텍스처 생성 (float 정밀도)
            var tex = new Texture2D(
                width: textureWidth,
                height: frameCount,
                textureFormat: TextureFormat.RGBAFloat,
                mipChain: false,
                linear: true
            );
            
            
            
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.anisoLevel = 0;
            tex.name = $"{Name}_Positions";

            // 행(=프레임) 단위로 SetPixels
            var row = new Color[vertexCount];

            var bakedMesh = new Mesh();
    #if UNITY_2021_2_OR_NEWER
            bakedMesh.indexFormat = smr.sharedMesh.indexFormat; // 큰 메쉬 대응
    #endif

            for (int f = 0; f < frameCount; f++)
            {
                float t = Mathf.Min(f * dt, clip.length); // 오버샘플 방지
                EditorUtility.DisplayProgressBar("Generate VAT", $"Sampling frame {f+1}/{frameCount}", (float)f / frameCount);

                AnimationMode.BeginSampling();
                AnimationMode.SampleAnimationClip(AnimatedModel, clip, t);
                AnimationMode.EndSampling();

    #if UNITY_2020_2_OR_NEWER
                smr.BakeMesh(bakedMesh, true); // scale 고려
    #else
                smr.BakeMesh(bakedMesh);
    #endif
                var vertices = bakedMesh.vertices;
                if (vertices == null || vertices.Length != vertexCount)
                {
                    Debug.LogError($"프레임 {f}: vertexCount 불일치. 기대 {vertexCount}, 실제 {vertices?.Length ?? 0}");
                    bakedMesh.Clear();
                    break;
                }

                // 로컬 공간 값 저장 (필요 시 월드 변환: var v = smr.localToWorldMatrix.MultiplyPoint3x4(vertices[i]);)
                for (int i = 0; i < vertexCount; i++)
                {
                    var v = vertices[i];
                    row[i] = new Color(v.x, v.y, v.z, 1f);
                }

                tex.SetPixels(0, f, vertexCount, 1, row);

                // 프레임 사이 살짝 양보해서 에디터 프리즈 방지
                if ((f & 7) == 0) yield return null;
            }

            tex.Apply(updateMipmaps: false, makeNoLongerReadable: false);
            bakedMesh.Clear();

            // EXR로 저장 (float 유지)
            string exrPath = $"{parentFolder}{Name}_Positions.exr";
            var exrBytes = tex.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat);
            File.WriteAllBytes(exrPath, exrBytes);

            // 임포트 설정
            AssetDatabase.ImportAsset(exrPath, ImportAssetOptions.ForceSynchronousImport);
            var importer = (TextureImporter)AssetImporter.GetAtPath(exrPath);
            if (importer != null)
            {
                importer.maxTextureSize = 4096;
                importer.textureType = TextureImporterType.Default;
                importer.sRGBTexture = false; // Linear
                importer.mipmapEnabled = false;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }

            // 메모리 텍스처도 프로젝트에 저장(선택사항): 에셋으로 직접 저장하고 싶다면 아래 사용
            // AssetDatabase.CreateAsset(tex, $"{parentFolder}{Name}_Positions.asset");

            Debug.Log($"VAT Position Texture saved: {exrPath}\nSize: {vertexCount}x{frameCount}, FPS: {AnimationFPS}");
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            EditorUtility.ClearProgressBar();

            // 원복
            animator.cullingMode = prevCulling;
            for (int i = 0; i < smrs.Length; i++)
                smrs[i].updateWhenOffscreen = prevUpdateOffscreen[i];

            AnimationMode.StopAnimationMode();
        }
    }

}
