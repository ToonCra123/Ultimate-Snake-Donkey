using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider musicSlider;
    public AudioSource musicSource;
    private const string MusicVolumeKey = "MusicVolume";
    private const string MusicMuteKey = "MusicMuted";


    void Start()
    {
        // Load saved volume or use default (1.0f)
        float savedVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        musicSlider.value = savedVolume;

        // Apply to audio
        SetVolume(savedVolume);

        // Listen for changes
        musicSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        // Apply to your audio system
        AudioListener.volume = volume;

        // Save setting
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        PlayerPrefs.Save();

        musicSource.volume = volume;
    }




    public void MuteVolume()
    {
        bool isMuted = PlayerPrefs.GetInt(MusicMuteKey, 0) == 1;

        if (!isMuted)
        {
            // Save current volume for unmute
            float currentVolume = musicSlider.value;
            PlayerPrefs.SetFloat("LastVolumeBeforeMute", currentVolume);

            // Mute audio without changing slider
            musicSource.volume = 0;
            AudioListener.volume = 0;

            PlayerPrefs.SetInt(MusicMuteKey, 1);
        }
        else
        {
            // Restore saved volume
            float savedVolume = PlayerPrefs.GetFloat("LastVolumeBeforeMute", 1f);
            musicSource.volume = savedVolume;
            AudioListener.volume = savedVolume;

            PlayerPrefs.SetInt(MusicMuteKey, 0);
        }

        PlayerPrefs.Save();
    }


}
