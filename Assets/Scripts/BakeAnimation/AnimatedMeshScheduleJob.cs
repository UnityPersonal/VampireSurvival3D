using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;

public class AnimatedMeshScheduleJob : SingletonOnlyScene<AnimatedMeshScheduleJob>
{
    List<AnimatedMeshProxy> requestsForRegist = new List<AnimatedMeshProxy>();
    List<uint> requestsForUnregist = new List<uint>();
    
    public Dictionary<uint, AnimatedMeshProxy> animatedMeshList = new();
    JobHandle handle;
    
    public class AnimatedMeshProxy
    {
        private static uint uidGenerator = 1;
        public uint uid;
        public bool isReleased;
        public AnimatedMesh animatedMesh;
        public AnimatedMeshProxy(AnimatedMesh animatedMesh)
        {
            isReleased = false;
            uid = uidGenerator++;
            this.animatedMesh = animatedMesh;
        }
    }

    public static uint RegistMesh(AnimatedMesh animatedMesh)
    {
        if (Instance == null)
            return 0;
        
        var proxy = new AnimatedMeshProxy(animatedMesh);
        Instance.requestsForRegist.Add(proxy);
        
        return proxy.uid;
    }

    public static void UnregistMesh(uint uid)
    {
        if (Instance == null)
            return;
        
        Instance.requestsForUnregist.Add(uid);
    }

    
    
    public struct JobLookup
    {
        public uint uid;
        public float lastTickTime;
        public float animationfps;
        public int clipCount;
        public int currentClip;
    }

    public struct JobResult
    {
        public uint uid;
        public float lastTickTime;
        public int currentClip;
    }
    
    [BurstCompile]
    public struct AnimatedMeshJob : IJobParallelFor
    {
        public float time;
        [ReadOnly]
        public NativeArray<JobLookup> lookups;
        [WriteOnly]
        public NativeArray<JobResult> results;
        public void Execute(int index)
        {
            var lookup = lookups[index];
            uint uid =  lookup.uid;
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
                uid = uid,
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

        var jobList = animatedMeshList.Values.ToArray();

        for (int i = 0; i < jobList.Length; i++)
        {
            var proxy = jobList[i];
            var anim = proxy.animatedMesh;
            lookups[i] = new JobLookup
            {
                uid = proxy.uid,
                lastTickTime = anim.LastTickTime,
                animationfps = anim.AnimationSO.AnimationFPS ,
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

        requestsForUnregist.ForEach(request=> animatedMeshList[request].isReleased = true);
        for (int i = 0; i < results.Length; i++)
        {
            var result = results[i];
            var proxy =animatedMeshList[result.uid];
            
            if(proxy.isReleased) continue;
            
            var anim = proxy.animatedMesh;
            anim.AnimationIndex = result.currentClip;
            anim.Filter.mesh = anim.AnimationMeshes[result.currentClip];
            anim.LastTickTime = result.lastTickTime;
        }
        
        requestsForUnregist.ForEach(request=> animatedMeshList.Remove(request));
        requestsForUnregist.Clear();
        
        requestsForRegist.ForEach(proxy => animatedMeshList[proxy.uid] = proxy);
        requestsForRegist.Clear();
        
        lookups.Dispose();
        results.Dispose();
    }
    
    void OnDisable()
    {
        SafeCompleteAndDispose();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        SafeCompleteAndDispose();
    }

    protected override void InitializeSingleton()
    {
    }

    void OnDestroy()
    {
        SafeCompleteAndDispose();
    }

    void SafeCompleteAndDispose()
    {
        try
        {
            if (handle.IsCompleted == false) handle.Complete();
        }
        catch { /* 이미 끝난 상태면 무시 */ }

        if (lookups.IsCreated) lookups.Dispose();
        if (results.IsCreated) results.Dispose();

        requestsForRegist.Clear();
        requestsForUnregist.Clear();
        animatedMeshList.Clear();
    }
}

