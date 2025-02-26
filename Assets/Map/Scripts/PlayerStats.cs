using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerStats : MonoBehaviour
{
    public string playerName = "Solomon";
    public TextMeshProUGUI nameText;      
    public TextMeshProUGUI healthText;    
    public TextMeshProUGUI manaText;      

    public static PlayerStats Instance;
    public CharacterStats stats = new CharacterStats();
   
    private void Start()
    {
        // Llamar a UpdateUI para asegurarse de que los valores de la UI estén sincronizados al inicio
        UpdateUI();
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TakeDamage(int damage)
    {
    stats.currentHealth -= damage;
    if (stats.currentHealth < 0) stats.currentHealth = 0;

    // Llamar a UpdateUI después de modificar la salud
    UpdateUI();

    if (stats.currentHealth <= 0)
    {
        Die();
    }
    }

    public void RestoreMana(int amount)
    {
    stats.currentMana += amount;
    if (stats.currentMana > stats.maxMana) stats.currentMana = stats.maxMana;

    // Llamar a UpdateUI después de restaurar maná
    UpdateUI();
    }

    public void UpdateUI()
    {
    // Actualizar el nombre del jugador
        if (nameText != null)
        {
            nameText.text = "Jugador: " + playerName; // Nombre del jugador
        }

        // Actualizar el texto de la vida
        if (healthText != null)
        {
            healthText.text = "HP: " + stats.currentHealth + "/" + stats.maxHealth;
        }

        // Actualizar el texto del maná
        if (manaText != null)
        {
            manaText.text = "MP: " + stats.currentMana + "/" + stats.maxMana;
        }
    
    }

    public void GainExp(int amount)
    {
        stats.GainExperience(amount);
    }

    void Die()
    {
        Debug.Log("El jugador ha sido derrotado.");
        gameObject.SetActive(false);
    }

    public void SaveGame()
    {
    SaveSystem.SavePlayer(this);
    }

    public void LoadGame()
    {
    SaveSystem.LoadPlayer(this);
    }
}
 