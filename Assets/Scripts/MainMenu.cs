using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject panelSettings;

    [SerializeField] private Button[] buttonsClickOnce;

    private AudioSource buttonClick;


    void Start()
    {
        GetComponents();
    }


    public void ButtonPlayGame()
    {
        StartCoroutine(LoadSceneAfterButtonClick());
    }

    public void ButtonSettings()
    {
        buttonClick.Play();
        panelSettings.SetActive(true);
    }

    public void ButtonExit()
    {
        StartCoroutine(CloseGameAfterClickButton());
    }

    public void ButtonBack()
    {
        buttonClick.Play();
        panelSettings.SetActive(false);
    }


    private void GetComponents()
    {
        buttonClick = GetComponent<AudioSource>();
    }

    private IEnumerator LoadSceneAfterButtonClick()
    {
        buttonClick.Play();
        buttonsClickOnce[0].interactable = false;

        yield return new WaitForSeconds(buttonClick.clip.length);

        SceneManager.LoadSceneAsync("Game");
    }

    private IEnumerator CloseGameAfterClickButton()
    {
        buttonClick.Play();
        buttonsClickOnce[1].interactable = false;

        yield return new WaitForSeconds(buttonClick.clip.length);

        Application.Quit();
    }
}
