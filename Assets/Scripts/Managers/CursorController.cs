using UnityEngine;

public class CursorController : MonoBehaviour
{
    private static CursorController instance;

    private bool isOnLooseScreen = false;

    public static CursorController Instance { get =>  instance; }


    void Awake()
    {
        DestroyThisGameObjectIfCurrentDeviceIsMobile();
        CreateCursorSingleton();
        SuscribeCursorControllerToPlayerEvents();
    }

    void Update()
    {
        EnabledAndDisabledCursor();
    }


    public void SuscribeCursorControllerToPlayerEvents()
    {
        PlayerEvents.OnPlayerDefeated += SetTrueOnLooseScreen;
    }

    public void UnsuscribeCursorControllerToPlayerEvents()
    {
        PlayerEvents.OnPlayerDefeated -= SetTrueOnLooseScreen;
        SetFalseOnLooseScreen();    
    }


    private void CreateCursorSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void DestroyThisGameObjectIfCurrentDeviceIsMobile()
    {
        if (DeviceManager.CurrentPlatform == "Mobile")
        {
            Destroy(gameObject);
        }
    }

    private void EnabledAndDisabledCursor()
    {
        if (GameManager.Instance.GameState == GameState.Menu)
        {
            IsJoystickUsed();
            IsMouseAndKeyboardUsed();
        }

        if (GameManager.Instance.GameState == GameState.Playing)
        {
            if (Time.timeScale == 0f)
            {
                IsJoystickUsed();
                IsMouseAndKeyboardUsed();
            }

            else
            {
                if (isOnLooseScreen)
                {
                    IsJoystickUsed();
                    IsMouseAndKeyboardUsed();
                }

                else
                {
                    Cursor.visible = false;
                }
            }
        }
    }

    private void SetTrueOnLooseScreen()
    {
        isOnLooseScreen = true;
    }

    private void SetFalseOnLooseScreen()
    {
        isOnLooseScreen = false;
    }

    private void IsJoystickUsed()
    {
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKey((KeyCode)((int)KeyCode.JoystickButton0 + i)))
            {
                Cursor.visible = false;
                return;
            }
        }

        if (Mathf.Abs(Input.GetAxis("Joystick Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Joystick Vertical")) > 0.1f)
        {
            Cursor.visible = false;
            return;
        }
    }

    private void IsMouseAndKeyboardUsed()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            Cursor.visible = true;
            return;
        }

        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (key >= KeyCode.A && key <= KeyCode.Z ||        // Letras
                key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9 || // Números superiores
                key >= KeyCode.Keypad0 && key <= KeyCode.Keypad9 || // Números del teclado numérico
                key >= KeyCode.F1 && key <= KeyCode.F12 || // Teclas de función
                key == KeyCode.Space || key == KeyCode.Return || key == KeyCode.Backspace || // Espacio, Enter, Borrar
                key == KeyCode.Tab || key == KeyCode.Escape || key == KeyCode.LeftControl || key == KeyCode.RightControl ||
                key == KeyCode.LeftShift || key == KeyCode.RightShift || key == KeyCode.LeftAlt || key == KeyCode.RightAlt ||
                key == KeyCode.UpArrow || key == KeyCode.DownArrow || key == KeyCode.LeftArrow || key == KeyCode.RightArrow || // Flechas
                key == KeyCode.BackQuote) // Tecla al lado del 1 (tilde)
            {
                if (Input.GetKeyDown(key))
                {
                    Cursor.visible = true;
                    return;
                }
            }
        }
    }
}
