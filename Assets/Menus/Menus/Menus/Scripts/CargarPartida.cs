using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CargarPartida : MonoBehaviour
{
    public Button loadGameButton;  // Asigna el botón desde el inspector

    private void Start()
    {
        // Activa el botón solo si existe una partida guardada
        loadGameButton.interactable = File.Exists(Application.persistentDataPath + "/playerdata.json");

        // Asigna la función al botón
        loadGameButton.onClick.AddListener(LoadGame);
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/playerdata.json"))
        {
            SaveSystem.LoadPlayerFromMainMenu();
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró una partida guardada.");
        }
    }
}
