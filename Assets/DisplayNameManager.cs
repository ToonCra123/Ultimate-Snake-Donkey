using UnityEngine;
using TMPro;

public class DisplayNameManager : MonoBehaviour
{
    public TMP_Text displayText;
    public bool isPlayer1 = true; // Set this in the inspector to switch between player 1 or 2

    void Start()
    {
        string key = isPlayer1 ? "Player1Username" : "Player2Username";
        string username = PlayerPrefs.GetString(key, "Unnamed");
        displayText.text = username;
    }
}
