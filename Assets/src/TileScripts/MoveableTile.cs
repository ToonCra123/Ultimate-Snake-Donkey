using UnityEngine;

public class MoveableTile : MonoBehaviour
{
    public GameStateManager state;
    public Transform rotatePoint;
    public float rotationSpeed = 50.0f;
    public Transform rotatingShit;

    // Update is called once per frame
    void Update()
    {
        if (state == null) return;
        if (state.shouldObjectsMove())
        {
            float step = rotationSpeed * Time.deltaTime;

            // Rotate around the rotatePoint
            rotatingShit.RotateAround(rotatePoint.position, Vector3.forward, step);
        }
    }
}
