using UnityEngine;

public class PersistentMusic : MonoBehaviour
{
    void Awake()
    {
        // Prevent duplicates
        if (FindObjectsOfType<PersistentMusic>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
