using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTeleport : MonoBehaviour
{
    public Vector2 posicionEscena1;  // Coordenada para la escena 1
    public Vector2 posicionEscena2;  // Coordenada para la escena 2
    // Puedes agregar más posiciones si es necesario

    // Método que se ejecuta cuando la escena es cargada
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int sceneIndex = scene.buildIndex;  // Obtener el índice de la escena cargada
        Debug.Log("Escena cargada: " + scene.name + " con índice: " + sceneIndex);

        // Verifica el índice de la escena cargada y asigna la posición correspondiente
        if (sceneIndex == 1)  // Si la escena cargada es la escena 1 (índice 1)
        {
            Debug.Log("Teletransportando al jugador a la posición de la escena 1");
            transform.position = posicionEscena1;
        }
        else if (sceneIndex == 2)  // Si la escena cargada es la escena 2 (índice 2)
        {
            Debug.Log("Teletransportando al jugador a la posición de la escena 2");
            transform.position = posicionEscena2;
        }
        else
        {
            Debug.Log("No hay coordenadas asignadas para este índice de escena.");
        }
    }
}
