using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GameValue<T>
{
    [SerializeField] T value;
    
    public UnityEvent<T,T> OnValueChanged; 

    public T Value
    {
        get { return value; }
        set
        {
            if (this.value.Equals(value))
            {
                OnValueChanged?.Invoke(this.value, value);
            }
            this.value = value;
        }
    }
}