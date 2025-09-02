using System;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoveController : MonoBehaviour 
{
    [SerializeField] private InputActionAsset inputActionAsset;
    [ReadOnly] private Vector2 moveInputVector;
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 180f;
    [SerializeField] Animator animator;
    [Required,SerializeField] CharacterController characterController;
    
    private InputActionMap playerActionMap;
    private InputAction moveAction;
    
    private Camera mainCamera;

    [ReadOnly,ShowInInspector] private Vector3 rightMoveDirection;
    [ReadOnly,ShowInInspector] private Vector3 forwardMoveDirection;
    
    [ReadOnly,ShowInInspector] private Vector3 moveDirection;

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

    private void OnEnable()
    {
        playerActionMap.Enable();
        
        
    }

    private void OnDisable()
    {
        playerActionMap.Disable();
    }


    private void Update()
    {
        moveInputVector = moveAction.ReadValue<Vector2>();
    }
    
    private void Rotate()
    {
        if (moveInputVector != Vector2.zero)
        {
            var moveRight = moveInputVector.x * rightMoveDirection;
            var moveForward = moveInputVector.y * forwardMoveDirection;
            var toward = (moveRight + moveForward).normalized;
            
            moveDirection = Vector3.Slerp(moveDirection , toward, Time.fixedDeltaTime * rotateSpeed);
            moveDirection = toward;
            transform.forward = moveDirection;
            
        }
    }

    
    private void FixedUpdate()
    {
        Move();
        Rotate();
    }
    
    private void Move()
    {
        animator.SetFloat("MoveSpeed", moveInputVector.magnitude);
        if (moveInputVector != Vector2.zero)
        {
            var moveDelta = moveDirection * (moveSpeed * Time.fixedDeltaTime);
            //transform.Translate(moveDelta, Space.World);
            characterController.Move(moveDelta);
        }
    }
}
