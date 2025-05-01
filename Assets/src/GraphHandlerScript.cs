using System.Collections.Generic;
using UnityEngine;

public class GraphHandlerScript : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject currentPreviewBlock;
    public GameObject[] placeablePrefabs;

    private GameObject selectedPrefab;
    private bool placing = false;

    public List<Rect> blockedAreas = new List<Rect>() {
        new Rect(-8.5f, -3.5f, -5.5f, -5),  // Example: block area from (-2, -1) to (2, 1)
        new Rect(3, 2, 2, 2)     // Another blocked zone
    };

    void Update()
    {
        if (!placing && Input.GetMouseButtonUp(0))
        {
            StartPlacingBlock(0);
        }
        if (!placing) return;

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 snappedPos = Utils.SnapToGrid(mouseWorldPos);

        currentPreviewBlock.transform.position = snappedPos;

        if (Input.GetMouseButtonDown(0) && CanPlaceBlock())
        {
            // Confirm placement
            PlaceBlock(snappedPos);
        }
    }

    public void StartPlacingBlock(int index)
    {
        selectedPrefab = placeablePrefabs[index];
        currentPreviewBlock = Instantiate(selectedPrefab);
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
        Instantiate(selectedPrefab, pos, Quaternion.identity);
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
