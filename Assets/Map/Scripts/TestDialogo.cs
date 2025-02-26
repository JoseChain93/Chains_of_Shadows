using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDialogo : MonoBehaviour
{
    void Start()
    {
        // Verificar si "HasShownDialogue" existe en PlayerPrefs
        if (!PlayerPrefs.HasKey("HasShownDialogue"))
        {
            // Si no existe, lo inicializamos con el valor 0 (no mostrado)
            PlayerPrefs.SetInt("HasShownDialogue", 0);
            PlayerPrefs.Save(); // Guardar los cambios
            Debug.Log("HasShownDialogue no existía, lo hemos inicializado a 0");
        }
        
        // Obtener y mostrar el valor de "HasShownDialogue"
        int hasShownDialogue = PlayerPrefs.GetInt("HasShownDialogue", 0);
        Debug.Log("Estado actual de HasShownDialogue: " + hasShownDialogue);
    }
}
