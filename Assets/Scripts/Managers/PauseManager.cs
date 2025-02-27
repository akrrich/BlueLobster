using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    private FinalScreens finalScreens;

    [SerializeField] private GameObject panelPause;
    [SerializeField] private GameObject panelSettings;

    [SerializeField] private Button[] buttonsClickOnce;

    private Button buttonPause;
    private AudioSource buttonClick;

    private bool isGamePaused = false;


    void Awake()
    {
        GetComponents();
    }

    void Start()
    {
        DestroyPauseButtonIfCurrentDeviceIsPC();
        SubscribeToFinalScreenEvents();
        SuscribeToGameManagerEvents();
    }

    // Simulacion de Update
    void UpdatePauseManager()
    {
        PauseAndUnPauseGameForDevicePC();
        ChangeSelectedButtonToSettingsPressingCircleB();
    }

    void OnDestroy()
    {
        UnSubscribeToFinalSccreenEvents();
        UnsuscribeToGameManagerEvents();
    }


    public void ButtonPauseGame()
    {
        buttonClick.Play();

        isGamePaused = true;
        Time.timeScale = 0f;

        if (buttonPause != null)
        {
            buttonPause.gameObject.SetActive(false);
        }

        panelPause.SetActive(true);
    }

    public void ButtonResumeGame()
    {
        buttonClick.Play();

        if (DeviceManager.CurrentPlatform == "PC")
        {
            if (!finalScreens.Screens[1].gameObject.activeSelf)
            {
                EventSystemGame.OnChangeSelectedButtonToResume?.Invoke();
            }

            else
            {
                EventSystemGame.OnChangeSelectedButtonRestartGame?.Invoke();
            }
        }

        isGamePaused = false;
        Time.timeScale = 1f;

        if (buttonPause != null)
        {
            buttonPause.gameObject.SetActive(true);
        }

        panelPause.SetActive(false);
    }

    public void ButtonSettings()
    {
        buttonClick.Play();
        panelSettings.SetActive(true);

        if (DeviceManager.CurrentPlatform == "PC")
        {
            EventSystemGame.OnChangeSelectedButtonToSliderMusic?.Invoke();
        }
    }

    public void ButtonBack()
    {
        buttonClick.Play();
        panelSettings.SetActive(false);

        if (DeviceManager.CurrentPlatform == "PC")
        {
            EventSystemGame.OnChangeSelectedButtonToSettings?.Invoke();
        }
    }

    public void ButtonMainMenu()
    {
        GameManager.Instance.ChangeStateTo(GameState.Menu);
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneAfterButtonClick());
    }

    public void ButtonExit()
    {
        StartCoroutine(CloseGameAfterClickButton());
    }


    private void DestroyPauseButtonIfCurrentDeviceIsPC()
    {
        if (DeviceManager.CurrentPlatform == "PC")
        {
            Destroy(buttonPause.gameObject);
        }
    }

    private void GetComponents()
    {
        finalScreens = FindObjectOfType<FinalScreens>();

        buttonPause = GameObject.Find("ButtonPauseGame").GetComponent<Button>();
        buttonClick = GetComponent<AudioSource>();
    }

    private void SuscribeToGameManagerEvents()
    {
        GameManager.Instance.OnGameStatePlaying += UpdatePauseManager;
    }

    private void UnsuscribeToGameManagerEvents()
    {
        GameManager.Instance.OnGameStatePlaying -= UpdatePauseManager;
    }

    private void SubscribeToFinalScreenEvents()
    {
        if (DeviceManager.CurrentPlatform == "Mobile")
        {
            FinalScreens.OnPauseButtonDisabled += DisabledPauseButton;
        }
    }

    private void UnSubscribeToFinalSccreenEvents()
    {
        if (DeviceManager.CurrentPlatform == "Mobile")
        {
            FinalScreens.OnPauseButtonDisabled -= DisabledPauseButton;
        }
    }

    private void ChangeSelectedButtonToSettingsPressingCircleB()
    {
        if (DeviceManager.CurrentPlatform == "PC")
        {
            if (Input.GetButtonDown("Circle/B") && panelSettings.gameObject.activeSelf)
            {
                ButtonBack();
            }
        }
    }

    private void PauseAndUnPauseGameForDevicePC()
    {
        if (DeviceManager.CurrentPlatform == "PC")
        {
            if (finalScreens.Screens[1].gameObject.activeSelf)
            {
                if (PressEscapeOrOptions() && !isGamePaused)
                {
                    ButtonPauseGame();

                    EventSystemGame.OnChangeSelectedButtonToResume?.Invoke();
                }

                else if (Input.GetButtonDown("Circle/B") && isGamePaused && !panelSettings.gameObject.activeSelf)
                {
                    ButtonResumeGame();
                }

                else if (PressEscapeOrOptions() && isGamePaused && !panelSettings.gameObject.activeSelf)
                {
                    ButtonResumeGame();
                }

                else if (PressEscapeOrOptions() && isGamePaused && panelSettings.gameObject.activeSelf)
                {
                    ButtonBack();
                    ButtonResumeGame();
                }
            }

            else
            {
                if (PressEscapeOrOptions() && !isGamePaused)
                {
                    ButtonPauseGame();
                }

                else if (Input.GetButtonDown("Circle/B") && isGamePaused && !panelSettings.gameObject.activeSelf)
                {
                    ButtonResumeGame();
                }

                else if (PressEscapeOrOptions() && isGamePaused && !panelSettings.gameObject.activeSelf)
                {
                    ButtonResumeGame();
                }

                else if (PressEscapeOrOptions() && isGamePaused && panelSettings.gameObject.activeSelf)
                {
                    ButtonBack();
                    ButtonResumeGame();
                }
            }
        }
    }

    public bool PressEscapeOrOptions()
    {
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Options/Settings");
    }

    private IEnumerator LoadSceneAfterButtonClick()
    {
        buttonClick.Play();
        buttonsClickOnce[0].interactable = false;

        yield return new WaitForSeconds(buttonClick.clip.length);

        SceneManager.LoadSceneAsync("MainMenu");
    }

    private IEnumerator CloseGameAfterClickButton()
    {
        buttonClick.Play();
        buttonsClickOnce[1].interactable = false;

        yield return new WaitForSeconds(buttonClick.clip.length);

        Application.Quit();
    }

    private void DisabledPauseButton()
    {
        if (buttonPause != null)
        {
            buttonPause.gameObject.SetActive(false);
        }
    }
}
