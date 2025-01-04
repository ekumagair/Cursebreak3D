using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEditor;

public static class SaveSystem
{
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Slot save
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static string SaveSlotPath(int slot, string identifier)
    {
        return Application.persistentDataPath + "/" + StaticClass.SLOT_PREFIX + slot.ToString() + "_" + identifier + StaticClass.SAVEGAME_FILETYPE;
    }

    public static void SaveGame(int slot)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().SaveToPlayerData(slot);
    }

    public static void LoadGame(int slot)
    {
        StaticClass.ResetStats(false);
        StaticClass.loadSavedPlayerInfo = true;
        StaticClass.loadSavedPlayerFullInfo = true;
        StaticClass.loadSavedMapData = true;

        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            // If loading the game from a scene without the player object. (A menu)
            if (Debug.isDebugBuild == true)
            {
                Debug.Log("Loading slot " + slot + "... (Scene " + LoadPlayer(slot).scene.ToString() + ")");
            }

            StaticClass.pendingLoad = slot;
            SceneManager.LoadScene(LoadPlayer(slot).scene.ToString());
        }
        else
        {
            // If loading the game from a scene that has the player object.
            StaticClass.pendingLoad = -1;
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().LoadFromPlayerData(slot);

            if (Debug.isDebugBuild == true)
            {
                Debug.Log("Loaded slot " + slot);
            }
        }
    }

    public static void SavePlayer(Player player, int slot)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = SaveSlotPath(slot, "player");
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();

        if (Debug.isDebugBuild == true)
        {
            Debug.Log("SAVED player info on slot " + slot + " at " + path);
        }
    }

    public static PlayerData LoadPlayer(int slot)
    {
        string path = SaveSlotPath(slot, "player");

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            if (Debug.isDebugBuild == true)
            {
                Debug.Log("LOADED player info on slot " + slot + " at " + path);
            }

            return data;
        }
        else
        {
            if (Debug.isDebugBuild == true)
            {
                Debug.Log("Player save file not found in " + path);
            }

            return null;
        }
    }

    public static void DeleteSave(int slot, string identifier)
    {
        string path = SaveSlotPath(slot, identifier);

        if (File.Exists(path))
        {
            File.Delete(path);

            if (Debug.isDebugBuild == true)
            {
                Debug.Log("DELETED " + identifier + " info on slot " + slot + " at " + path);
            }
        }
    }

    public static bool SaveExists(int slot, string identifier)
    {
        string path = SaveSlotPath(slot, identifier);

        if (File.Exists(path) || PlayerPrefs.HasKey(StaticClass.SLOT_PREFIX + slot.ToString() + "_scene_name"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Global save
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static string GlobalSavePath()
    {
        return Application.persistentDataPath + "/global.dat";
    }

    public static void SaveGlobal()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(GlobalSavePath(), FileMode.Create);

        GlobalData data = new GlobalData();

        formatter.Serialize(stream, data);
        stream.Close();

        if (Debug.isDebugBuild == true)
        {
            Debug.Log("SAVED global info at " + GlobalSavePath());
        }
    }

    public static GlobalData GetSavedGlobal()
    {
        if (File.Exists(GlobalSavePath()))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(GlobalSavePath(), FileMode.Open);

            GlobalData data = formatter.Deserialize(stream) as GlobalData;
            stream.Close();

            /*
            if (Debug.isDebugBuild == true)
            {
                Debug.Log("LOADED global info at " + GlobalSavePath());
            }*/
            return data;
        }
        else
        {
            /*
            if (Debug.isDebugBuild == true)
            {
                Debug.Log("Global file not found in " + GlobalSavePath());
            }*/
            return null;
        }
    }

    public static void LoadGlobal()
    {
        GlobalData data = GetSavedGlobal();

        if (data != null)
        {
            Options.mouseSensitivity = data.mouseSensitivity;
            Options.musicVolume = data.musicVolume;
            Options.soundVolume = data.soundVolume;
            Crosshair.sprite = data.crosshairSprite;
            Options.flashingEffects = data.flashingEffects;
            Options.gameplayLowRes = data.gameplayLowRes;

            StaticClass.unlockedChapter = GetSavedGlobal().unlockedChapters;

            for (int i = 0; i < StaticClass.chapterHighScore.Length; i++)
            {
                StaticClass.chapterHighScore[i] = data.chapterHighScore[i];
            }

            if (data.gameResolution.GetType() != null)
            {
                Options.gameResolution = data.gameResolution;
            }
        }
        else
        {
            Options.ResetOptions();
        }

        SaveGlobal();
    }
}
