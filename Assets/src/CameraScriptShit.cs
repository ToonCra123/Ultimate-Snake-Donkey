using System.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class CameraScriptShit : MonoBehaviour
{
    public Transform overviewPosition;  // empty GameObject where camera starts
    public float transitionDuration = 1f;

    private Transform player;

    private float followZoom = 7;
    private float sceneZoom = 20f;

    private bool following;
    private bool inTransition;

    private void Start()
    {
        following = false;
        inTransition = false;
    }

    public void StartFollowCameraTransition(Transform playerIn)
    {
        player = playerIn;
        StartCoroutine(TransitionToFollow());
    }

    IEnumerator TransitionToFollow()
    {
        inTransition = true;

        float elapsed = 0f;
        Vector3 startPos = transform.position;

        Camera cam = GetComponent<Camera>();
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.identity;

        float startZoom = cam != null ? cam.orthographicSize : sceneZoom;
        float endZoom = followZoom;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            // Dynamic target position (updates each frame)
            Vector3 currentTargetPos = player.position + new Vector3(0, 2, 0);

            // Zoom
            if (cam != null)
                cam.orthographicSize = Mathf.Lerp(startZoom, endZoom, t);

            // Position
            Vector2 interpolated2D = Vector2.Lerp(
                new Vector2(startPos.x, startPos.y),
                new Vector2(currentTargetPos.x, currentTargetPos.y),
                t
            );
            transform.position = new Vector3(interpolated2D.x, interpolated2D.y, startPos.z);

            // Rotation (not needed in 2D but kept for consistency)
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final snap to player's current position
        Vector3 finalTarget = player.position + new Vector3(0, 2, 0);
        transform.position = new Vector3(finalTarget.x, finalTarget.y, startPos.z);
        if (cam != null) cam.orthographicSize = endZoom;
        transform.rotation = targetRot;

        inTransition = false;
        following = true;
    }

    public void StartSceneViewTransition()
    {
        if (inTransition) return;
        following = false;
        StartCoroutine(TransitionToSceneView());
    }

    IEnumerator TransitionToSceneView()
    {
        inTransition = true;

        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = overviewPosition.position;

        Camera cam = GetComponent<Camera>();
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.identity;

        float startZoom = cam != null ? cam.orthographicSize : sceneZoom;
        float endZoom = sceneZoom;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            if (cam != null)
                cam.orthographicSize = Mathf.Lerp(startZoom, endZoom, t);

            Vector2 interpolated2D = Vector2.Lerp(
                new Vector2(startPos.x, startPos.y),
                new Vector2(targetPos.x, targetPos.y),
                t
            );
            transform.position = new Vector3(interpolated2D.x, interpolated2D.y, startPos.z);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(targetPos.x, targetPos.y, startPos.z);
        if (cam != null) cam.orthographicSize = endZoom;
        transform.rotation = targetRot;

        inTransition = false;
    }

    private void Update()
    {
        if (!following || inTransition) return;

        Vector3 playerPos = player.position;
        transform.position = new Vector3(playerPos.x, playerPos.y + 2, transform.position.z);
    }

    public bool isFollowing() { return following; }
}
