using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Make sure to include this for scene management

public class QuitGame : MonoBehaviour
{
    void Start()
    {
        // Subscribe to scene change event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        // Detecta si se presiona la tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
        }
    }

    void Exit()
    {
        // Cierra el juego
        Application.Quit();
    }

    // This method is called when a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Destroy this GameObject once the scene is loaded
        Destroy(gameObject);
    }

    // Unsubscribe from the event when the script is destroyed to prevent memory leaks
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
