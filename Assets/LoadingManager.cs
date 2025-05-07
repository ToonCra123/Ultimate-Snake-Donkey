using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;


public class LoadingManager : MonoBehaviour
{
    public string targetScene = "LevelScene";
    public float waitBeforeLoading = 1f;
    public TextMeshProUGUI loadingText; // drag your "LOADING..." text here



    void Start()
    {
        StartCoroutine(AnimateLoadingText());
        StartCoroutine(LoadTargetScene());
    }


    IEnumerator LoadTargetScene()
    {
        yield return new WaitForSeconds(waitBeforeLoading);

        // Load the target scene additively
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);

        while (!loadOp.isDone)
        {
            yield return null;
        }

        // Optionally fade out loading visuals here

        // Unload this loading scene
        SceneManager.UnloadSceneAsync("LoadingScene");
    }

    IEnumerator AnimateLoadingText()
    {
        string baseText = "LOADING";
        int dotCount = 0;

        while (true)
        {
            dotCount = (dotCount + 1) % 4; // cycles from 0 to 3
            loadingText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(0.25f);
        }
    }

}
