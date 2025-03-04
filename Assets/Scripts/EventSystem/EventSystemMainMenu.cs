using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class EventSystemMainMenu : MonoBehaviour
{
    private EventSystem eventSystem;
    private AudioSource selectedButton;
    private GameObject lastButtonSelected;

    [SerializeField] private Button[] buttonsMainMenu; // 0 =  Play, 1 = Settings, 2 = Exit
    [SerializeField] private GameObject[] optionsSettings; // 0 = SliderMusic, 1 = SliderSFX, 2 = ToggleHZ, 3 = Back

    private bool ignoreNextSelectionSound = false;

    private static event Action onChangeSelectedButtonToSliderMusic;
    private static event Action onChangeSelectedButtonToSettings;

    public static Action OnChangeSelectedButtonToSliderMusic { get => onChangeSelectedButtonToSliderMusic; set => onChangeSelectedButtonToSliderMusic = value; }
    public static Action OnChangeSelectedButtonToSettings { get => onChangeSelectedButtonToSettings; set => onChangeSelectedButtonToSettings = value; }


    void Awake()
    {
        SlectInputModule();
        GetComponents();
        InitializeSelectedButton();
        SuscribeToGameManagerEvents();
        SuscribeToOwnEvents();
        SuscribeToMainMenuButtonsEvents();
    }

    // Simulacion de Update
    void UpdateEventSystemMainMenu()
    {
        PlaySelectionSound();
    }

    void OnDestroy()
    {
        UnsuscribeToGameManagerEvents();
        UnsuscribeToOwnEvents();
        UnsuscribeToMainMenuButtonsEvents();
    }


    private void SlectInputModule()
    {
        if (DeviceManager.CurrentPlatform == "PC")
        {
            return;
        }

        else
        {
            // destruir el nuevo input system y el script adjunto
            InputSystemUIInputModule inputSystemUIInputModule = GetComponent<InputSystemUIInputModule>();
            Destroy(inputSystemUIInputModule);
            Destroy(this);

            // agregar el input system viejo
            StandaloneInputModule standaloneInputModule = gameObject.AddComponent<StandaloneInputModule>();
        }
    }

    private void GetComponents()
    {
        eventSystem = GetComponent<EventSystem>();
        selectedButton = GetComponent<AudioSource>();
    }   

    private void InitializeSelectedButton()
    {
        eventSystem.firstSelectedGameObject = buttonsMainMenu[0].gameObject;
        lastButtonSelected = eventSystem.firstSelectedGameObject;
    }

    private void SuscribeToGameManagerEvents()
    {
        GameManager.Instance.OnGameStateMenu += UpdateEventSystemMainMenu;
    }

    private void UnsuscribeToGameManagerEvents()
    {
        GameManager.Instance.OnGameStateMenu -= UpdateEventSystemMainMenu;
    }

    private void SuscribeToOwnEvents()
    {
        onChangeSelectedButtonToSliderMusic += ChangeSelectedButtonToSliderMusic;
        onChangeSelectedButtonToSettings += ChangeSelectedButtonToSettings;
    }

    private void UnsuscribeToOwnEvents()
    {
        onChangeSelectedButtonToSliderMusic -= ChangeSelectedButtonToSliderMusic;
        onChangeSelectedButtonToSettings -= ChangeSelectedButtonToSettings;
    }

    private void SuscribeToMainMenuButtonsEvents()
    {
        MainMenu.OnButtonPlayOrExitToDestroyEventSystemMainMenu += DestroyThisGameObject;
    }

    private void UnsuscribeToMainMenuButtonsEvents()
    {
        MainMenu.OnButtonPlayOrExitToDestroyEventSystemMainMenu -= DestroyThisGameObject;
    }

    private void DestroyThisGameObject()
    {
        Destroy(gameObject);
    }

    private void ChangeSelectedButtonToSliderMusic()
    {
        eventSystem.SetSelectedGameObject(optionsSettings[0].gameObject);
        ignoreNextSelectionSound = true;
    }

    private void ChangeSelectedButtonToSettings()
    {
        eventSystem.SetSelectedGameObject(buttonsMainMenu[1].gameObject);
        ignoreNextSelectionSound = true;
    }

    private void PlaySelectionSound()
    {
        GameObject currentSelected = eventSystem.currentSelectedGameObject;

        if (currentSelected != null && currentSelected != lastButtonSelected)
        {
            if (!ignoreNextSelectionSound)
            {
                selectedButton.Play();
            }

            else
            {
                ignoreNextSelectionSound = false;
            }

            lastButtonSelected = currentSelected;
        }
    }
}
