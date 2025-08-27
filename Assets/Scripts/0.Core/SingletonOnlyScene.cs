using System;
using UnityEngine;

public abstract class SingletonOnlyScene<T> : MonoBehaviour where T : MonoBehaviour
{
    static bool isApplicationQuitting = false;
    static T instance;
    
    public static T Instance
    {
        get
        {
            if (isApplicationQuitting)
            {
                Debug.Log($"게임이 중단되어 싱글턴을 더이상 찾을 수 없습니다.");
                return null;
            }
            
            if (instance is null)
            {
                instance = FindAnyObjectByType<T>();
                if (instance is null) // scene 존재하지 않는 경우
                {
                    Debug.LogError($"싱글톤 {typeof(T).Name}의 인스턴스를 찾을 수 없습니다. 사용 전 반드시 씬에 오브젝트를 추가해주세요");
                }
                else
                {
                    var derivedInstacne = instance as SingletonOnlyScene<T>;
                    derivedInstacne.InitializeSingleton();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance is null)
        {
            instance = this as T;
            InitializeSingleton();
        }
        else if(!this.Equals(instance))
        {
            Debug.LogWarning($"싱글톤 오브젝트가 중복됩니다. 반드시 하나만 사용해주세요");
            Destroy(gameObject);
        }
    }

    protected virtual void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }

    protected abstract void InitializeSingleton();
}
