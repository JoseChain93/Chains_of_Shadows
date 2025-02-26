using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void QuitGame() // Asegúrate de que este método es público
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Detener el juego en el editor
        #else
            Application.Quit(); // Cerrar el juego en una build
        #endif
    }
}
