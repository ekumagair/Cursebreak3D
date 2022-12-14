using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public GameObject mapObjectsRoot;
    public GameObject wallSpritePrefab;
    public GameObject playerSpritePrefab;
    public Vector3 posOffset;
    public float scrollScale = 1f;
    public float zoomScale;

    GameObject[] everyGameObject;
    GameObject playerObject;
    GameObject playerSprite;

    List<Vector3> usedPositions = new List<Vector3>();

    void Start()
    {
        everyGameObject = GameObject.FindObjectsOfType<GameObject>();
        mapObjectsRoot.transform.localScale = new Vector3(1, 1, 1);
        usedPositions.Clear();

        if (StaticClass.minimapType == 1)
        {
            Vector3 posLatest = new Vector3(0, 0, 0);

            foreach (GameObject obj in everyGameObject)
            {
                if (obj.name != "Floor" && obj.name != "Ceiling" && obj.GetComponent<Transform>() != null && obj.GetComponent<RectTransform>() == null && StaticClass.minimapType == 1)
                {
                    if (obj.isStatic && posLatest != new Vector3(obj.transform.position.x + posOffset.x, obj.transform.position.z + posOffset.y, 0))
                    {
                        var ws = Instantiate(wallSpritePrefab, mapObjectsRoot.transform);
                        ws.transform.position = new Vector3(obj.transform.position.x + posOffset.x, obj.transform.position.z + posOffset.y, 0);
                        posLatest = ws.transform.position;
                    }
                    else if (obj.tag == "Player")
                    {
                        playerObject = obj;
                        playerSprite = Instantiate(playerSpritePrefab, mapObjectsRoot.transform);
                    }
                }
            }
        }
        else if (StaticClass.minimapType == 2)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            playerSprite = Instantiate(playerSpritePrefab, mapObjectsRoot.transform);
        }
    }

    void Update()
    {
        if (HUD.mapEnabled && StaticClass.gameState == 0)
        {
            if (StaticClass.minimapType == 1)
            {
                playerSprite.transform.localPosition = new Vector3(playerObject.transform.position.x + posOffset.x - 640, playerObject.transform.position.z + posOffset.y - 400, 0);
            }
            else if (StaticClass.minimapType == 2)
            {
                playerSprite.transform.localPosition = new Vector3(playerObject.transform.position.x + posOffset.x, playerObject.transform.position.z + posOffset.y, 0);
            }

            if (StaticClass.minimapType != 0)
            {
                mapObjectsRoot.transform.position -= transform.right * Input.GetAxisRaw("Horizontal") * scrollScale * Time.deltaTime;
                mapObjectsRoot.transform.position -= transform.up * Input.GetAxisRaw("Vertical") * scrollScale * Time.deltaTime;

                if (StaticClass.minimapType == 2)
                {
                    if (mapObjectsRoot.transform.localPosition == Vector3.zero)
                    {
                        mapObjectsRoot.transform.localPosition = playerSprite.transform.localPosition * -1;
                    }
                }

                if (Input.GetAxis("Mouse ScrollWheel") > 0 && mapObjectsRoot.transform.localScale.x < 8f)
                {
                    mapObjectsRoot.transform.localScale += new Vector3(zoomScale, zoomScale, zoomScale);
                    mapObjectsRoot.transform.position -= transform.right * 160 * 5 * zoomScale;
                    mapObjectsRoot.transform.position -= transform.up * 80 * 5 * zoomScale;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0 && mapObjectsRoot.transform.localScale.x > 0.8f)
                {
                    mapObjectsRoot.transform.localScale -= new Vector3(zoomScale, zoomScale, zoomScale);
                    mapObjectsRoot.transform.position += transform.right * 160 * 5 * zoomScale;
                    mapObjectsRoot.transform.position += transform.up * 80 * 5 * zoomScale;
                }
            }
        }
    }

    public void AddToMinimap(GameObject obj)
    {
        if (usedPositions.Contains(new Vector3(obj.transform.position.x + posOffset.x, obj.transform.position.z + posOffset.y, 0)) == false && Time.timeScale != 0.0f)
        {
            playerSprite.transform.SetParent(null);

            Vector3 previousMinimapPosition = mapObjectsRoot.transform.position;
            Vector3 previousMinimapScale = mapObjectsRoot.transform.localScale;

            mapObjectsRoot.transform.position = new Vector3(0, 0, 0);
            mapObjectsRoot.transform.localScale = new Vector3(1, 1, 1);

            var ws = Instantiate(wallSpritePrefab, mapObjectsRoot.transform);
            ws.transform.position = new Vector3(obj.transform.position.x + posOffset.x, obj.transform.position.z + posOffset.y, 0);
            usedPositions.Add(ws.transform.position);

            Debug.Log("Added " + obj.name + " to minimap");

            mapObjectsRoot.transform.position = previousMinimapPosition;
            mapObjectsRoot.transform.localScale = previousMinimapScale;

            playerSprite.transform.SetParent(mapObjectsRoot.transform);
        }
    }
}
