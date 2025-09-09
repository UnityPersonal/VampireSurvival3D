using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AnimatedMesh : MonoBehaviour
{
    private static int uidgen = 1;
    private int uid = 0;
    [SerializeField]
    public AnimatedMeshScriptableObject AnimationSO;
    [HideInInspector] public MeshFilter MeshFilter;
    private MeshRenderer Renderer;

    [Header("Debug")]
    [SerializeField]
    public int AnimationIndex;
    [SerializeField] public string AnimationName;
    [ReadOnly] public List<Mesh> AnimationMeshes;

    AnimatedMeshScriptableObject.Animation currentAnimation;

    [ReadOnly] public float LastTickTime;
    public bool isLooping = false;
    
    public delegate void AnimationEndEvent(string Name);
    public AnimationEndEvent OnAnimationEnd;
    
    private void Awake()
    {
        uid  = uidgen++;
        gameObject.name = $"AnimatedMesh_{uid.ToString()}";
        MeshFilter = GetComponent<MeshFilter>();
        Renderer = GetComponent<MeshRenderer>();
    }

    private uint animatedMeshUID = 0;
    public uint AnimatedMeshUID { get => animatedMeshUID; set => animatedMeshUID = value; }

    private void OnDisable()
    {
       //AnimatedMeshScheduleJob.UnregistMesh(this);
    }

    private bool isPlaying = false;
    public void Play(string AnimationName)
    {
        this.AnimationName = AnimationName;
        LastTickTime = Time.time;
        AnimationIndex = 0;
        currentAnimation = AnimationSO.Animations.Find((item) => item.Name.Equals(AnimationName));

        if (string.IsNullOrEmpty(currentAnimation.Name))
        {
            Debug.LogError($"Animated model {name} does not have an animation baked for {AnimationName}!");
            isPlaying = false;
            return;
        }
            
        AnimationMeshes = currentAnimation.Meshes;
        isLooping = currentAnimation.loop;
        
        isPlaying  = true;
    }

    private void Update()
    {
        if (!isPlaying) return;
        
        if (Time.time >= LastTickTime + (1f / AnimationSO.AnimationFPS))
        {
            AnimationIndex++;
            if (AnimationIndex >= AnimationMeshes.Count)
            {
                if (isLooping)
                {
                    AnimationIndex = 0;
                }
                else
                {
                    AnimationIndex = AnimationMeshes.Count - 1;
                    OnAnimationEnd?.Invoke(AnimationName);
                    isPlaying = false;
                }
            }
            LastTickTime = Time.time;
        }
        MeshFilter.mesh = AnimationMeshes[AnimationIndex];
    }

}
