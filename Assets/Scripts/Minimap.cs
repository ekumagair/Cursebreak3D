using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    #region Variables

    public GameObject mapObjectsRoot;
    public GameObject wallSpritePrefab;
    public GameObject playerSpritePrefab;
    public GameObject[] doorSpritePrefab;
    public GameObject floorSpritePrefab;
    public Vector3 posOffset;
    public float scrollScale = 1f;
    public float zoomScale;

    private GameObject[] _everyGameObject;
    private GameObject _playerObject;
    private GameObject _playerSprite;

    private Vector3 _previousMinimapPosition;
    private Vector3 _previousMinimapScale;

    private Vector3 _lastFloorPosition = Vector3.zero;

    private List<Vector3> _usedPositions = new List<Vector3>();
    private List<GameObject> _filteredObjects = new List<GameObject>();

    #endregion

    #region Default Methods

    void Start()
    {
        _everyGameObject = GameObject.FindObjectsOfType<GameObject>();
        mapObjectsRoot.transform.localScale = new Vector3(1, 1, 1);
        _usedPositions.Clear();
        _filteredObjects.Clear();

        if (StaticClass.minimapType == 1)
        {
            Vector3 posLatest = new Vector3(0, 0, 0);

            foreach (GameObject obj in _everyGameObject)
            {
                if (obj.name != "Floor" && obj.name != "Ceiling" && obj.GetComponent<Transform>() != null && obj.GetComponent<RectTransform>() == null && StaticClass.minimapType == 1)
                {
                    if (obj.tag == "StaticWall" && posLatest != new Vector3(obj.transform.position.x + posOffset.x, obj.transform.position.z + posOffset.y, 0))
                    {
                        var ws = Instantiate(wallSpritePrefab, mapObjectsRoot.transform);
                        ws.transform.position = new Vector3(obj.transform.position.x + posOffset.x, obj.transform.position.z + posOffset.y, 0);
                        posLatest = ws.transform.position;
                    }
                    else if (obj.tag == "Player")
                    {
                        _playerObject = obj;
                        _playerSprite = Instantiate(playerSpritePrefab, mapObjectsRoot.transform);
                    }
                }
            }
        }
        else if (StaticClass.minimapType == 2)
        {
            _playerObject = GameObject.FindGameObjectWithTag("Player");
            _playerSprite = Instantiate(playerSpritePrefab, mapObjectsRoot.transform);
        }
    }

    void Update()
    {
        if (HUD.minimapEnabled && StaticClass.gameState == 0)
        {
            if (StaticClass.minimapType == 1)
            {
                _playerSprite.transform.localPosition = new Vector3(_playerObject.transform.position.x + posOffset.x - 640, _playerObject.transform.position.z + posOffset.y - 400, 0);
            }
            else if (StaticClass.minimapType == 2)
            {
                _playerSprite.transform.localPosition = new Vector3(_playerObject.transform.position.x + posOffset.x, _playerObject.transform.position.z + posOffset.y, 0);
            }

            if (StaticClass.minimapType != 0)
            {
                mapObjectsRoot.transform.position -= transform.right * Input.GetAxisRaw("Horizontal") * scrollScale * Time.deltaTime;
                mapObjectsRoot.transform.position -= transform.up * Input.GetAxisRaw("Vertical") * scrollScale * Time.deltaTime;

                if (StaticClass.minimapType == 2)
                {
                    if (mapObjectsRoot.transform.localPosition == Vector3.zero)
                    {
                        mapObjectsRoot.transform.localPosition = _playerSprite.transform.localPosition * -1;
                    }
                }

                if (Input.GetAxis("Mouse ScrollWheel") > 0 && mapObjectsRoot.transform.localScale.x < 8f)
                {
                    mapObjectsRoot.transform.localScale += new Vector3(zoomScale, zoomScale, zoomScale);
                    mapObjectsRoot.transform.localPosition -= transform.right * _playerObject.transform.position.x * zoomScale;
                    mapObjectsRoot.transform.localPosition -= transform.up * _playerObject.transform.position.z * zoomScale;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0 && mapObjectsRoot.transform.localScale.x > 0.8f)
                {
                    mapObjectsRoot.transform.localScale -= new Vector3(zoomScale, zoomScale, zoomScale);
                    mapObjectsRoot.transform.localPosition += transform.right * _playerObject.transform.position.x * zoomScale;
                    mapObjectsRoot.transform.localPosition += transform.up * _playerObject.transform.position.z * zoomScale;
                }
            }
        }
    }

    #endregion

    #region Add

    // Check conditions and then add to minimap.
    public void AddToMinimapFilter(GameObject obj)
    {
        if (_filteredObjects.Contains(obj) == false && obj.layer != 5 && Time.timeScale != 0.0f)
        {
            if (obj.tag == "StaticWall")
            {
                // Adds static walls to minimap. Checks the object tag because the "isStatic" boolean doesn't work for builds!
                AddToMinimap(obj, obj.transform.position, true);
            }
            else if (obj.tag == "MovingWall")
            {
                // Adds secret walls to minimap. Only adds them once, based on their initial position.
                if (obj.GetComponent<MovingWall>().canBeAddedToMinimap)
                {
                    AddToMinimap(obj, obj.transform.position, true);
                    obj.GetComponent<MovingWall>().canBeAddedToMinimap = false;
                }
            }
            else if (obj.GetComponent<Door>() != null)
            {
                // Adds doors to minimap. Only adds them once, and only if they are closed.
                if (obj.GetComponent<Door>().doorState == 0)
                {
                    AddToMinimap(obj, obj.transform.position, true);
                }
            }
        }
    }

    // Add floor to the minimap, with an object's position as reference.
    public void AddFloorToMinimap(GameObject reference)
    {
        Vector3 pos = new Vector3(Mathf.RoundToInt(reference.transform.position.x), 0, Mathf.RoundToInt(reference.transform.position.z));

        if (_usedPositions.Contains(pos) == false && Time.timeScale != 0.0f && (pos.x % 2 == 0 || pos.z % 2 == 0))
        {
            AddToMinimap(reference, pos, false);
            _usedPositions.Add(pos);
            _lastFloorPosition = pos;
            //Debug.Log("Added floor to minimap at " + pos);
        }
    }

    // Adds to minimap without checking conditions.
    void AddToMinimap(GameObject obj, Vector3 pos, bool filtered)
    {
        if (_usedPositions.Contains(new Vector3(pos.x + posOffset.x, pos.z + posOffset.y, 0)) == false && Time.timeScale != 0.0f)
        {
            GameObject prefabToAdd = wallSpritePrefab;

            if (obj.GetComponent<Door>() != null)
            {
                prefabToAdd = doorSpritePrefab[obj.GetComponent<Door>().key];
            }
            if (obj.tag == "Player")
            {
                prefabToAdd = floorSpritePrefab;
            }

            ////// Reset position start //////
            _playerSprite.transform.SetParent(null);

            _previousMinimapPosition = mapObjectsRoot.transform.position;
            _previousMinimapScale = mapObjectsRoot.transform.localScale;

            mapObjectsRoot.transform.position = new Vector3(0, 0, 0);
            mapObjectsRoot.transform.localScale = new Vector3(1, 1, 1);
            ////// Reset position end //////

            // Instantiate prefab on the minimap
            var ws = Instantiate(prefabToAdd, mapObjectsRoot.transform);
            ws.transform.localPosition = new Vector3(pos.x + posOffset.x, pos.z + posOffset.y, 0);
            _usedPositions.Add(ws.transform.position);

            if (filtered == true)
            {
                _filteredObjects.Add(obj);
            }

            //Debug.Log("Added " + obj.name + " to minimap");

            ////// Restore position start //////
            mapObjectsRoot.transform.position = _previousMinimapPosition;
            mapObjectsRoot.transform.localScale = _previousMinimapScale;

            _playerSprite.transform.SetParent(mapObjectsRoot.transform);
            ////// Restore position end //////
        }
    }

    #endregion
}
