using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeEscenaMapa4MuroDer : MonoBehaviour
{
    public string nombreEscena;  // Nombre de la escena a cargar

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Acceder al PlayerController y bloquear combate/transiciones
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
            playerController.isInTransition = true; // Evita entrar en combate durante el cambio de escena
            }

            // Encuentra y destruye a Solomon antes de cargar la nueva escena
            GameObject Solomon = GameObject.Find("Solomon");
            if (Solomon != null)
            {
                // Si solomon tiene DontDestroyOnLoad, desvincúlalo y destrúyelo
                Solomon.transform.SetParent(null);  
                Destroy(Solomon);  // Se destruirá antes de la nueva escena si se hace así
            }

            UpdatePlayerStats();

            // Suscribir el método para mover al jugador tras la carga de la escena
            SceneManager.sceneLoaded += MoverJugador;

           StartCoroutine(CargarEscenaConRetraso("Mapa5"));
        }
    }

    private IEnumerator CargarEscenaConRetraso(string sceneIndex)
    {
        yield return new WaitForEndOfFrame();  // Espera un frame para asegurar que Destroy se procese
        SceneManager.LoadScene(sceneIndex);
    }

    private void MoverJugador(Scene scene, LoadSceneMode mode)
    {
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador != null)
        {
            jugador.transform.position = new Vector3(5.414443f, -3.7615f, jugador.transform.position.z);

            Vector3 escalaJugador = jugador.transform.localScale;
            escalaJugador.x = -Mathf.Sign(escalaJugador.x) * Mathf.Abs(escalaJugador.x);  // Asegura que mire a la izquierda
            jugador.transform.localScale = escalaJugador;
        }

        SceneManager.sceneLoaded -= MoverJugador;  // Evita múltiples suscripciones
    }

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
