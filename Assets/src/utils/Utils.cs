using UnityEngine;

public class Utils
{
    public static Vector2 SnapToGrid(Vector2 rawPosition, float gridSize = 0.5f)
    {
        float x = Mathf.Round(rawPosition.x / gridSize) * gridSize;
        float y = Mathf.Round(rawPosition.y / gridSize) * gridSize;
        return new Vector2(x, y);
    }
}
