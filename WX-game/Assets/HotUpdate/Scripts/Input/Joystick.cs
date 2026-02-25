using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class Joystick : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform joystickBase;   // 底座
    [SerializeField] private RectTransform joystickHandle; // 摇杆
    [SerializeField] private float handleRange = 50f;      // 摇杆可移动范围
    
    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string m_ControlPath;
    
    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
    
    private Vector2 input = Vector2.zero;
    private Canvas canvas;
    private Camera cam;
    
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;
    }
    
    // 每帧调用，返回单位向量
    public Vector2 GetInputDirection()
    {
        return input.magnitude > 0.1f ? input.normalized : Vector2.zero;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (joystickBase == null) return;
        
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBase, 
            eventData.position, 
            cam, 
            out localPoint
        );
        
        float distance = Mathf.Clamp(localPoint.magnitude, 0f, handleRange);
        Vector2 direction = localPoint.normalized;
        
        if (joystickHandle != null)
            joystickHandle.anchoredPosition = direction * distance;
        
        input = direction * (distance / handleRange);
        
        // 发送输入到新输入系统
        SendValueToControl(input);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (joystickHandle != null)
            joystickHandle.anchoredPosition = Vector2.zero;
        
        input = Vector2.zero;
        
        // 发送输入到新输入系统
        SendValueToControl(input);
    }
}
