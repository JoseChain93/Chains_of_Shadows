using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.EventSystems; 


public class EstadisticasPersonaje : MonoBehaviour
{
    public Button botonPersonaje; // El botón que activará la interfaz
    public GameObject playerStatsInterface; // La interfaz de estadísticas del jugador
    private bool isInterfaceOpen = false; // Estado de la interfaz (abierta o cerrada)

    void Start()
    {
        // Asegúrate de que la interfaz esté inicialmente cerrada
        playerStatsInterface.SetActive(false);

        // Asignamos la función al evento del botón "Personaje"
        botonPersonaje.onClick.AddListener(TogglePlayerStatsInterface);
    }

    void Update()
    {
        // Detectamos si la tecla "S" es presionada
        if (Input.GetKeyDown(KeyCode.S) && isInterfaceOpen)
        {
            ClosePlayerStatsInterface();
        }
    }

    // Función para abrir o cerrar la interfaz
    void TogglePlayerStatsInterface()
    {
        if (isInterfaceOpen)
        {
            ClosePlayerStatsInterface();
        }
        else
        {
            OpenPlayerStatsInterface();
        }
    }

    // Función para abrir la interfaz
    void OpenPlayerStatsInterface()
    {
        playerStatsInterface.SetActive(true);
        isInterfaceOpen = true;
    }

    // Función para cerrar la interfaz
    void ClosePlayerStatsInterface()
    {
        playerStatsInterface.SetActive(false);
        isInterfaceOpen = false;
    }
}
