using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject titleScreen;



    public SpriteRenderer fadeSprite;


    private bool onTitleScreen = true;

    void Start()
    {
        ShowTitleScreen();
    }

    void Update()
    {
        if (onTitleScreen && Input.anyKeyDown)
        {
            ShowMainMenu();
        }
    }

    public void ShowTitleScreen()
    {
        titleScreen.SetActive(true);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        onTitleScreen = true;
    }


    public void HideMenus()
    {
        titleScreen.SetActive(false);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        onTitleScreen = false;
    }

    public void ShowMainMenu()
    {
        titleScreen.SetActive(false);
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        onTitleScreen = false;
    }

    public void ShowSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }

    public void PlayGame()
    {
        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        float fadeDuration = 0.5f;
        float t = 0f;

        Color startColor = fadeSprite.color;
        Color endColor = new Color(0, 0, 0, 1); // fully opaque black

        HideMenus();
        // Fade in
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeSprite.color = Color.Lerp(startColor, endColor, t / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene("LoadingScene");
    }
}

