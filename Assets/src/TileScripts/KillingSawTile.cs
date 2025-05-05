using UnityEngine;

public class KillingSawTile : MoveableTile {
    public float speed = 5f;
    public float maxDistance = 5f;
    private Vector3 startPosition;
    private bool movingRight = true;

    void Start() {
        shouldMove = false;
        startPosition = transform.position;
    }

    void Update() {
        if (shouldMove) {
            Move();
        }
    }

    void Move() {
        float step = speed * Time.deltaTime;
        if (movingRight) {
            transform.position += Vector3.right * step;
            if (Vector3.Distance(startPosition, transform.position) >= maxDistance) {
                movingRight = false;
            }
        } else {
            transform.position -= Vector3.right * step;
            if (Vector3.Distance(startPosition, transform.position) <= 0.1f) {
                movingRight = true;
            }
        }
    }
}