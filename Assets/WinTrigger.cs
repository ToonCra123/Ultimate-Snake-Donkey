using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class WinTrigger : MonoBehaviour
{
    public string menuSceneName = "MainMenu";

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered by: " + other.name);

        if (!other.CompareTag("Player")) return;

        // Figure out who won and who lost
        if (other.GetComponent<PlayerMovment>())
        {
            Debug.Log("Player 1 wins!");
            StartCoroutine(ReportWin("Player1Username"));
            StartCoroutine(ReportLose("Player2Username"));
        }
        else if (other.GetComponent<PlayerMovmentP2>())
        {
            Debug.Log("Player 2 wins!");
            StartCoroutine(ReportWin("Player2Username"));
            StartCoroutine(ReportLose("Player1Username"));
        }

        SceneManager.LoadScene(menuSceneName);
    }

    IEnumerator ReportWin(string playerKey)
    {
        string username = PlayerPrefs.GetString(playerKey, "Unknown");
        string json = JsonUtility.ToJson(new UsernameWrapper { username = username });

        UnityWebRequest req = new UnityWebRequest("https://api.toonhosting.net/player/win", "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
            Debug.Log("Reported win for " + username);
        else
            Debug.LogWarning("Win report failed: " + req.error);
    }

    IEnumerator ReportLose(string playerKey)
    {
        string username = PlayerPrefs.GetString(playerKey, "Unknown");
        string json = JsonUtility.ToJson(new UsernameWrapper { username = username });

        UnityWebRequest req = new UnityWebRequest("https://api.toonhosting.net/player/lose", "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
            Debug.Log("Reported loss for " + username);
        else
            Debug.LogWarning("Loss report failed: " + req.error);
    }

    [System.Serializable]
    public class UsernameWrapper
    {
        public string username;
    }
}
