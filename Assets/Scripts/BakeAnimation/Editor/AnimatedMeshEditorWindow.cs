using System.Collections.Generic;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations; // if needed

public class AnimatedMeshEditorWindow : EditorWindow
{
    [MenuItem("Tools/Animated Mesh Creator")]
    public static void CreateEditorWindow()
    {
        var window = GetWindow<AnimatedMeshEditorWindow>();
        window.titleContent = new GUIContent("Animated Mesh Editor");
    }

    private GameObject AnimatedModel;
    private int AnimationFPS = 30;
    private string Name;
    private bool Optimize = false; // consider removing; deprecated
    private bool DryRun = false;

    private const string BASE_PATH = "Assets/Animated Models/";

    void OnGUI()
    {
        var newAnimatedModel = (GameObject)EditorGUILayout.ObjectField("Animated Model", AnimatedModel, typeof(GameObject), true);
        if (newAnimatedModel != AnimatedModel && newAnimatedModel != null)
            Name = newAnimatedModel.name + " animations";

        AnimatedModel = newAnimatedModel;
        var animator = AnimatedModel ? AnimatedModel.GetComponentInChildren<Animator>() : null;

        Name = EditorGUILayout.TextField("Name", Name);
        AnimationFPS = EditorGUILayout.IntSlider("Animation FPS", AnimationFPS, 1, 100);
        Optimize = EditorGUILayout.Toggle("Optimize (deprecated)", Optimize);
        DryRun = EditorGUILayout.Toggle("Dry Run", DryRun);

        GUI.enabled = (AnimatedModel && animator && animator.runtimeAnimatorController);
        if (GUILayout.Button("Generate ScriptableObjects"))
        {
            if (string.IsNullOrEmpty(Name)) Name = "AnimatedMesh";
            GenerateFolderPaths(BASE_PATH + Name);
            // 한 번만 코루틴 실행
            EditorCoroutineUtility.StartCoroutine(GenerateModels(animator, DryRun), this);
        }
        GUI.enabled = true;

        if (GUILayout.Button("Clear progress bar"))
            EditorUtility.ClearProgressBar();
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

    private System.Collections.IEnumerator GenerateModels(Animator animator, bool dryRun)
    {
        // 안전장치: 에디터 샘플링 모드 진입
        AnimationMode.StartAnimationMode();

        // 오프스크린 업데이트 강제
        foreach (var smr in AnimatedModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            smr.updateWhenOffscreen = true;

        // 애니메이터 컷팅 무시
        var prevCulling = animator.cullingMode;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        var so = CreateInstance<AnimatedMeshScriptableObject>();
        so.AnimationFPS = AnimationFPS;

        var clips = animator.runtimeAnimatorController.animationClips;
        var parentFolder = $"{BASE_PATH}{Name}/";

        int clipIndex = 0;

        try
        {
            AssetDatabase.StartAssetEditing();

            foreach (var clip in clips)
            {
                clipIndex++;
                var msg = $"Processing animation {clip.name} ({clipIndex} / {clips.Length})";
                Debug.Log(msg);
                EditorUtility.DisplayProgressBar("Processing Animations", msg, clipIndex / (float)clips.Length);

                var meshes = new List<Mesh>();
                var anim = new AnimatedMeshScriptableObject.Animation { Name = clip.name };

                float dt = 1f / AnimationFPS;
                float tMax = Mathf.Max(clip.length, dt); // 방어적

                // 애니메이션 샘플링 루프
                for (float t = 0f; t < tMax; t += dt)
                {
                    // 에디터 전용 정확 샘플링
                    AnimationMode.BeginSampling();
                    AnimationMode.SampleAnimationClip(AnimatedModel, clip, t);
                    AnimationMode.EndSampling();

                    // 바로 베이크
                    foreach (var smr in AnimatedModel.GetComponentsInChildren<SkinnedMeshRenderer>())
                    {
                        var mesh = new Mesh();
#if UNITY_2021_2_OR_NEWER
                        smr.BakeMesh(mesh, true); // scale 적용
#else
                        smr.BakeMesh(mesh);
#endif
                        if (Optimize)
                        {
#if UNITY_2020_2_OR_NEWER
                            // Optimize()는 deprecated — 필요하면 아래 두 개로 대체
                            MeshUtility.Optimize(mesh);
#else
                            mesh.Optimize();
#endif
                        }

                        if (!dryRun)
                        {
                            var clipFolder = parentFolder + clip.name;
                            if (!AssetDatabase.IsValidFolder(clipFolder))
                            {
                                var parent = System.IO.Path.GetDirectoryName(clipFolder).Replace("\\", "/");
                                var folder = System.IO.Path.GetFileName(clipFolder);
                                AssetDatabase.CreateFolder(parent, folder);
                            }
                            var assetPath = $"{clipFolder}/{t:F4}.asset";
                            AssetDatabase.CreateAsset(mesh, assetPath);
                        }
                        meshes.Add(mesh);
                    }

                    if (dryRun)
                        yield return new EditorWaitForSeconds(dt);
                }

                anim.Meshes = meshes;
                so.Animations.Add(anim);
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AnimationMode.StopAnimationMode();
            animator.cullingMode = prevCulling;
            EditorUtility.ClearProgressBar();
        }

        if (!dryRun)
        {
            var soPath = $"{BASE_PATH}{Name}.asset";
            Debug.Log($"Creating asset: {soPath} (animations={so.Animations.Count}, meshes={so.Animations.Sum(a=>a.Meshes.Count)})");
            AssetDatabase.CreateAsset(so, soPath);
            EditorUtility.SetDirty(so);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
