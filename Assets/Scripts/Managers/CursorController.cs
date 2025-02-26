using UnityEngine;

public class CursorController : MonoBehaviour
{
    private static CursorController instance;


    void Awake()
    {
        DestroyThisGameObjectIfCurrentDeviceISMobile();
        CreateCursorSingleton();
    }

    void Update()
    {
        IsJoystickUsed();
        EnabledAndDisabledCursor();
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

    private void DestroyThisGameObjectIfCurrentDeviceISMobile()
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
            Cursor.visible = true;
        }
    }

    private void IsJoystickUsed()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            Cursor.visible = false;
        }

        if (Input.anyKeyDown)
        {
            Cursor.visible = false;
        }
    }
}
