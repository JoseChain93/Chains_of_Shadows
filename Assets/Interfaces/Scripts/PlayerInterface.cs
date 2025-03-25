using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerInterface : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerLevelText;
    public TextMeshProUGUI playerExpText;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI playerMPText;
    public TextMeshProUGUI playerStatsText;
    public TextMeshProUGUI playerAttributesText;
    
    private PlayerStats playerStats;

    void Start()
    {
        playerStats = PlayerStats.Instance;
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    // Método para actualizar la interfaz de usuario solo cuando sea necesario
    public void UpdateUI()
    {
        if (playerStats == null) return;

        CharacterStats stats = playerStats.stats;

        // Actualiza el nombre del jugador
        if (playerNameText != null)
        {
            playerNameText.text = playerStats.playerName;
        }

        // Actualiza el nivel y la experiencia
        if (playerLevelText != null)
        {
            playerLevelText.text = "Nivel: " + stats.level;
        }

        if (playerExpText != null)
        {
            playerExpText.text = "EXP: " + stats.experience + " / " + stats.experienceToNextLevel;
        }

        // Actualiza HP y MP
        if (playerHPText != null)
        {
            playerHPText.text = "HP: " + stats.currentHealth + " / " + stats.maxHealth;
        }

        if (playerMPText != null)
        {
            playerMPText.text = "MP: " + stats.currentMana + " / " + stats.maxMana;
        }

        // Actualiza las estadísticas del jugador (ataque y poder mágico)
        if (playerStatsText != null)
        {
            playerStatsText.text = "Ataque: " + stats.attackPower + "\n" + "Poder Mágico: " + stats.magicPower;
        }

        // Actualiza los atributos del jugador (fuerza, agilidad, inteligencia)
        if (playerAttributesText != null)
        {
            playerAttributesText.text = "Fuerza: " + stats.strength + "\n" + "Agilidad: " + stats.agility + "\n" + "Inteligencia: " + stats.intelligence;
        }
    }
}
