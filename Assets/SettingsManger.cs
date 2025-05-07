using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider musicSlider;
    public AudioSource musicSource;
    private const string MusicVolumeKey = "MusicVolume";
    private const string MusicMuteKey = "MusicMuted";

    public TMP_InputField player1Input;
    public TMP_InputField player2Input;
    private const string Player1Key = "Player1Username";
    private const string Player2Key = "Player2Username";



    void Start()
    {
        // Load saved volume or use default (1.0f)
        float savedVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        musicSlider.value = savedVolume;

        // Apply to audio
        SetVolume(savedVolume);

        // Listen for changes
        musicSlider.onValueChanged.AddListener(SetVolume);


         // Existing music code...

         player1Input.text = PlayerPrefs.GetString(Player1Key, "");
        Debug.Log("Player 1 Username: " + PlayerPrefs.GetString(Player1Key, ""));
        player2Input.text = PlayerPrefs.GetString(Player2Key, "");
        Debug.Log("Player 2 Username: " + PlayerPrefs.GetString(Player2Key, ""));

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


    public void SavePlayer1Username()
    {
        Debug.Log("Saving Player 1 Username: " + player1Input.text);
        PlayerPrefs.SetString(Player1Key, player1Input.text);
        PlayerPrefs.Save();
    }

    public void SavePlayer2Username()
    {
        Debug.Log("Saving Player 2 Username: " + player2Input.text);
        PlayerPrefs.SetString(Player2Key, player2Input.text);
        PlayerPrefs.Save();
    }



}
