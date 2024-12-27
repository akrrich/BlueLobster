using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSOption : MonoBehaviour
{
    [SerializeField] private Toggle FPSToggle;

    private AudioSource buttonClick;

    private const string FPS_KEY = "SelectedFPS";


    void Awake()
    {
        InitializeToggle();
        StartCoroutine(GetAudioComponentAfterOneSecond());
    }


    public void OnToggleValueChanged(bool isOn)
    {
        if (buttonClick != null)
        {
            buttonClick.Play();
        }

        int selectedFPS;
        isOn = FPSToggle.isOn;

        if (isOn)
        {
            selectedFPS = 120;
        }

        else 
        {
            selectedFPS = 60;
        }

        QualitySettings.vSyncCount = 0; 
        Application.targetFrameRate = selectedFPS;

        PlayerPrefs.SetInt(FPS_KEY, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }


    private void InitializeToggle()
    {
        if (PlayerPrefs.HasKey(FPS_KEY))
        {
            int savedFPSIndex = PlayerPrefs.GetInt(FPS_KEY);
            FPSToggle.isOn = savedFPSIndex == 1;
            OnToggleValueChanged(FPSToggle.isOn);
        }

        else
        {
            FPSToggle.isOn = false;
            OnToggleValueChanged(FPSToggle.isOn);
        }

        FPSToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private IEnumerator GetAudioComponentAfterOneSecond()
    {
        float timeToWaitToGetComponent = 0.5f;

        yield return new WaitForSeconds(timeToWaitToGetComponent);

        buttonClick = GetComponent<AudioSource>();
    }
}
