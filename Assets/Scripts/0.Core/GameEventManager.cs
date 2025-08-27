using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : SingletonOnlyScene<GameEventManager>
{
    Dictionary<Type,Delegate> eventMap = new Dictionary<Type, Delegate>(); 
    
    public static void RegistListener<Parameter>(Action<Parameter> callback) where Parameter : EventArgs
    {
        if (Instance is null)
        {
            return;
        }

        var key = typeof(Parameter);
        var eventMap = Instance.eventMap;

        if (eventMap.TryGetValue(key, out Delegate callbacks))
        {
            eventMap[key] = Delegate.Combine(callbacks, callback);
        }
        else
        {
            eventMap.Add(key, callback);
        }
    }

    public static void UnregistListener<Parameter>(Action<Parameter> callback) where Parameter : EventArgs
    {
        if (Instance is null)
        {
            return;
        }
        
        var key = typeof(Parameter);
        var eventMap = Instance.eventMap;

        if (eventMap.TryGetValue(key, out Delegate callbacks))
        {
            eventMap[key] = Delegate.Remove(callbacks, callback);
        }
        else
        {
            Debug.LogWarning($"{key}에 리스너가 조회되지 않습니다. 등록여부나 중복 해제를 체크해주세요");
        }
    }

    public static void Publish<Parameter>(Parameter e) where Parameter : EventArgs
    {
        if (Instance is null)
        {
            return;
        }
        
        var key = typeof(Parameter);
        var eventMap = Instance.eventMap;

        if (eventMap.TryGetValue(key, out Delegate callbacks))
        {
            callbacks.DynamicInvoke(e);
        }

    }

    protected override void InitializeSingleton()
    {
    }
}
