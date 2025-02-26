using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int level, experience, experienceToNextLevel;
    public int strength, agility, intelligence;
    public int maxHealth, currentHealth, maxMana, currentMana;
    public int attackPower, magicPower;

    public float positionX, positionY, positionZ; // Posición del jugador
    public string sceneName;                      // Nombre de la escena

    public PlayerData(PlayerStats player)
    {
        playerName = player.playerName;
        level = player.stats.level;
        experience = player.stats.experience;
        experienceToNextLevel = player.stats.experienceToNextLevel;

        strength = player.stats.strength;
        agility = player.stats.agility;
        intelligence = player.stats.intelligence;

        maxHealth = player.stats.maxHealth;
        currentHealth = player.stats.currentHealth;
        maxMana = player.stats.maxMana;
        currentMana = player.stats.currentMana;

        attackPower = player.stats.attackPower;
        magicPower = player.stats.magicPower;

        // Posición del jugador
        positionX = player.transform.position.x;
        positionY = player.transform.position.y;
        positionZ = player.transform.position.z;

        // Escena actual
        sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
}
}