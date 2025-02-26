using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStats : MonoBehaviour
{
    void Update()
{
    if (Input.GetKeyDown(KeyCode.UpArrow)) 
    {
        if (PlayerStats.Instance == null || PlayerStats.Instance.stats == null)
        {
            Debug.LogError("Error: PlayerStats.Instance o stats es null.");
            return;
        }

        PlayerStats.Instance.GainExp(50);
        PlayerStats.Instance.TakeDamage(10);

        Debug.Log("Se presionó Flecha Arriba");
        Debug.Log(" +50 EXP | -10 Vida");
        Debug.Log("Nivel: " + PlayerStats.Instance.stats.level);
        Debug.Log("Vida: " + PlayerStats.Instance.stats.currentHealth + "/" + PlayerStats.Instance.stats.maxHealth);
        Debug.Log("Mana: " + PlayerStats.Instance.stats.currentMana + "/" + PlayerStats.Instance.stats.maxMana);
        Debug.Log("XP: " + PlayerStats.Instance.stats.experience + "/" + PlayerStats.Instance.stats.experienceToNextLevel);
        Debug.Log("Fuerza: " + PlayerStats.Instance.stats.strength);
        Debug.Log("Agilidad: " + PlayerStats.Instance.stats.agility);
        Debug.Log("Inteligencia: " + PlayerStats.Instance.stats.intelligence);
    }
}
}
