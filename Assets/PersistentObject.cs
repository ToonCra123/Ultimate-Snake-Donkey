using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentMusic : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip menuMusic;
    public AudioClip levelMusic;

    void Awake()
    {
        if (FindObjectsOfType<PersistentMusic>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded; // Hook into scene load
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            if (audioSource.clip != menuMusic)
            {
                audioSource.clip = menuMusic;
                audioSource.Play();
            }
        }
        else if (scene.name == "LevelScene") // Replace with actual level scene name
        {
            if (audioSource.clip != levelMusic)
            {
                audioSource.clip = levelMusic;
                audioSource.Play();
            }
        }
    }
}
