using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject titleScreen;

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
        SceneManager.LoadScene("LevelScene");
    }
}
