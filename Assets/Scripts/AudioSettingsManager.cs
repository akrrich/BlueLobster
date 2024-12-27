using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider musicSlider;   
    [SerializeField] public Slider SFXSlider;     
    

    void Start()
    {
        InitializeValues();
    }


    public void SetMusicVolume()
    {
        if (musicSlider.value <= 0.01f)
        {
            audioMixer.SetFloat("Music", -80); 
        }

        else
        {
            audioMixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
        }

        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

    public void SetSFXVolume()
    {
        if (SFXSlider.value <= 0.01f)  
        {
            audioMixer.SetFloat("SFX", -80); 
        }

        else
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(SFXSlider.value) * 20);
        }

        PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
    }


    private void InitializeValues()
    {
        float initializeValue = 0.5f;

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", initializeValue);
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", initializeValue);

        SetMusicVolume();
        SetSFXVolume();
    }
}
