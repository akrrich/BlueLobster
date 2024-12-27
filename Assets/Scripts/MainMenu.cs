using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject panelSettings;

    private AudioSource buttonClick;

    public AudioClip audios;


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

        yield return new WaitForSeconds(buttonClick.clip.length);

        SceneManager.LoadSceneAsync("Game");
    }

    private IEnumerator CloseGameAfterClickButton()
    {
        buttonClick.Play();

        yield return new WaitForSeconds(buttonClick.clip.length);

        Application.Quit();
    }
}
