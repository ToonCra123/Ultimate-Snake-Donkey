using UnityEngine;

public class SpinObject : MonoBehaviour
{
    public float spinSpeed = 360f; // degrees per second

    void Update()
    {
        transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
    }
}
