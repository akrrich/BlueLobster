using UnityEngine;

public class DeviceManager
{
    private static string currentPlatform;
    public static string CurrentPlatform { get => currentPlatform; }


    public DeviceManager()
    {
        GetCurrentPlatform();
    }

    public static Vector2 GetMovementInput()
    {
        if (currentPlatform == "Mobile")
        {
            return new Vector2(JoystickTouch.LastDirection.x, JoystickTouch.LastDirection.y);
        }

        else
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            
            return new Vector2(horizontal, vertical);
        }
    }

    public static bool GetButtonCircleB()
    {
        return Input.GetButtonDown("Circle/B");
    }

    public static bool PressEscapeOrOptions()
    {
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Options/Settings");
    }

    public static bool GetRightClickOrCircleB()
    {
        int rightClick = 1;

        return Input.GetMouseButtonDown(rightClick) || Input.GetButtonDown("Circle/B");
    }

    public static bool GetLeftClickOrSquareX()
    {
        int leftClick = 0;

        return Input.GetMouseButtonDown(leftClick) || Input.GetButtonDown("Square/X");
    }


    private static string GetCurrentPlatform()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                currentPlatform = "Mobile";
                break;

            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.LinuxPlayer:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.LinuxEditor:
                currentPlatform = "PC";
                break;
        }

        return currentPlatform;
    }
}
