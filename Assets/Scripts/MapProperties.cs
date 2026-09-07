using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapProperties : MonoBehaviour
{
    #region Variables

    public static MapProperties Instance { get; private set; }

    // Sets the current chapter and map variables.
    public int chapter = 1;
    public int map = 1;

    // Set map-specific rules.
    public bool healthItemDoesNotAutosave = false;
    public bool armorItemDoesNotAutosave = false;

    // Controls the level's music.
    private AudioSource _as;

    #endregion

    #region Default Methods

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StaticClass.currentChapter = chapter;
        StaticClass.currentMap = map;
        _as = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Fade out music volume on death.
        if (_as != null)
        {
            if (StaticClass.gameState == 2 && _as.volume > 0)
            {
                _as.volume -= Time.deltaTime * 1.5f;
            }
        }
    }

    #endregion
}
