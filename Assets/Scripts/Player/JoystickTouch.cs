using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickTouch : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform joystickHandle;
    [SerializeField] private RectTransform joystickBackground;

    private static Vector2 lastDirection;

    public static Vector2 LastDirection { get => lastDirection; }


    void Awake()
    {
        DestroyGameObjectIfCurrentDeviceIsPC();
    }

    void Start()
    {
        StartingValues();
    }


    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position, eventData.pressEventCamera, out position);

        Vector2 direction = position / (joystickBackground.sizeDelta / 2.7f); // valor original = 2
        direction = Vector2.ClampMagnitude(direction, 1);

        joystickHandle.anchoredPosition = direction * (joystickBackground.sizeDelta.x / 2.7f); // valor original = 2

        lastDirection = direction;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickHandle.anchoredPosition = Vector2.zero; 
        lastDirection = Vector2.zero;
    }


    private void DestroyGameObjectIfCurrentDeviceIsPC()
    {
        if (DeviceManager.CurrentPlatform == "PC")
        {
            Destroy(gameObject);
        }
    }

    private void StartingValues()
    {
        joystickHandle.anchoredPosition = Vector2.zero;
        lastDirection = Vector2.zero; 
    }
}
