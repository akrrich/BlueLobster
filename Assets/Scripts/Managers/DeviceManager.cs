using UnityEngine;

public class DeviceManager
{
    private static string currentPlatform;
    public static string CurrentPlatform { get => currentPlatform; }


    public DeviceManager()
    {
        GetCurrentPlatform();
    }

    public static Vector2 GetMovementInput(Vector2 defaultInput)
    {
        if (currentPlatform == "Mobile")
        {
            return new Vector2(defaultInput.x, defaultInput.y);
        }

        else
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            
            return new Vector2(horizontal, vertical);
        }
    }

    public static string GetCurrentPlatform()
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
