using System.Collections.Generic;
using UnityEngine;

public class GraphHandlerScript : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject currentPreviewBlock;
    public GameObject[] placeablePrefabs;

    private GameObject selectedPrefab;
    private bool placing = false;
    private Quaternion currRotation;

    public List<Rect> blockedAreas = new List<Rect>() {
        new Rect(-9f, -4f, -6f, -6f),  // Example: block area from (-2, -1) to (2, 1)
        new Rect(3, 2, 2, 2)     // Another blocked zone
    };

    void Update()
    {
        if (!placing) return;

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 snappedPos = Utils.SnapToGrid(mouseWorldPos);

        currentPreviewBlock.transform.position = snappedPos;
        if(Input.GetKeyDown(KeyCode.R))
        {
            currentPreviewBlock.transform.Rotate(0, 0, 90);
            currRotation = currentPreviewBlock.transform.rotation;
        }

        if (Input.GetMouseButtonDown(0) && CanPlaceBlock())
        {
            // Confirm placement
            PlaceBlock(snappedPos);
        }
    }

    public bool isPlacing()
    {
        return placing;
    }

    public void StartPlacingBlock(int index)
    {
        selectedPrefab = placeablePrefabs[index];
        currentPreviewBlock = Instantiate(selectedPrefab);
        currRotation = currentPreviewBlock.transform.rotation;
        SetPreviewMode(currentPreviewBlock, true);
        placing = true;
    }

    bool IsInBlockedArea(Vector2 pos)
    {
        foreach (Rect area in blockedAreas)
        {
            if (area.Contains(pos))
            {
                return true;
            }
        }
        return false;
    }

    // Fix this plz
    bool CanPlaceBlock()
    {
        return true;
    }

    void PlaceBlock(Vector2 pos)
    {
        if (IsInBlockedArea(pos))
        {
            Debug.Log("Blocked: This area is not allowed");
            return;
        }
        Instantiate(selectedPrefab, pos, currRotation);
        Destroy(currentPreviewBlock);
        placing = false;
    }

    void SetPreviewMode(GameObject obj, bool isPreview)
    {
        var renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            color.a = isPreview ? 0.5f : 1f;
            renderer.color = color;
        }
    }



}
