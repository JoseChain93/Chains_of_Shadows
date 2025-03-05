using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroJuego : MonoBehaviour
{
    public string nombreEscenaMenuPrincipal = "Mapa1_Dialogos"; // Nombre de la escena siguiente
    public CharacterStats stats;

    void Update()
    {
        // Al presionar Space o Z, se cambia de escena
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z))
        {
            CargarMenuPrincipal();
        }
    }

    void CargarMenuPrincipal()
    {
        // Reiniciar las estadísticas del jugador antes de cargar el menú
        if (stats != null)
        {
            stats.ResetStats();
        }

        // Verifica si se ha asignado el nombre de la escena y la carga
        if (!string.IsNullOrEmpty(nombreEscenaMenuPrincipal))
        {
            SceneManager.LoadScene(nombreEscenaMenuPrincipal);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el nombre de la escena del menú principal.");
        }
    }
}
