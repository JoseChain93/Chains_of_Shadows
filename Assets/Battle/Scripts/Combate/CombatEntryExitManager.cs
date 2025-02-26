using System.Collections;
using System.Collections.Generic;
using UnityEngine;            
using UnityEngine.SceneManagement; 

public class CombatEntryExitManager : MonoBehaviour
{
    public static CombatEntryExitManager Instance { get; private set; }  // Singleton

    void Awake()
    {
        // Asegurarse de que haya solo una instancia de SceneController
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruir esta instancia si ya existe una
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No destruir al cambiar de escena
        }
    }

    // Guardar la escena y la posición del jugador antes de entrar en combate
    public void SaveSceneAndPosition()
    {
        // Guardar la escena actual
        PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);

        // Guardar la posición del jugador
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerPrefs.SetFloat("PrevX", player.transform.position.x);
            PlayerPrefs.SetFloat("PrevY", player.transform.position.y);
            PlayerPrefs.SetFloat("PrevZ", player.transform.position.z);
        }

        PlayerPrefs.Save();  // Guardar los cambios
    }

    // Restaurar la escena y la posición cuando el jugador regresa al Overworld
    public void RestoreSceneAndPosition()
    {
        // Restaurar la escena
        if (PlayerPrefs.HasKey("PreviousScene"))
        {
            string previousScene = PlayerPrefs.GetString("PreviousScene");
            SceneManager.LoadScene(previousScene);  // Cargar la escena anterior
        }

        // Restaurar la posición
        if (PlayerPrefs.HasKey("PrevX") && PlayerPrefs.HasKey("PrevY") && PlayerPrefs.HasKey("PrevZ"))
        {
            float prevX = PlayerPrefs.GetFloat("PrevX");
            float prevY = PlayerPrefs.GetFloat("PrevY");
            float prevZ = PlayerPrefs.GetFloat("PrevZ");

            // Encontrar al jugador y restaurar su posición
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(prevX, prevY, prevZ);
                Debug.Log("Jugador regresó a la posición anterior.");
            }

            // Limpiar los datos guardados
            PlayerPrefs.DeleteKey("PrevX");
            PlayerPrefs.DeleteKey("PrevY");
            PlayerPrefs.DeleteKey("PrevZ");
            PlayerPrefs.DeleteKey("PreviousScene");
            PlayerPrefs.Save();
        }
    }
}