using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public abstract class RangedGameValue<T> where T : IComparable<T>, IEquatable<T>
{
    [SerializeField] protected T value;
    /// <summary>
    /// (T oldValue, T newValue, T maxValue)
    /// </summary>
    public UnityEvent<T,T, T> OnValueChanged;
    
    [SerializeField] protected T maxValue;
    public T MaxValue {
        get { return maxValue; }
        set
        {
            maxValue = value;
            OnValueChanged?.Invoke(this.value, this.value, this.maxValue);
        }
    }
    public UnityEvent OnOverflow;

    public virtual T Value
    {
        get { return this.value; }
        set
        {
            var old = this.value;
            this.value = value;
            if (this.value.CompareTo(this.maxValue) >= 0)
            {
                ToRange();
                OnOverflow?.Invoke();
            }
            
            OnValueChanged?.Invoke(old, this.value, this.maxValue);
        }
    }

    public abstract void ToRange();

}