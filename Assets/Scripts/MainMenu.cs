using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject panelSettings;

    [SerializeField] private Button[] buttonsClickOnce;

    private AudioSource buttonClick;

    private static event Action onButtonPlayOrExitToDestroyEventSystemMainMenu;

    public static Action OnButtonPlayOrExitToDestroyEventSystemMainMenu { get => onButtonPlayOrExitToDestroyEventSystemMainMenu; set => onButtonPlayOrExitToDestroyEventSystemMainMenu = value; }


    void Awake()
    {
        GetComponents();
        SuscribeToGameManagerEvents();
    }

    // Simulacion de Update
    void UpdateMainMenu()
    {
        ChangeSelectedButtonToSettingsPressingCircleB();
    }

    void OnDestroy()
    {
        UnSuscribeToGameManagerEvents();
    }


    public void ButtonPlayGame()
    {
        onButtonPlayOrExitToDestroyEventSystemMainMenu?.Invoke();

        StartCoroutine(LoadSceneAfterButtonClick());
    }

    public void ButtonSettings()
    {
        buttonClick.Play();
        panelSettings.SetActive(true);

        if (DeviceManager.CurrentPlatform == "PC")
        {
            EventSystemMainMenu.OnChangeSelectedButtonToSliderMusic?.Invoke();
        }
    }

    public void ButtonExit()
    {
        onButtonPlayOrExitToDestroyEventSystemMainMenu?.Invoke();

        StartCoroutine(CloseGameAfterClickButton());
    }

    public void ButtonBack()
    {
        buttonClick.Play();
        panelSettings.SetActive(false);

        if (DeviceManager.CurrentPlatform == "PC")
        {
            EventSystemMainMenu.OnChangeSelectedButtonToSettings?.Invoke();
        }
    }


    private void GetComponents()
    {
        buttonClick = GetComponent<AudioSource>();
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

    private void SuscribeToGameManagerEvents()
    {
        GameManager.Instance.OnGameStateMenu += UpdateMainMenu;
    }

    private void UnSuscribeToGameManagerEvents()
    {
        GameManager.Instance.OnGameStateMenu -= UpdateMainMenu;
    }

    private IEnumerator LoadSceneAfterButtonClick()
    {
        buttonClick.Play();
        buttonsClickOnce[0].interactable = false;

        yield return new WaitForSeconds(buttonClick.clip.length);

        SceneManager.LoadSceneAsync("Game");
        GameManager.Instance.ChangeStateTo(GameState.Playing);
    }

    private IEnumerator CloseGameAfterClickButton()
    {
        buttonClick.Play();
        buttonsClickOnce[1].interactable = false;

        yield return new WaitForSeconds(buttonClick.clip.length);

        Application.Quit();
    }
}
