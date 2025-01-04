using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticClass
{
    // Game states
    public static int gameState = 0;
    // State 0 = Normal gameplay
    // State 1 = Winning level
    // State 2 = Death
    // State 3 = Pause

    // Chapter and Map variables
    public static int currentChapter = 1;
    public static int currentMap = 1;

    // Unlocked chapter
    public static int unlockedChapter = 1;

    // Chapter high scores
    public static int[] chapterHighScore = new int[3];

    // Difficulty
    public static int difficulty = 1;
    // Normal values are from 0 to 3. Values greater than 3 will be functional but won't have special properties.
    // -2 = No enemies, all push walls revealed
    // -1 = No enemies
    // 0 = Easy
    // 1 = Normal
    // 2 = Hard
    // 3 = Very Hard

    // Current level's stats. Must be reset before starting next level.
    public static int secretsTotal = 0;
    public static int secretsDiscovered = 0;
    public static int enemiesTotal = 0;
    public static int enemiesKilled = 0;

    // Keep the player's items from one level to another. Takes effect when the next scene starts.
    public static bool loadSavedPlayerInfo = false;
    public static bool loadSavedPlayerFullInfo = false;
    public static bool loadSavedMapData = false;
    public static int pendingLoad = -1;

    // Misc rules.
    public static bool canPause = true;
    public static bool debugRays = false;

    // If true, disable chapter unlocking checks. This would make everything unlocked from the start. If false, the player must beat all chapters in order, from first to last.
    public static bool ignoreUnlockedChapter = false;

    // Minimap type.
    public static int minimapType = 2;
    // 0 = Minimap disabled
    // 1 = All revealed from the start
    // 2 = Gradual reveal

    // Intermission display type.
    public static int intermissionDisplayType = 1;
    // 0 = Show numbers instantly
    // 1 = Show numbers gradually

    // Reset stats before starting a new level. Can reset current chapter progress or not.
    public static void ResetStats(bool resetChapterProgress)
    {
        Player.scoreThisLevel = 0;
        Player.timeSeconds = 0;
        Player.timeMinutes = 0;
        Player.gotScoreTimer = 0;
        HUD.minimapEnabled = false;
        Enemy.sightSoundsPlaying = 0;
        gameState = 0;
        secretsDiscovered = 0;
        secretsTotal = 0;
        enemiesKilled = 0;
        enemiesTotal = 0;

        if (resetChapterProgress == true)
        {
            Player.score = 0;
            Player.damageStopsSprint = false;
            Player.savedCurrentWeapon = 1;
            pendingLoad = -1;
            currentChapter = 1;
            currentMap = 1;
            loadSavedPlayerInfo = false;
            loadSavedPlayerFullInfo = false;
            loadSavedMapData = false;

            Debug.Log("Reset chapter progress!");
        }

        Debug.Log("Reset stats!");
    }

    // Constants
    public const string SLOT_PREFIX = "slot";
    public const string SAVEGAME_FILETYPE = ".cursebreaksave";
}