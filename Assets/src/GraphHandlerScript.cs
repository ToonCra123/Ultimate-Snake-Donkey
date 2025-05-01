using UnityEngine;

public class GraphHandlerScript : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject currentPreviewBlock;
    public GameObject[] placeablePrefabs;

    private GameObject selectedPrefab;
    private bool placing = false;

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

        if (Input.GetMouseButtonDown(0))
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

    void PlaceBlock(Vector2 pos)
    {
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
