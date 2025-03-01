using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;

public class FinalScreens : MonoBehaviour
{
    [SerializeField] private Button[] buttonsLoseScreen; // 0 = RestarGame, 1 = MainMenu
    private Image[] screens;  // 0 = Win, 1 = Loose
    private AudioSource buttonClick;

    private static event Action onPauseButtonDisabled;

    private static event Action onButtonMainMenuOrPlayAgainToDestroyEventSystemGame;

    public Button[] ButtonsLoseScreen { get => buttonsLoseScreen; set => buttonsLoseScreen = value; }
    public Image[] Screens { get => screens; }

    public static Action OnPauseButtonDisabled { get => onPauseButtonDisabled; set => onPauseButtonDisabled = value; }

    public static Action OnButtonMainMenuOrPlayAgainToDestroyEventSystemGame { get => onButtonMainMenuOrPlayAgainToDestroyEventSystemGame; set => onButtonMainMenuOrPlayAgainToDestroyEventSystemGame = value; }


    void Awake()
    {
        GetComponents();
        SubscribeToPlayerEvents();
    }

    void OnDestroy()
    {
        UnsubscribeToPlayerEvents();    
    }


    public void ButtonRestartGame()
    {
        onButtonMainMenuOrPlayAgainToDestroyEventSystemGame?.Invoke();

        StartCoroutine(LoadSceneAfterButtonClick(buttonsLoseScreen[0], "Game"));

        CursorController.Instance.UnsuscribeCursorControllerToPlayerEvents();
        CursorController.Instance.SuscribeCursorControllerToPlayerEvents();
    }

    public void ButtonMainMenu()
    {
        onButtonMainMenuOrPlayAgainToDestroyEventSystemGame?.Invoke();

        StartCoroutine(LoadSceneAfterButtonClick(buttonsLoseScreen[1], "MainMenu"));
        GameManager.Instance.ChangeStateTo(GameState.Menu);

        CursorController.Instance.UnsuscribeCursorControllerToPlayerEvents();
        CursorController.Instance.SuscribeCursorControllerToPlayerEvents();
    }


    private void GetComponents()
    {
        screens = GetComponentsInChildren<Image>(true);
        buttonClick = GetComponent<AudioSource>();
    }

    private void SubscribeToPlayerEvents()
    {
        PlayerEvents.OnPlayerDefeated += ShowDefeatScreen;
    }

    private void UnsubscribeToPlayerEvents()
    {
        PlayerEvents.OnPlayerDefeated -= ShowDefeatScreen;
    }

    private void ShowWinScreen()
    {
        screens[0].gameObject.SetActive(true);
        onPauseButtonDisabled?.Invoke();
    }

    private void ShowDefeatScreen()
    {
        screens[1].gameObject.SetActive(true);
        onPauseButtonDisabled?.Invoke();
    }

    private IEnumerator LoadSceneAfterButtonClick(Button buttonsClickOnce, string sceneNameToLado)
    {
        buttonClick.Play();
        buttonsClickOnce.interactable = false;

        yield return new WaitForSeconds(buttonClick.clip.length);

        SceneManager.LoadSceneAsync(sceneNameToLado);
    }
}
