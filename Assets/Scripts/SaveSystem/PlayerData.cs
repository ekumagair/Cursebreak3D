using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    public string scene;
    public int health;
    public int armor;
    public float armorMult;
    public bool[] weaponsUnlocked = new bool[7];
    public int[] ammo = new int[3];
    public float[] position;
    public float rotation;

    public string[] enemyStartPositions;
    public string[] killedEnemies;
    public string[] destroyedItemsPositions;
    public string[] discoveredSecrets;

    public PlayerData(Player player)
    {
        scene = SceneManager.GetActiveScene().name;
        health = player.GetComponent<Health>().health;
        armor = player.GetComponent<Health>().armor;
        armorMult = player.GetComponent<Health>().armorMult;

        for (int i = 0; i < player.ammo.Length; i++)
        {
            ammo[i] = player.ammo[i];
        }

        for (int i = 0; i < player.weaponsUnlocked.Length; i++)
        {
            weaponsUnlocked[i] = player.weaponsUnlocked[i];
        }

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        rotation = player.transform.eulerAngles.y;

        enemyStartPositions = new string[player.enemyStartPositions.Count];
        for (int i = 0; i < player.enemyStartPositions.Count; i++)
        {
            enemyStartPositions[i] = player.enemyStartPositions[i];
        }

        destroyedItemsPositions = new string[player.destroyedItemsPositions.Count];
        for (int i = 0; i < player.destroyedItemsPositions.Count; i++)
        {
            destroyedItemsPositions[i] = player.destroyedItemsPositions[i];
        }

        killedEnemies = new string[player.killedEnemies.Count];
        for (int i = 0; i < player.killedEnemies.Count; i++)
        {
            killedEnemies[i] = player.killedEnemies[i];
        }

        discoveredSecrets = new string[player.discoveredSecrets.Count];
        for (int i = 0; i < player.discoveredSecrets.Count; i++)
        {
            discoveredSecrets[i] = player.discoveredSecrets[i];
        }
    }
}
