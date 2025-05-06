using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;

public class GraphHandlerScript : MonoBehaviour
{
    [Header("Game State")]
    public GameStateManager state;

    public Camera mainCamera;
    public GameObject currentPreviewBlock;
    public GameObject[] placeablePrefabs;

    private GameObject selectedPrefab;
    private bool placing = false;
    private Quaternion currRotation;

    private int timeout = 500;
    private int timout_timer = 0;

    void Update()
    {
        if (!placing) return;

        selectedPrefab.SetActive(true);

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 snappedPos = Utils.SnapToGrid(mouseWorldPos);

        currentPreviewBlock.transform.position = snappedPos;
        if(Input.GetKeyDown(KeyCode.R))
        {
            currentPreviewBlock.transform.Rotate(0, 0, 90);
            currRotation = currentPreviewBlock.transform.rotation;
        }



        if (timout_timer < timeout)
        {
            timout_timer++;
            return;
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
        
        gmObj.SetActive(false);
        selectedPrefab = gmObj;
        currentPreviewBlock = Instantiate(selectedPrefab);
        currentPreviewBlock.SetActive(true);
        currRotation = currentPreviewBlock.transform.rotation;
        SetPreviewMode(currentPreviewBlock, true);
        placing = true;
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
        if (!CanPlaceBlock()) return;
        GameObject cool = Instantiate(selectedPrefab, pos, currRotation);
        Destroy(currentPreviewBlock);
        if (cool.GetComponent<MoveableTile>() != null)
        {
            cool.GetComponent<MoveableTile>().state = state;
        }
        timout_timer = 0;
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
