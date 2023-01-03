using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticClass
{
    // Game states
    public static int gameState = 0;
    // State 0 = Normal gameplay
    // State 1 = Finishing level
    // State 2 = Death
    // State 3 = Pause

    public static int currentChapter = 1;
    public static int currentMap = 1;

    // Difficulty
    public static int difficulty = 1;
    // 0 = Easy
    // 1 = Normal
    // 2 = Hard
    // 3 = Very Hard

    // Current level's stats
    public static int secretsTotal = 0;
    public static int secretsDiscovered = 0;
    public static int enemiesTotal = 0;
    public static int enemiesKilled = 0;

    // Keep the player's items from one level to another
    public static bool loadSavedPlayerInfo = false;

    // Misc rules
    public static bool canPause = true;
    public static bool debug = true;

    // Minimap type
    public static int minimapType = 2;
    // 0 = Minimap disabled
    // 1 = All revealed from the start
    // 2 = Gradual reveal

    // Intermission display type
    public static int intermissionDisplayType = 1;
    // 0 = Show numbers instantly
    // 1 = Show numbers gradually

    // Reset stats
    public static void ResetStats()
    {
        Player.scoreThisLevel = 0;
        StaticClass.currentChapter = 1;
        StaticClass.currentMap = 1;
        StaticClass.secretsDiscovered = 0;
        StaticClass.secretsTotal = 0;
        StaticClass.enemiesKilled = 0;
        StaticClass.enemiesTotal = 0;
        StaticClass.loadSavedPlayerInfo = false;
        Player.timeSeconds = 0;
        Player.timeMinutes = 0;

        Debug.Log("Reset stats!");
    }
}