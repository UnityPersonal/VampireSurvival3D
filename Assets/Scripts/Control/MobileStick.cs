using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

[AddComponentMenu("Input/MobileStick")]
public class MobileStick : OnScreenControl
{
    [SerializeField]
    [Min(0)]
    private float movementRange = 50;

    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string controlPath;
    protected override string controlPathInternal
    {
        get => controlPath;
        set => controlPath = value;
    }
    
}


