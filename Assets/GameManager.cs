using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Camera mainCamera;
    public Transform boxContents;


    [Header("Box Spawn Settings")]
    public Transform cameraFocusPoint;
    public GameObject boxPrefab;
    public GameObject[] props; // random selection pool
    public Transform[] propSpawnPoints; // 4 defined spawn locations
    private bool boxSpawned = false;
    public bool selectionDone = false;







    public float cameraSmoothSpeed = 5f;
    public Vector3 cameraOffset = new Vector3(0, 2, -10);
    public float safeMargin = 3f;
    public float defaultZoom = 6f;
    public float maxZoomOut = 12f;
    public float zoomSensitivity = 0.6f;



    private GameObject player1;
    private GameObject player2;

    private int selectionTurn = 0; // 0 = P1, 1 = P2
    private GameObject selectedByP1;
    private GameObject selectedByP2;
    private GameObject currentBox;

    private bool placingProps = false;
    private bool p1Placed = false;
    private bool p2Placed = false;
    private GameObject propBeingPlaced;




    void Awake()
    {
        if (player1Prefab && player2Prefab && spawnPoint1 && spawnPoint2)
        {
            player1 = Instantiate(player1Prefab, spawnPoint1.position, Quaternion.identity);
            player2 = Instantiate(player2Prefab, spawnPoint2.position, Quaternion.identity);

            mainCamera.orthographicSize = defaultZoom;
        }
        else
        {
            Debug.LogError("Missing references in GameManager");
        }
    }

    void FixedUpdate()
    {
        if (!player1 && !player2) return;

        bool p1Dead = IsDead(player1);
        bool p2Dead = IsDead(player2);

        if (p1Dead && p2Dead)
        {
            if (cameraFocusPoint)
            {
                Vector3 focusTarget = cameraFocusPoint.position + cameraOffset;
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, focusTarget, Time.fixedDeltaTime * cameraSmoothSpeed);
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 10f, Time.fixedDeltaTime * 3f);
            }

            if (!boxSpawned && boxPrefab && props.Length > 0)
            {
                currentBox = Instantiate(boxPrefab, new Vector3(cameraFocusPoint.position.x, cameraFocusPoint.position.y, cameraFocusPoint.position.z - 5), Quaternion.identity);
                boxSpawned = true;

                SpawnPropsInBox(currentBox);
            }



            return;
        }






        Vector3 cameraCenter = mainCamera.transform.position - cameraOffset;
        Vector3 followTarget = cameraCenter;

        float vertExtent = mainCamera.orthographicSize;
        float horizExtent = vertExtent * mainCamera.aspect;
        Bounds cameraBounds = new Bounds(cameraCenter, new Vector3(horizExtent * 2 - safeMargin * 2, vertExtent * 2 - safeMargin * 2, 0));

        bool p1Outside = !p1Dead && !cameraBounds.Contains(player1.transform.position);
        bool p2Outside = !p2Dead && !cameraBounds.Contains(player2.transform.position);

        if (p1Outside && !p2Outside)
            followTarget = player1.transform.position;
        else if (p2Outside && !p1Outside)
            followTarget = player2.transform.position;
        else if (p1Outside && p2Outside)
            followTarget = (player1.transform.position + player2.transform.position) / 2f;
        else if (!p1Dead && p2Dead)
            followTarget = player1.transform.position;
        else if (!p2Dead && p1Dead)
            followTarget = player2.transform.position;

        Vector3 finalTarget = followTarget + cameraOffset;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, finalTarget, Time.fixedDeltaTime * cameraSmoothSpeed);

        if (!p1Dead && !p2Dead)
        {
            float playerDistance = Vector3.Distance(player1.transform.position, player2.transform.position);
            float targetZoom = Mathf.Clamp(defaultZoom + playerDistance * zoomSensitivity, defaultZoom, maxZoomOut);
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetZoom, Time.fixedDeltaTime * 3f);
        }
        else if (!p1Dead || !p2Dead)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 3f, Time.fixedDeltaTime * 3f);
        }


    }






    void Update()
    {
        if (placingProps)
        {
            if (propBeingPlaced)
            {
                Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorld.z = 0f;
                propBeingPlaced.transform.position = mouseWorld;

                if (Input.GetMouseButtonDown(0))
                {
                    propBeingPlaced = null;

                    if (!p1Placed)
                    {
                        p1Placed = true;
                        propBeingPlaced = Instantiate(selectedByP2, mouseWorld, Quaternion.identity);
                        propBeingPlaced.transform.localScale *= 2.5f;
                        return;

                    }
                    else if (!p2Placed)
                    {
                        p2Placed = true;
                        placingProps = false;
                        RevivePlayers();
                    }
                }
            }
            return;
        }
    }


    bool IsDead(GameObject obj)
    {
        if (!obj) return true;
        var p1 = obj.GetComponent<PlayerMovment>();
        var p2 = obj.GetComponent<PlayerMovmentP2>();
        if (p1) return p1.isDead;
        if (p2) return p2.isDead;
        return true;
    }


    void SpawnPropsInBox(GameObject box)
    {
        foreach (Transform spawnPoint in box.transform)
        {
            Vector3 spawnPos = spawnPoint.position;
            spawnPos.z = 0f;

            GameObject randomProp = Instantiate(props[Random.Range(0, props.Length)], spawnPos, Quaternion.identity);
            randomProp.transform.parent = boxContents.transform;

            var clickable = randomProp.AddComponent<ClickableProp>();
            clickable.Init(this);
        }
    }


    public void OnPropSelected(GameObject selected)
    {
        if (selectionTurn == 0)
        {
            selectedByP1 = props.FirstOrDefault(p => p.name == selected.name.Replace("(Clone)", "").Trim());
            selectionTurn = 1;

            // Destroy old props
            foreach (Transform child in boxContents.transform)
                Destroy(child.gameObject);

            // Spawn new ones for player 2
            SpawnPropsInBox(currentBox);
        }
        else if (selectionTurn == 1)
        {
            selectedByP2 = props.FirstOrDefault(p => p.name == selected.name.Replace("(Clone)", "").Trim());
            selectionDone = true;

            // Store data BEFORE destroying
            string p1Name = selectedByP1 != null ? selectedByP1.name : "None";
            string p2Name = selectedByP2 != null ? selectedByP2.name : "None";

            Debug.Log("Player 1 picked: " + p1Name);
            Debug.Log("Player 2 picked: " + p2Name);

            Destroy(currentBox);

            // Destroy old props
            foreach (Transform child in boxContents.transform)
                Destroy(child.gameObject);


            placingProps = true;
            p1Placed = false;
            p2Placed = false;
            propBeingPlaced = Instantiate(selectedByP1, Vector3.zero, Quaternion.identity);
            propBeingPlaced.transform.localScale *= 2.5f;
        }
    }


    void RevivePlayers()
    {
        var p1Script = player1.GetComponent<PlayerMovment>();
        var p2Script = player2.GetComponent<PlayerMovmentP2>();

        if (p1Script)
        {
            p1Script.isDead = false;
            player1.transform.position = spawnPoint1.position;
            player1.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        if (p2Script)
        {
            p2Script.isDead = false;
            player2.transform.position = spawnPoint2.position;
            player2.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        selectionTurn = 0; // ? THIS LINE FIXES IT
        selectionDone = false;
        boxSpawned = false;
        selectedByP1 = null;
        selectedByP2 = null;
    }






}
