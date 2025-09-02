using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem.Layouts;

public class PlayerMobileController : OnScreenControl
{
    [SerializeField] private RectTransform joystickUI;

    private Vector2 touchStartPos;

    [SerializeField] private InputActionAsset inputActionAsset;
    
    [Header("Input")]
    [SerializeField] private InputActionReference positionActionRef; // <Pointer>/position, Vector2
    [SerializeField] private InputActionReference pressActionRef;    // <Pointer>/press, Button
    
    [Header("UI")]
    [SerializeField] private RectTransform joystickRoot; // 조이스틱 루트 (배경 원 포함)
    [SerializeField] private RectTransform knob;         // 스틱 손잡이(선택)
    [SerializeField] private Canvas canvas;              // 조이스틱이 있는 캔버스
    
    [Header("Logic")]
    [SerializeField] private float knobRadius = 120f;    // 손잡이 최대 이동 반경(px)
    
    
    private InputAction _posAction;
    private InputAction _pressAction;
    private Vector2 _startScreenPos;
    private bool _pressed;


    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string stickControlPath;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        _posAction   = positionActionRef.action;
        _pressAction = pressActionRef.action;
            
        _posAction.Enable();
        _pressAction.Enable();

        
    }


    protected override void OnDisable()
    {
        base.OnDisable();
       
        _posAction.Disable();
        _pressAction.Disable();
    }
    
   
    protected override string controlPathInternal
    {
        get => stickControlPath;
        set => stickControlPath = value;
    }

    private void OnPressStarted()
    {
        _pressed = true;

        // 포인터(터치/마우스) 현재 화면 좌표
        _startScreenPos = _posAction.ReadValue<Vector2>();

        // UI를 해당 위치로 이동 + 활성화
        MoveRectToScreenPoint(joystickRoot, _startScreenPos);
        joystickRoot.gameObject.SetActive(true);

        if (knob) knob.anchoredPosition = Vector2.zero; // 손잡이 리셋
    }

    private void OnPressCanceled()
    {
        _pressed = false;
        joystickRoot.gameObject.SetActive(false);
        
        SendValueToControl(Vector2.zero);
        
    }

    private void Update()
    {
        if( _pressAction.WasPressedThisFrame())
        {
            OnPressStarted();
        }
        else if(_pressAction.WasReleasedThisFrame())
        {
            OnPressCanceled();
        }

        if (!_pressed) return;

        Vector2 currentScreenPos = _posAction.ReadValue<Vector2>();

        // 드래그 벡터(화면 좌표계)
        Vector2 deltaScreen = currentScreenPos - _startScreenPos;

        // 조이스틱 루트의 로컬 좌표계에서 손잡이 위치 갱신
        if (knob)
        {
            // 화면좌표 → 캔버스로컬 → 조이스틱 로컬 보정
            Vector2 startLocal = ScreenToLocal(joystickRoot, _startScreenPos);
            Vector2 curLocal   = ScreenToLocal(joystickRoot, currentScreenPos);
            Vector2 deltaLocal = curLocal - startLocal;

            
            // 반경 제한
            Vector2 clamped = Vector2.ClampMagnitude(deltaLocal, knobRadius);
            knob.anchoredPosition = clamped;
        }

        // 여기서 이동 입력 벡터를 계산해 캐릭터에 전달하면 됩니다.
        Vector2 move = knob.anchoredPosition / knobRadius; // -1~1 범위
        move = move.sqrMagnitude > 1 ? move.normalized : move;
        
        SendValueToControl(move);
    }

    private void MoveRectToScreenPoint(RectTransform rect, Vector2 screenPos)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;

        // 캔버스 모드에 맞춰 카메라 전달:
        // - Screen Space Overlay: camera = null
        // - Screen Space Camera/World: camera = canvas.worldCamera
        Camera cam = canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, cam, out Vector2 localPoint);

        rect.anchoredPosition = localPoint;
    }
    
   
    private Vector2 ScreenToLocal(RectTransform rect, Vector2 screenPos)
    {
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect, screenPos, cam, out Vector2 localPoint);
        return localPoint;
    }
}
