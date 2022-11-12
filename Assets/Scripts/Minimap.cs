using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public GameObject mapObjectsRoot;
    public GameObject wallSprite;
    public GameObject playerSprite;
    public Vector3 posOffset;
    public float scrollScale = 1f;
    public float zoomScale;

    GameObject[] everyGameObject;
    GameObject playerObject;
    GameObject ps;

    void Start()
    {
        everyGameObject = GameObject.FindObjectsOfType<GameObject>();
        mapObjectsRoot.transform.localScale = new Vector3(1, 1, 1);

        if (StaticClass.minimapType == 1)
        {
            Vector3 posRecente = new Vector3(0, 0, 0);

            foreach (GameObject obj in everyGameObject)
            {
                if (obj.name != "Floor" && obj.name != "Ceiling" && obj.GetComponent<Transform>() != null && obj.GetComponent<RectTransform>() == null && StaticClass.minimapType == 1)
                {
                    if (obj.isStatic && posRecente != new Vector3(obj.transform.position.x + posOffset.x, obj.transform.position.z + posOffset.y, 0))
                    {
                        var ws = Instantiate(wallSprite, mapObjectsRoot.transform);
                        ws.transform.position = new Vector3(obj.transform.position.x + posOffset.x, obj.transform.position.z + posOffset.y, 0);
                        posRecente = ws.transform.position;
                    }
                    else if (obj.tag == "Player")
                    {
                        playerObject = obj;
                        ps = Instantiate(playerSprite, mapObjectsRoot.transform);
                    }
                }
            }
        }
    }

    void Update()
    {
        if (HUD.mapEnabled && StaticClass.gameState == 0)
        {
            ps.transform.localPosition = new Vector3(playerObject.transform.position.x + posOffset.x - 640, playerObject.transform.position.z + posOffset.y - 400, 0);
            mapObjectsRoot.transform.position -= transform.right * Input.GetAxisRaw("Horizontal") * scrollScale * Time.deltaTime;
            mapObjectsRoot.transform.position -= transform.up * Input.GetAxisRaw("Vertical") * scrollScale * Time.deltaTime;

            if (Input.GetAxis("Mouse ScrollWheel") > 0 && mapObjectsRoot.transform.localScale.x < 7f)
            {
                mapObjectsRoot.transform.localScale += new Vector3(zoomScale, zoomScale, zoomScale);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && mapObjectsRoot.transform.localScale.x > 0.3f)
            {
                mapObjectsRoot.transform.localScale -= new Vector3(zoomScale, zoomScale, zoomScale);
            }
        }
    }
}
