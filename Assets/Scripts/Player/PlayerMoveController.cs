using System;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMoveController : MonoBehaviour 
{
    [SerializeField] private InputActionAsset inputActionAsset;
    [ReadOnly] private Vector2 moveInputVector;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 180f;

    private InputActionMap playerActionMap;
    private InputAction moveAction;
    
    private Camera mainCamera;

    [ReadOnly,ShowInInspector] private Vector3 rightMoveDirection;
    [ReadOnly,ShowInInspector] private Vector3 forwardMoveDirection;
    
    [SerializeField] Animator animator;

    private void Awake()
    {
        playerActionMap =  inputActionAsset.FindActionMap("Player");
        
        moveAction = playerActionMap.FindAction("Move");
        mainCamera =  Camera.main;
        
        rightMoveDirection = mainCamera.transform.right;
        rightMoveDirection.y = 0;
        rightMoveDirection = rightMoveDirection.normalized;
        
        forwardMoveDirection = mainCamera.transform.forward;
        forwardMoveDirection.y = 0;
        forwardMoveDirection = forwardMoveDirection.normalized;
        
        
    }


    private void Update()
    {
        moveInputVector = moveAction.ReadValue<Vector2>();
        
        var moveRight = moveInputVector.x * rightMoveDirection;
        var moveForward = moveInputVector.y * forwardMoveDirection;
        var toward = (moveRight + moveForward).normalized;
        moveDirection = Vector3.Slerp(moveDirection , toward, Time.deltaTime * rotateSpeed);
        transform.forward = moveDirection;
        
        animator.SetFloat("MoveSpeed", moveInputVector.magnitude);
    }

    [ShowInInspector] private Vector3 moveDirection;
    
    private void FixedUpdate()
    {
        if (moveInputVector != Vector2.zero)
        {
            var moveDelta = moveDirection * (moveSpeed * Time.fixedDeltaTime);
            transform.Translate(moveDelta, Space.World);
        }
        
    }
}
