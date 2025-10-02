
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[Serializable]
public class Status<T>
{
    [SerializeField] private string StatusName;
    [SerializeField] private T defaultValue;
    [SerializeField] private T Value;
    
    [SerializeField] private UnityEvent<T> OnChanged;
}