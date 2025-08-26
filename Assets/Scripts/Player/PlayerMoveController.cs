using System;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMoveController : MonoBehaviour , InputSystem_Actions.IPlayerActions
{
    [ReadOnly] private Vector2 moveDirection;
    [SerializeField] private float moveSpeed = 5f;
    InputSystem_Actions inputSystemActions;
    void Start()
    {
    }

    private void Update()
    {
        var moveRight = moveDirection.x * transform.right;
        var moveForward = moveDirection.y * transform.forward;
        var moveDelta = (moveRight + moveForward).normalized * moveSpeed * Time.deltaTime;
        transform.Translate(moveDelta, Space.World); ;
    }

    private void OnEnable()
    {
        inputSystemActions = new InputSystem_Actions();
        inputSystemActions.Player.SetCallbacks(instance : this);
        inputSystemActions.Enable();
    }
    
    private void OnDisable()
    {
        inputSystemActions.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
        
    }
}
