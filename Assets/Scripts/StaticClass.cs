using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticClass
{
    public static int gameState = 0;
    // State 0 = Normal gameplay
    // State 1 = Finishing level
    // State 2 = Death

    public static int chapterReadOnly = 1;
    public static int mapReadOnly = 1;

    public static int difficulty = 1;

    public static int secretsTotal = 0;
    public static int secretsDiscovered = 0;

    public static int enemiesTotal = 0;
    public static int enemiesKilled = 0;

    public static bool loadSavedPlayerInfo = false;
}