using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EventSystemMainMenu : MonoBehaviour
{
    private EventSystem eventSystem;
    private AudioSource selectedButton;
    private GameObject lastSelected;

    [SerializeField] private Button[] buttonsMainMenu; // 0 =  Play, 1 = Settings, 2 = Exit
    [SerializeField] private GameObject[] optionsSettings; // 0 = SliderMusic, 1 = SliderSFX, 2 = ToggleHZ, 3 = Back

    private bool ignoreNextSelectionSound = false;


    private static event Action onChangeSelectedButtonToSliderMusic;
    public static Action OnChangeSelectedButtonToSliderMusic { get => onChangeSelectedButtonToSliderMusic; set => onChangeSelectedButtonToSliderMusic = value; }

    private static event Action onChangeSelectedButtonToSettings;
    public static Action OnChangeSelectedButtonToSettings { get => onChangeSelectedButtonToSettings; set => onChangeSelectedButtonToSettings = value; }


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
        eventSystem = GetComponent<EventSystem>();
        selectedButton = GetComponent<AudioSource>();
    }   

    private void InitializeSelectedButton()
    {
        eventSystem.firstSelectedGameObject = buttonsMainMenu[0].gameObject;
        lastSelected = eventSystem.firstSelectedGameObject;
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

        if (currentSelected != null && currentSelected != lastSelected)
        {
            if (!ignoreNextSelectionSound)
            {
                selectedButton.Play();
            }

            else
            {
                ignoreNextSelectionSound = false;
            }

            lastSelected = currentSelected;
        }
    }
}
