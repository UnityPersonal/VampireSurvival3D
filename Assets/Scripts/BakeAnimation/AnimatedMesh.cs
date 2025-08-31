using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AnimatedMesh : MonoBehaviour
{
    [SerializeField]
    public AnimatedMeshScriptableObject AnimationSO;
    public MeshFilter Filter;
    private MeshRenderer Renderer;

    [Header("Debug")]
    [SerializeField]
    public int Tick = 1;
    [SerializeField]
    public int AnimationIndex;
    [SerializeField] public string AnimationName;
    [ReadOnly] public List<Mesh> AnimationMeshes;

    public delegate void AnimationEndEvent(string Name);
    public event AnimationEndEvent OnAnimationEnd;

    [ReadOnly] public float LastTickTime;
    
    [ReadOnly] public bool IsAnimating = false;

    private void Awake()
    {
        Filter = GetComponent<MeshFilter>();
        Renderer = GetComponent<MeshRenderer>();
        if(!string.IsNullOrEmpty(AnimationName))
        {
            Play(AnimationName);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void Play(string AnimationName)
    {
        //if (AnimationName != this.AnimationName)
        {
            this.AnimationName = AnimationName;
            Tick = 1;
            AnimationIndex = 0;
            AnimatedMeshScriptableObject.Animation animation = AnimationSO.Animations.Find((item) => item.Name.Equals(AnimationName));
            AnimationMeshes = animation.Meshes;
            if (string.IsNullOrEmpty(animation.Name))
            {
                Debug.LogError($"Animated model {name} does not have an animation baked for {AnimationName}!");
                return;
            }
            
            AnimatedMeshScheduleJob.RegistMehs(this);
            
        }
    }

    /*private void Update()
    {
        if (AnimationMeshes != null)
        {
            if (Time.time >= LastTickTime + (1f / AnimationSO.AnimationFPS))
            {
                Filter.mesh = AnimationMeshes[AnimationIndex];

                AnimationIndex++;
                if (AnimationIndex >= AnimationMeshes.Count)
                {
                    OnAnimationEnd?.Invoke(AnimationName);
                    AnimationIndex = 0;
                }
                LastTickTime = Time.time;
            }
            Tick++;
        }
    }*/
}
