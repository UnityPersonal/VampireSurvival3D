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


    [ReadOnly] public float LastTickTime;
    public bool isLooping = false;
    
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
        AnimatedMeshScheduleJob.UnregistMesh(this);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void Play(string AnimationName)
    {
        //if (AnimationName != this.AnimationName)
        {
            this.AnimationName = AnimationName;
            LastTickTime = 0;
            AnimationIndex = 0;
            AnimatedMeshScriptableObject.Animation animation = AnimationSO.Animations.Find((item) => item.Name.Equals(AnimationName));
            AnimationMeshes = animation.Meshes;
            isLooping = animation.loop;
            if (string.IsNullOrEmpty(animation.Name))
            {
                Debug.LogError($"Animated model {name} does not have an animation baked for {AnimationName}!");
                return;
            }

            if(animatedMeshUID == 0)
                AnimatedMeshScheduleJob.RegistMesh(this);
        }
    }

}
