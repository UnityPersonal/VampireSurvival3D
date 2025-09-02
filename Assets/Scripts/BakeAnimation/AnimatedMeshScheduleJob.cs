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
        var sigleton = Instance;
        if (sigleton == null)
            return 0;

        return sigleton.RegistMeshImplementation(animatedMesh);
    }

    private uint RegistMeshImplementation(AnimatedMesh animatedMesh)
    {
        if (animatedMeshList.TryGetValue(animatedMesh.AnimatedMeshUID, out var mesh) == false)
        {
            var proxy = new AnimatedMeshProxy(animatedMesh);
            Instance.requestsForRegist.Add(proxy);
            animatedMesh.AnimatedMeshUID = proxy.uid; // align uid 
        }
        
        return animatedMesh.AnimatedMeshUID;
    }

    public static void UnregistMesh(AnimatedMesh animatedMesh)
    {
        var sigleton = Instance;
        if (sigleton == null)
            return;

        sigleton.UnregistMeshImplementation(animatedMesh);  
    }

    private void UnregistMeshImplementation(AnimatedMesh animatedMesh)
    {
        if (animatedMesh.AnimatedMeshUID != 0)
        {
            Instance.requestsForUnregist.Add(animatedMesh.AnimatedMeshUID);
            animatedMesh.AnimatedMeshUID = 0; // reset uid
        }
    }

    
    
    public struct JobLookup
    {
        public uint uid;
        public float lastTickTime;
        public float animationfps;
        public int clipCount;
        public int currentClip;
        public bool isLoop;
    }

    public struct JobResult
    {
        public uint uid;
        public float lastTickTime;
        public int currentClip;
        public bool isCompleted;
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
            bool isCompleted = false;
            
            if (time >= lookup.lastTickTime + (1f / lookup.animationfps))
            {
                resultClip = lookup.currentClip + 1;
                if (resultClip >= lookup.clipCount)
                {
                    if (lookup.isLoop)
                    {
                        resultClip = 0;
                    }
                    else
                    {
                        isCompleted = true;
                    }
                }
                lastTickTime = time;
            }

            results[index] = new JobResult
            {
                uid = uid,
                lastTickTime = lastTickTime,
                currentClip = resultClip,
                isCompleted = isCompleted,
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
                currentClip = anim.AnimationIndex,
                isLoop = anim.isLooping
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

            if (result.isCompleted)
            {
                UnregistMeshImplementation(proxy.animatedMesh);
                continue;
            }
            
            var anim = proxy.animatedMesh;
            anim.AnimationIndex = result.currentClip;
            anim.MeshFilter.mesh = anim.AnimationMeshes[result.currentClip];
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

