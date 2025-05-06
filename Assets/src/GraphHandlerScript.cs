using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;

public class GraphHandlerScript : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject currentPreviewBlock;
    public GameObject[] placeablePrefabs;

    private GameObject selectedPrefab;
    private bool placing = false;
    private Quaternion currRotation;

    private int timeout = 500;
    private int timout_timer = 0;




    public List<Rect> blockedAreas = new List<Rect>() {
        new Rect(-9f, -4f, -6f, -6f),  // Example: block area from (-2, -1) to (2, 1)
        new Rect(3, 2, 2, 2)     // Another blocked zone
    };

    void Update()
    {
        if (!placing) return;

        if (timout_timer < timeout)
        {
            timout_timer++;
            return;
        }

        selectedPrefab.SetActive(true);

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
        } else if (!CanPlaceBlock()) 
        {
            SetPreviewModeNoPlace(currentPreviewBlock);
        } else if (CanPlaceBlock())
        {
            SetPreviewModePlace(currentPreviewBlock);
        }
    }

    public bool isPlacing()
    {
        return placing;
    }

    public void StartPlacingBlock(int index)
    {
        timout_timer = 0;

        selectedPrefab = placeablePrefabs[index];
        currentPreviewBlock = Instantiate(selectedPrefab);
        currRotation = currentPreviewBlock.transform.rotation;
        SetPreviewMode(currentPreviewBlock, true);
        placing = true;
    }

    public void StartPlacingBlock(GameObject gmObj)
    {
        selectedPrefab = gmObj;
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
        try
        {
            BoxCollider2D blockBox = currentPreviewBlock.GetComponent<BoxCollider2D>();
            return !CheckOverlaps(blockBox);
        } catch
        { 
            return false; 
        }
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

    bool CheckOverlaps(BoxCollider2D myBox)
    {
        BoxCollider2D[] allBoxes = FindObjectsByType<BoxCollider2D>(FindObjectsSortMode.None);

        foreach (BoxCollider2D otherBox in allBoxes)
        {
            if (otherBox == myBox) continue; // skip self

            if (myBox.bounds.Intersects(otherBox.bounds))
            {
                return true;
            }
        }

        return false;
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

    void SetPreviewModeNoPlace(GameObject obj)
    {
        var renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            color.g = 0.2f;
            color.b = 0.2f;
            color.a = 0.5f;
            renderer.color = color;
        }
    }

    void SetPreviewModePlace(GameObject obj)
    {
        var renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            color.g = 1f;
            color.b = 1f;
            color.a = 0.5f;
            renderer.color = color;
        }
    }



}
