using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    private static GameOverManager instance;
    public GameObject gameOverPanel; // Referencia al panel de Game Over

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        gameOverPanel.SetActive(false); // Asegurar que está desactivado al inicio
    }

    private void Update()
    {
        if (gameOverPanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0; // Detener el juego
    }

    private void RestartGame()
    {
        Time.timeScale = 1; // Restaurar el tiempo del juego
        SceneManager.LoadScene("MainMenu"); // Asegúrate de que el nombre coincide con tu escena del menú
    }
}
