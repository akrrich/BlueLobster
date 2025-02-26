using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;

public class FinalScreens : MonoBehaviour
{
    [SerializeField] private Button[] buttonsLose; // 0 = RestarGame, 1 = MainMenu
    private Image[] screens;  // 0 = Win, 1 = Loose
    private AudioSource buttonClick;

    private static event Action onPauseButtonDisabled;

    public Button[] ButtonsLose { get => buttonsLose; set => buttonsLose = value; }

    public static Action OnPauseButtonDisabled { get => onPauseButtonDisabled; set => onPauseButtonDisabled = value; }


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
        StartCoroutine(LoadSceneAfterButtonClick(buttonsLose[0], "Game"));
    } 

    public void ButtonMainMenu()
    {
        StartCoroutine(LoadSceneAfterButtonClick(buttonsLose[1], "MainMenu"));
        GameManager.Instance.ChangeStateTo(GameState.Menu);
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
