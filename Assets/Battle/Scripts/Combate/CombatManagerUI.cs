using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CombatUIManager : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI playerMPText;
    
    //public Text hpText;       // Referencia al texto que muestra la vida (HP)
    //public Text mpText;       // Referencia al texto que muestra el maná (MP)

    // Referencia a PlayerStats (asegurándonos de que PlayerStats esté accesible)
    private PlayerStats playerStats;

    void Start()
    {
        // Asigna el PlayerStats para acceder a la información del jugador
        playerStats = PlayerStats.Instance;

        // Asegúrate de que las barras de salud y maná se actualicen al inicio
        UpdateUI();
    }

    void Update()
    {
        // Asegúrate de que la UI se actualice cada vez que el jugador recibe daño o usa maná
        UpdateUI();
    }

    // Esta función actualiza los valores de la UI (vida, maná, barras)
    void UpdateUI()
    {
        // Actualizar el texto de nombre de personaje
        if (playerNameText != null && PlayerStats.Instance != null)
        {
            playerNameText.text = PlayerStats.Instance.playerName; // Nombre del jugador
        }

        // Actualizar el texto de vida
        if (playerHPText != null && playerStats != null)
        {
            playerHPText.text = "HP: " + playerStats.stats.currentHealth + "/" + playerStats.stats.maxHealth;
        }

        // Actualizar el texto de maná
        if (playerMPText != null && playerStats != null)
        {
            playerMPText.text = "MP: " + playerStats.stats.currentMana + "/" + playerStats.stats.maxMana;
        }
    }
       }
