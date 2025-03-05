using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GuardarJuegoMenu : MonoBehaviour
{
    // Este método se llama cuando el botón de guardar es presionado
    public void OnSaveButtonClick()
    {
        // Obtener el nombre de la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Condiciones para bloquear el guardado
        bool isCombatScene = currentSceneName.Contains("Combat");
        bool isDialogScene = currentSceneName.Contains("Dialogo");
        bool isRestrictedMap = currentSceneName == "Mapa5" || currentSceneName == "Mapa6" || currentSceneName == "Mapa7";

        // Verificar si estamos en una escena permitida para el guardado
        if (!isCombatScene && !isDialogScene && !isRestrictedMap)
        {
            // Guardar el juego si las condiciones son correctas
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.SaveGame();
                Debug.Log("Juego guardado exitosamente.");
            }
            else
            {
                Debug.LogError("No se pudo acceder a PlayerStats.");
            }
        }
        else
        {
            // Mostrar un mensaje de advertencia si no se puede guardar
            Debug.LogWarning("¡No puedes guardar la partida aquí!");
        }
    }
}
