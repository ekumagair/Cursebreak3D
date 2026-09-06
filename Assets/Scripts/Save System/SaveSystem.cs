using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Slot save
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static string SaveSlotPath(int slot)
    {
        string fileName = StaticClass.SLOT_PREFIX + slot.ToString() + "_" + StaticClass.PDATA_IDENTIFIER + StaticClass.PDATA_FILETYPE;
        return Path.Combine(Application.persistentDataPath, fileName);
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
        PlayerData data = new PlayerData(player);

#if !PLAYER_PREF_SAVE
        string path = SaveSlotPath(slot);
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();

        if (Debug.isDebugBuild == true)
        {
            Debug.Log("SAVED player info on slot " + slot + " at " + path);
        }
#else
        string dataAsString;

        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, data);
            dataAsString = Convert.ToBase64String(stream.ToArray());

            PlayerPrefs.SetString(StaticClass.PLAYER_PREF_PDATA_KEY + slot.ToString(), dataAsString);
            PlayerPrefs.Save();
        }

        if (Debug.isDebugBuild == true)
        {
            Debug.Log("SAVED player info on slot " + slot + " as player pref PlayerData: " + dataAsString);
        }
#endif
    }

    public static PlayerData LoadPlayer(int slot)
    {
        BinaryFormatter formatter = new BinaryFormatter();

#if !PLAYER_PREF_SAVE
        string path = SaveSlotPath(slot);

        if (File.Exists(path))
        {
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
#else
        string data = PlayerPrefs.GetString(StaticClass.PLAYER_PREF_PDATA_KEY + slot.ToString(), "");

        if (string.IsNullOrEmpty(data))
        {
            return null;
        }

        byte[] bytes = Convert.FromBase64String(data);

        using (MemoryStream stream = new MemoryStream(bytes))
        {
            return (PlayerData)formatter.Deserialize(stream);
        }
#endif
    }

    public static void DeleteSave(int slot)
    {
#if !PLAYER_PREF_SAVE
        string path = SaveSlotPath(slot);

        if (File.Exists(path))
        {
            File.Delete(path);

            if (Debug.isDebugBuild == true)
            {
                Debug.Log("DELETED player info on slot " + slot + " at " + path);
            }
        }
#else
        if (PlayerSaveExists(slot))
        {
            PlayerPrefs.DeleteKey(StaticClass.PLAYER_PREF_PDATA_KEY + slot.ToString());
            PlayerPrefs.Save();
        }
#endif
    }

    public static bool PlayerSaveExists(int slot)
    {
#if !PLAYER_PREF_SAVE
        string path = SaveSlotPath(slot);
        return File.Exists(path);
#else
        return PlayerPrefs.HasKey(StaticClass.PLAYER_PREF_PDATA_KEY + slot.ToString());
#endif
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Global save
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static string GlobalSavePath()
    {
        return Path.Combine(Application.persistentDataPath, StaticClass.GDATA_IDENTIFIER + StaticClass.GDATA_FILETYPE);
    }

    public static void SaveGlobal()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        GlobalData data = new GlobalData();

#if !PLAYER_PREF_SAVE
        FileStream stream = new FileStream(GlobalSavePath(), FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();

        if (Debug.isDebugBuild == true)
        {
            Debug.Log("SAVED global info at " + GlobalSavePath());
        }
#else
        string dataAsString;

        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, data);
            dataAsString = Convert.ToBase64String(stream.ToArray());

            PlayerPrefs.SetString(StaticClass.PLAYER_PREF_GDATA_KEY, dataAsString);
            PlayerPrefs.Save();
        }

        if (Debug.isDebugBuild == true)
        {
            Debug.Log("SAVED global info as player pref GlobalData: " + dataAsString);
        }
#endif
    }

    public static GlobalData GetSavedGlobal()
    {
        BinaryFormatter formatter = new BinaryFormatter();

#if !PLAYER_PREF_SAVE
        if (File.Exists(GlobalSavePath()))
        {
            FileStream stream = new FileStream(GlobalSavePath(), FileMode.Open);

            GlobalData data = formatter.Deserialize(stream) as GlobalData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
#else
        string data = PlayerPrefs.GetString(StaticClass.PLAYER_PREF_GDATA_KEY, "");

        if (string.IsNullOrEmpty(data))
        {
            return null;
        }

        byte[] bytes = Convert.FromBase64String(data);

        using (MemoryStream stream = new MemoryStream(bytes))
        {
            return (GlobalData)formatter.Deserialize(stream);
        }
#endif
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
