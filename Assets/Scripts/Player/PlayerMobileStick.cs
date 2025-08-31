using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

[RequireComponent(typeof(RectTransform))]
public class PlayerMobileStick : MonoBehaviour
{
    [SerializeField, Required] InputActionAsset inputActionAsset;
    [ShowInInspector]InputAction tabOnAction;
    [ShowInInspector]InputAction tabOffAction;
    [ShowInInspector]InputAction tabWhereAction;
    
    RectTransform rectTransform;

    [SerializeField] Canvas canvas;
    [SerializeField,Required] RectTransform parentRectTransform;
    bool isPerformed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var playerMap = inputActionAsset.FindActionMap("Player");
        tabOnAction = playerMap.FindAction("TabOn");
        tabOffAction = playerMap.FindAction("TabOff");
        tabWhereAction = playerMap.FindAction("TabWhere");
    
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tabOnAction.WasPerformedThisFrame())
        {
            isPerformed = true;
            var screenPoint = tabWhereAction.ReadValue<Vector2>();
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPoint, canvas.worldCamera,
                out Vector2 movePos);

            Vector3 mousePos = canvas.transform.TransformPoint(movePos);

            //Set fake mouse Cursor
            transform.position = mousePos;
        }

        if (tabWhereAction.WasPerformedThisFrame())
        {
            isPerformed = false;
        }

        if (isPerformed)
        {
            
        }
    }
}
