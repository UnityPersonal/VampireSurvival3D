using System;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GameValue<T> where T : IComparable<T>, IEquatable<T>
{
    [SerializeField] protected T value;
    
    // (oldvalue , newvalue)
    public UnityEvent<T,T> OnValueChanged; 

    public virtual T Value
    {
        get { return value; }
        set
        {
            var old = this.value;
            this.value = value;
            if (!old.Equals(this.value))
            {
                OnValueChanged?.Invoke(old, this.value);
            }
        }
    }
}
