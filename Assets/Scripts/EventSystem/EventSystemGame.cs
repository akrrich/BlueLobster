using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class EventSystemGame : MonoBehaviour
{
    private FinalScreens finalScreens;

    private EventSystem eventSystem;
    private AudioSource selectedButton;
    private GameObject lastButtonSelected;

    [SerializeField] private Button[] buttonsPanelPause; // 0 =  Resume, 1 = Settings, 2 = MainMenu, 3 = Exit
    [SerializeField] private GameObject[] optionsSettings; // 0 = SliderMusic, 1 = SliderSFX, 2 = ToggleHZ, 3 = Back

    private static event Action onChangeSelectedButtonToSliderMusic;
    private static event Action onChangeSelectedButtonToSettings;
    private static event Action onChangeSelectedButtonToResume;
    private static event Action onChangeSelectedButtonRestartGame;

    private bool ignoreNextSelectionSound = false;

    public static Action OnChangeSelectedButtonToSliderMusic { get => onChangeSelectedButtonToSliderMusic; set => onChangeSelectedButtonToSliderMusic = value; }
    public static Action OnChangeSelectedButtonToSettings { get => onChangeSelectedButtonToSettings; set => onChangeSelectedButtonToSettings = value; }
    public static Action OnChangeSelectedButtonToResume { get => onChangeSelectedButtonToResume; set => onChangeSelectedButtonToResume = value; }
    public static Action OnChangeSelectedButtonRestartGame { get => onChangeSelectedButtonRestartGame; set => onChangeSelectedButtonRestartGame = value; }


    void Awake()
    {
        DisableScriptIfCurrentDeviceIsMobile();
    }

    void Start()
    {
        GetComponents();
        InitializeSelectedButton();
        SuscribeToGameManagerEvents();
        SuscribeToOwnEvents();
        SuscribeToPlayerEvents();
        SuscribeToPauseManagerEvents();
        SuscribeToFinalScreensEvents();
    }

    // Simulacion de Update
    void UpdateEventSystemGame()
    {
        PlaySelectionSound();
    }

    void OnDestroy()
    {
        UnsuscribeToGameManagerEvents();
        UnsuscribeToOwnEvents();
        UnsuscribeToPlayerEvents();
        UnsuscribeToPauseManagerEvents();
        UnsuscribeToFinalScreensEvents();
    }


    private void DisableScriptIfCurrentDeviceIsMobile()
    {
        if (DeviceManager.CurrentPlatform == "Mobile")
        {
            Destroy(this);
        }
    }

    private void GetComponents()
    {
        finalScreens = FindObjectOfType<FinalScreens>();

        eventSystem = GetComponent<EventSystem>();
        selectedButton = GetComponent<AudioSource>();
    }

    private void InitializeSelectedButton()
    {
        eventSystem.firstSelectedGameObject = buttonsPanelPause[0].gameObject;
        lastButtonSelected = eventSystem.firstSelectedGameObject;
    }

    private void SuscribeToGameManagerEvents()
    {
        GameManager.Instance.OnGameStatePlaying += UpdateEventSystemGame;
    }

    private void UnsuscribeToGameManagerEvents()
    {
        GameManager.Instance.OnGameStatePlaying -= UpdateEventSystemGame;
    }

    private void SuscribeToOwnEvents()
    {
        onChangeSelectedButtonToSliderMusic += ChangeSelectedButtonToSliderMusic;
        onChangeSelectedButtonToSettings += ChangeSelectedButtonToSettings;
        onChangeSelectedButtonToResume += ChangeSelectedButtonToResume;
        onChangeSelectedButtonRestartGame += ChangeSelectedButtonToRestartGameInLooseScreenFromFinalScreens;
    }

    private void UnsuscribeToOwnEvents()
    {
        onChangeSelectedButtonToSliderMusic -= ChangeSelectedButtonToSliderMusic;
        onChangeSelectedButtonToSettings -= ChangeSelectedButtonToSettings;
        onChangeSelectedButtonToResume -= ChangeSelectedButtonToResume;
        onChangeSelectedButtonRestartGame -= ChangeSelectedButtonToRestartGameInLooseScreenFromFinalScreens;
    }

    private void SuscribeToPlayerEvents()
    {
        PlayerEvents.OnPlayerDefeated += ChangeSelectedButtonToRestartGameInLooseScreenFromFinalScreens;
    }

    private void UnsuscribeToPlayerEvents()
    {
        PlayerEvents.OnPlayerDefeated -= ChangeSelectedButtonToRestartGameInLooseScreenFromFinalScreens;
    }

    private void SuscribeToPauseManagerEvents()
    {
        PauseManager.OnButtonMainMenuOrExitToDestroyEventSystemGame += DestroyThisGameObject;
    }

    private void UnsuscribeToPauseManagerEvents()
    {
        PauseManager.OnButtonMainMenuOrExitToDestroyEventSystemGame -= DestroyThisGameObject;
    }

    private void SuscribeToFinalScreensEvents()
    {
        FinalScreens.OnButtonMainMenuOrPlayAgainToDestroyEventSystemGame += DestroyThisGameObject;
    }

    private void UnsuscribeToFinalScreensEvents()
    {
        FinalScreens.OnButtonMainMenuOrPlayAgainToDestroyEventSystemGame -= DestroyThisGameObject;
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
        eventSystem.SetSelectedGameObject(buttonsPanelPause[1].gameObject);
        ignoreNextSelectionSound = true;
    }

    private void ChangeSelectedButtonToResume()
    {
        if (eventSystem.currentSelectedGameObject != buttonsPanelPause[0].gameObject)
        {
            eventSystem.SetSelectedGameObject(buttonsPanelPause[0].gameObject);
            ignoreNextSelectionSound = true;
        }
    }

    private void ChangeSelectedButtonToRestartGameInLooseScreenFromFinalScreens()
    {
        eventSystem.SetSelectedGameObject(finalScreens.ButtonsLoseScreen[0].gameObject);
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
