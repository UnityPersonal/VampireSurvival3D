using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;

public class AnimatedMeshScheduleJob : Singleton<AnimatedMeshScheduleJob>
{
    AnimatedMesh animatedMesh;
    public List<AnimatedMesh> animatedMeshList = new List<AnimatedMesh>();
    JobHandle handle;

    public static void RegistMehs(AnimatedMesh animatedMesh)
    {
        if (Instance == null)
            return;
        Instance.animatedMeshList.Add(animatedMesh);
    }

    public struct JobLookup
    {
        public float lastTickTime;
        public float animationfps;
        public int clipCount;
        public int currentClip;
    }

    public struct JobResult
    {
        public float lastTickTime;
        public int currentClip;
    }
    
    public struct AnimatedMeshJob : IJobParallelFor
    {
        public float time;
        public NativeArray<JobLookup> lookups;
        public NativeArray<JobResult> results;
        public void Execute(int index)
        {
            var lookup = lookups[index];
            
            int resultClip = lookup.currentClip;
            float lastTickTime = lookup.lastTickTime;
            
            if (time >= lookup.lastTickTime + (1f / lookup.animationfps))
            {
                resultClip = lookup.currentClip + 1;
                if (resultClip >= lookup.clipCount)
                {
                    resultClip = 0;
                }
                lastTickTime = time;
            }

            results[index] = new JobResult
            {
                lastTickTime = lastTickTime,
                currentClip = resultClip
            };
        }
    }

    private NativeArray<JobLookup> lookups;
    private NativeArray<JobResult> results;
    
    void Update()
    {
        lookups = new NativeArray<JobLookup>(animatedMeshList.Count, Allocator.TempJob);
        results = new NativeArray<JobResult>(animatedMeshList.Count, Allocator.TempJob);

        for (int i = 0; i < animatedMeshList.Count; i++)
        {
            var anim = animatedMeshList[i];
            lookups[i] = new JobLookup
            {
                lastTickTime = anim.LastTickTime,
                animationfps = anim.AnimationSO.AnimationFPS,
                clipCount = anim.AnimationMeshes.Count,
                currentClip = anim.AnimationIndex
            };
        }
        
        AnimatedMeshJob animatedMeshJob = new AnimatedMeshJob
        {
            time = Time.time,
            lookups = lookups,
            results = results,
            
        };
        
        // If this job required a previous job to complete before it could safely begin execution,
        // we'd use its handle here. For this simple case, there are no job dependencies,
        // so a default JobHandle is sufficient.
        JobHandle dependencyJobHandle = default;
        
        handle = animatedMeshJob.ScheduleByRef(animatedMeshList.Count, 128, dependencyJobHandle);
        
    }

    private void LateUpdate()
    {
        handle.Complete();

        for (int i = 0; i < results.Length; i++)
        {
            var anim = animatedMeshList[i];
            var result = results[i];
            anim.AnimationIndex = result.currentClip;
            anim.Filter.mesh = anim.AnimationMeshes[result.currentClip];
            anim.LastTickTime = result.lastTickTime;
        }
        
        lookups.Dispose();
        results.Dispose();
    }
}
