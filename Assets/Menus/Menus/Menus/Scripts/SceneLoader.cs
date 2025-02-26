using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IniciarJuego : MonoBehaviour
{
    void Start()
    {
        Debug.Log("SceneLoader está activo en " + gameObject.name);
    }

    public void LoadGameScene()
    {
        Debug.Log("Botón presionado: Intentando cargar la escena 'Mapa1'...");
        
        PlayerPrefs.SetInt("HasShownDialogue", 0);
        PlayerPrefs.Save(); // Guardar los cambios

        SceneManager.LoadScene("1"); // Cambia a la escena '1'
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("ESPACIO presionado: Cargando escena...");
            LoadGameScene(); // Carga la escena y permite que el diálogo se vea
        }
    }
}