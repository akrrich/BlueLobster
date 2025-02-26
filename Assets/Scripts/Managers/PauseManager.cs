using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
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
        isGamePaused = true;
        Time.timeScale = 0f;

        buttonClick.Play();

        if (buttonPause != null)
        {
            buttonPause.gameObject.SetActive(false);
        }

        panelPause.SetActive(true);
    }

    public void ButtonResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;

        buttonClick.Play();

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
        buttonPause = GetComponentInChildren<Button>();
        buttonClick = GetComponent<AudioSource>();
    }

    private void ChangeSelectedButtonToSettingsPressingCircleB()
    {
        if (Input.GetButtonDown("Circle/B") && DeviceManager.CurrentPlatform == "PC")
        {
            ButtonBack();
        }
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

    private void PauseAndUnPauseGameForDevicePC()
    {
        if (DeviceManager.CurrentPlatform == "PC")
        {
            if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Options/Settings")) && !isGamePaused)
            {
                ButtonPauseGame();
            }

            else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Options/Settings")) && isGamePaused)
            {
                ButtonResumeGame();
            }
        }
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
