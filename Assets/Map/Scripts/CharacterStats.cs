using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    // Nivel y experiencia
    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100;

    // Atributos base
    public int strength = 10;  // Aumenta ataque y vida
    public int agility = 8;    // Aumenta evasión
    public int intelligence = 4; // Aumenta mana y poder mágico

    // Vida y Maná
    public int maxHealth;
    public int currentHealth;
    public int maxMana;
    public int currentMana;

    // Daño basado en atributos
    public int attackPower;
    public int magicPower;

    public CharacterStats()
    {
        CalculateStats();
    }

    public void GainExperience(int amount)
    {
        experience += amount;
        
        while (experience >= experienceToNextLevel)
        {
            experience -= experienceToNextLevel; // Resta el exceso de experiencia
            LevelUp();
        }
    }

    public void LevelUp()
    {
        level++;
        experienceToNextLevel += 50; // Aumenta el requisito de XP

        // Aumenta atributos base
        strength += 2;
        agility += 1;
        intelligence += 1;

        // Recalcula estadísticas
        CalculateStats();

        //Restaurar salud y maná al máximo
        currentHealth = maxHealth;
        currentMana = maxMana;

        Debug.Log("¡Subiste al nivel " + level + "!");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
    }

    public void Heal(int amount)
    {
        
        int healingAmount = amount + (intelligence * 2);  // Ajusta el multiplicador según lo que necesites
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void RestoreMana(int amount)
    {
        currentMana += amount;
        if (currentMana > maxMana) currentMana = maxMana;
    }

    public void CalculateStats()
    {
        maxHealth = 100 + (strength * 2);
        maxMana = 50 + (intelligence * 1);
        attackPower = strength * 1;
        magicPower = intelligence * 1;
    }

    public void ResetStats()
    {
        level = 1;
        experience = 0;
        experienceToNextLevel = 100;

        strength = 10;
        agility = 8;
        intelligence = 4;
        maxHealth = 120;
        currentHealth = 120;
        maxMana = 54;
        currentMana = 54;
        attackPower = 10;
        magicPower = 4;

        CalculateStats();

        currentHealth = maxHealth;
        currentMana = maxMana;

      
    }
}