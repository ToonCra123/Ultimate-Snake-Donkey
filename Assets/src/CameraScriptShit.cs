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
        following = true;
        StartCoroutine(TransitionToFollow());
    }

    IEnumerator TransitionToFollow()
    {
        inTransition = true;

        float elapsed = 0f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = player.position + new Vector3(0, 5, 0); // no -10 on Z for 2D

        Camera cam = GetComponent<Camera>();
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.identity; // optional: no rotation for 2D

        float startZoom = sceneZoom;
        float endZoom = followZoom;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            t = Mathf.SmoothStep(0f, 1f, t); // smoother and feels faster

            // Lerp camera zoom
            if (cam != null)
                cam.orthographicSize = Mathf.Lerp(startZoom, endZoom, t);

            // Interpolate X and Y only, keep original Z
            Vector2 interpolated2D = Vector2.Lerp(
                new Vector2(startPos.x, startPos.y),
                new Vector2(targetPos.x, targetPos.y),
                t
            );
            transform.position = new Vector3(interpolated2D.x, interpolated2D.y, startPos.z);

            // Optional: Skip rotation or use fixed 2D angle
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final position and zoom
        transform.position = new Vector3(targetPos.x, targetPos.y, startPos.z);
        if (cam != null) cam.orthographicSize = endZoom;
        transform.rotation = targetRot;

        inTransition = false;
    }

    public void StartSceneViewTransition()
    {
        if (inTransition) return;
        following = false; // stop following player
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
        Quaternion targetRot = Quaternion.identity; // or overviewPosition.rotation if needed

        float startZoom = cam != null ? cam.orthographicSize : sceneZoom;
        float endZoom = sceneZoom;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            // Zoom interpolation
            if (cam != null)
                cam.orthographicSize = Mathf.Lerp(startZoom, endZoom, t);

            // Interpolate X and Y only
            Vector2 interpolated2D = Vector2.Lerp(
                new Vector2(startPos.x, startPos.y),
                new Vector2(targetPos.x, targetPos.y),
                t
            );
            transform.position = new Vector3(interpolated2D.x, interpolated2D.y, startPos.z);

            // Optional: handle rotation
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final set
        transform.position = new Vector3(targetPos.x, targetPos.y, startPos.z);
        if (cam != null) cam.orthographicSize = endZoom;
        transform.rotation = targetRot;

        inTransition = false;
    }

    private void Update()
    {
        if (!following || inTransition) return;

        Vector3 playerPos = player.position;
        transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
    }

    public bool isFollowing() { return following; }
}
