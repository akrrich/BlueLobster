using UnityEngine;
using UnityEngine.UI;
using System;

public class InputSystem : MonoBehaviour
{
    private Button[] buttonsInput; // 0 = PunchAndHit, 1 = Throw, 2 = PickUp

    private static event Action onPunchAndHit;
    private static event Action onThrow;
    private static event Action onPickUp;

    public static Action OnPunchAndHit { get => onPunchAndHit; set => onPunchAndHit = value; }
    public static Action OnThrow { get => onThrow; set => onThrow = value; }
    public static Action OnPickUp { get => onPickUp; set => onPickUp = value; }


    void Awake()
    {
        GetComponents();
        SuscribeToPlayerEvents();
    }

    void OnDestroy()
    {
        UnsubscribeToPlayerEvents();
    }


    public void ButtonPunchAndHit()
    {
        onPunchAndHit?.Invoke();
    }

    public void ButtonThrow()
    {
        onThrow?.Invoke();
    }

    public void ButtonPickUp()
    {
        OnPickUp?.Invoke();
    }


    private void GetComponents()
    {
        buttonsInput = GetComponentsInChildren<Button>(true);
    }

    private void SuscribeToPlayerEvents()
    {
        PlayerEvents.OnEnabledPickUpButton += EnabledButtonPickUp;
        PlayerEvents.OnDisabledPickUpButton += DisabledButtonPickUp;
    }

    private void UnsubscribeToPlayerEvents()
    {
        PlayerEvents.OnEnabledPickUpButton -= EnabledButtonPickUp;
        PlayerEvents.OnDisabledPickUpButton -= DisabledButtonPickUp;
    }

    private void EnabledButtonPickUp()
    {
        buttonsInput[2].gameObject.SetActive(true);
    }

    private void DisabledButtonPickUp()
    {
        buttonsInput[2].gameObject.SetActive(false);
    }
}
