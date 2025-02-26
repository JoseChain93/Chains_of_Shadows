using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/playerdata.json";

    public static void SavePlayer(PlayerStats player)
    {
        PlayerData data = new PlayerData(player);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log($"Juego guardado en: {path}");
    }

    public static void LoadPlayer(PlayerStats player)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("No se encontró ningún archivo de guardado.");
            return;
        }

        string json = File.ReadAllText(path);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        // Cargar estadísticas
        player.playerName = data.playerName;
        player.stats.level = data.level;
        player.stats.experience = data.experience;
        player.stats.experienceToNextLevel = data.experienceToNextLevel;

        player.stats.strength = data.strength;
        player.stats.agility = data.agility;
        player.stats.intelligence = data.intelligence;

        player.stats.maxHealth = data.maxHealth;
        player.stats.currentHealth = data.currentHealth;
        player.stats.maxMana = data.maxMana;
        player.stats.currentMana = data.currentMana;

        player.stats.attackPower = data.attackPower;
        player.stats.magicPower = data.magicPower;

        player.UpdateUI();

        // Si la escena actual es diferente de la guardada, carga la escena primero
        if (SceneManager.GetActiveScene().name != data.sceneName)
        {
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                player.transform.position = new Vector3(data.positionX, data.positionY, data.positionZ);
                
            };

            
            SceneManager.LoadScene(data.sceneName);
        }
        else
        {
            // Si ya estás en la escena, solo actualiza la posición
            player.transform.position = new Vector3(data.positionX, data.positionY, data.positionZ);
           
        }
    }

    public static void LoadPlayerFromMainMenu()
{
    string path = Application.persistentDataPath + "/playerdata.json";

    if (!File.Exists(path))
    {
        Debug.LogWarning("No hay datos guardados.");
        return;
    }

    string json = File.ReadAllText(path);
    PlayerData data = JsonUtility.FromJson<PlayerData>(json);

    SceneManager.sceneLoaded += (scene, mode) =>
    {
        PlayerStats player = PlayerStats.Instance;
        if (player != null)
        {
            player.transform.position = new Vector3(data.positionX, data.positionY, data.positionZ);

            player.playerName = data.playerName;
            player.stats.level = data.level;
            player.stats.experience = data.experience;
            player.stats.experienceToNextLevel = data.experienceToNextLevel;

            player.stats.strength = data.strength;
            player.stats.agility = data.agility;
            player.stats.intelligence = data.intelligence;

            player.stats.maxHealth = data.maxHealth;
            player.stats.currentHealth = data.currentHealth;
            player.stats.maxMana = data.maxMana;
            player.stats.currentMana = data.currentMana;

            player.stats.attackPower = data.attackPower;
            player.stats.magicPower = data.magicPower;

            player.UpdateUI();
            
        }
        else
        {
            Debug.LogError("PlayerStats no encontrado en la escena.");
        }
    };

   
    SceneManager.LoadScene(data.sceneName);
}
}