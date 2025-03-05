using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.EventSystems; 


public class ControlPanelManager : MonoBehaviour
{
    public Button botonPersonaje; // El botón que activará la interfaz
    public GameObject controlPanel; // La interfaz de estadísticas del jugador
    private bool isPanelOpen = false; // Estado de la interfaz (abierta o cerrada)

    void Start()
    {
    
        controlPanel.SetActive(false);

        
        botonPersonaje.onClick.AddListener(ToggleControlPanel);
    }

    void Update()
    {
        // Detectamos si la tecla "C" es presionada
        if (Input.GetKeyDown(KeyCode.C) && isPanelOpen)
        {
             CloseControlPanel();
        }
    }

    // Alternar entre abrir y cerrar el panel
    void ToggleControlPanel()
    {
        if (isPanelOpen)
        {
            CloseControlPanel();
        }
        else
        {
            OpenControlPanel();
        }
    }

   // Función para abrir el panel
    void OpenControlPanel()
    {
        controlPanel.SetActive(true);
        isPanelOpen = true;
    }

    // Función para cerrar el panel
    void CloseControlPanel()
    {
        controlPanel.SetActive(false);
        isPanelOpen = false;
    }
}
