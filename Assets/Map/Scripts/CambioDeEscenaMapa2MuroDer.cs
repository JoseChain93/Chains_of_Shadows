using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeEscenaMapa2MuroDer : MonoBehaviour
{
    public string nombreEscena;  // Especifica la escena a cargar en el Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Acceder al PlayerController y bloquear combate/transiciones
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.isInTransition = true; // Evita entrar en combate durante el cambio de escena
        }

        if (other.CompareTag("Player"))
        {
            // Encuentra y destruye a Solomon antes de cargar la nueva escena
            GameObject Solomon = GameObject.Find("Solomon");
            if (Solomon != null)
            {
                // Si solomon tiene DontDestroyOnLoad, desvincúlalo y destrúyelo
                Solomon.transform.SetParent(null);  
                Destroy(Solomon);  // Se destruirá antes de la nueva escena si se hace así
            }
            
            // **No restauramos la posición guardada en ScenePositionEnterCombatData**
            // Solo actualizamos estadísticas y cargamos la nueva escena
            UpdatePlayerStats();
            SceneManager.sceneLoaded += MoverJugador;
            SceneManager.LoadScene("Mapa3");
        }
    }


    private void MoverJugador(Scene scene, LoadSceneMode mode)
    {
        // Busca el objeto jugador en la nueva escena y cambia su posición
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador != null)
        {
            jugador.transform.position = new Vector3(-7.21f, -3.81f, jugador.transform.position.z);
            Vector3 escalaJugador = jugador.transform.localScale;

           
        }

        // Desvincula el evento para evitar que se ejecute más de una vez
        SceneManager.sceneLoaded -= MoverJugador;
    }

    // Función para actualizar las estadísticas del jugador
    private void UpdatePlayerStats()
    {
        if (PlayerStats.Instance == null || PlayerStats.Instance.stats == null)
        {
            Debug.LogError("Error: PlayerStats.Instance o PlayerStats.Instance.stats es null.");
            return;
        }

        PlayerStats.Instance.stats.currentHealth = PlayerStats.Instance.stats.maxHealth;
        PlayerStats.Instance.stats.currentMana = PlayerStats.Instance.stats.maxMana;
        PlayerStats.Instance.stats.CalculateStats();
    }
}

