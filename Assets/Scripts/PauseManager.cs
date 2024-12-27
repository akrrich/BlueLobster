using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject panelPause;
    [SerializeField] private GameObject panelSettings;

    [SerializeField] private Button[] buttonsClickOnce;

    private Button buttonPause;
    private AudioSource buttonClick;


    void Awake()
    {
        GetComponents();
    }


    public void ButtonPauseGame()
    {
        Time.timeScale = 0f;

        buttonClick.Play();
        buttonPause.gameObject.SetActive(false);
        panelPause.SetActive(true);
    }

    public void ButtonResumeGame()
    {
        Time.timeScale = 1f;

        buttonClick.Play();
        buttonPause.gameObject.SetActive(true);
        panelPause.SetActive(false);
    }

    public void ButtonSettings()
    {
        buttonClick.Play();
        panelSettings.SetActive(true);
    }

    public void ButtonBack()
    {
        buttonClick.Play();
        panelSettings.SetActive(false);
    }

    public void ButtonMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneAfterButtonClick());
    }

    public void ButtonExit()
    {
        StartCoroutine(CloseGameAfterClickButton());
    }


    private void GetComponents()
    {
        buttonPause = GetComponentInChildren<Button>();
        buttonClick = GetComponent<AudioSource>();
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
}
