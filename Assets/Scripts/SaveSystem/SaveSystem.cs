using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityEditor;

public static class SaveSystem
{
    public static string SavePath(int slot, string identifier)
    {
        return Application.persistentDataPath + "/" + StaticClass.SLOT_PREFIX + slot.ToString() + "_" + identifier + StaticClass.SAVEGAME_FILETYPE;
    }

    public static void SaveGame(int slot)
    {
        PlayerPrefs.SetString(StaticClass.SLOT_PREFIX + slot.ToString() + "_scene_name", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetInt(StaticClass.SLOT_PREFIX + slot.ToString() + "_difficulty", StaticClass.difficulty);
        PlayerPrefs.SetInt(StaticClass.SLOT_PREFIX + slot.ToString() + "_score", Player.score);
        PlayerPrefs.SetInt(StaticClass.SLOT_PREFIX + slot.ToString() + "_scoreThisLevel", Player.scoreThisLevel);
        PlayerPrefs.SetInt(StaticClass.SLOT_PREFIX + slot.ToString() + "_timeSeconds", Player.timeSeconds);
        PlayerPrefs.SetInt(StaticClass.SLOT_PREFIX + slot.ToString() + "_timeMinutes", Player.timeMinutes);
        PlayerPrefs.Save();

        Debug.Log("Score this chapter: " + PlayerPrefs.GetInt(StaticClass.SLOT_PREFIX + slot.ToString() + "_score"));
        Debug.Log("Score this level: " + PlayerPrefs.GetInt(StaticClass.SLOT_PREFIX + slot.ToString() + "_scoreThisLevel"));

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
            if (StaticClass.debug == true)
            {
                Debug.Log("Loading slot " + slot + "...");
            }

            StaticClass.pendingLoad = slot;
            SceneManager.LoadScene(PlayerPrefs.GetString(StaticClass.SLOT_PREFIX + slot.ToString() + "_scene_name"));
        }
        else
        {
            StaticClass.pendingLoad = -1;
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().LoadFromPlayerData(slot);

            if (StaticClass.debug == true)
            {
                Debug.Log("Loaded slot " + slot);
            }
        }
    }

    public static void SavePlayer(Player player, int slot)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = SavePath(slot, "player");
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();

        if(StaticClass.debug == true)
        {
            Debug.Log("SAVED player info on slot " + slot + " at " + path);
        }
    }

    public static PlayerData LoadPlayer(int slot)
    {
        StaticClass.difficulty = PlayerPrefs.GetInt(StaticClass.SLOT_PREFIX + slot.ToString() + "_difficulty");
        Player.score = PlayerPrefs.GetInt(StaticClass.SLOT_PREFIX + slot.ToString() + "_score");
        Player.scoreThisLevel = PlayerPrefs.GetInt(StaticClass.SLOT_PREFIX + slot.ToString() + "_scoreThisLevel");
        Player.timeSeconds = PlayerPrefs.GetInt(StaticClass.SLOT_PREFIX + slot.ToString() + "_timeSeconds");
        Player.timeMinutes = PlayerPrefs.GetInt(StaticClass.SLOT_PREFIX + slot.ToString() + "_timeMinutes");

        string path = SavePath(slot, "player");

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            if (StaticClass.debug == true)
            {
                Debug.Log("LOADED player info on slot " + slot + " at " + path);
            }

            return data;
        }
        else
        {
            Debug.Log("Player save file not found in " + path);
            return null;
        }
    }

    public static void DeleteSave(int slot, string identifier)
    {
        string path = SavePath(slot, identifier);

        if(File.Exists(path))
        {
            File.Delete(path);

            if (StaticClass.debug == true)
            {
                Debug.Log("DELETED " + identifier + " info on slot " + slot + " at " + path);
            }
        }
    }

    public static bool SaveExists(int slot, string identifier)
    {
        string path = SavePath(slot, identifier);

        if(File.Exists(path) && PlayerPrefs.HasKey(StaticClass.SLOT_PREFIX + slot.ToString() + "_scene_name"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
