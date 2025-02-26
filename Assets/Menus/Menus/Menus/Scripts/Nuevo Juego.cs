using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Método que se llama para cargar la escena
    public void LoadGameScene()
    {
        // Inicia la corrutina para cargar la escena y luego activar el objeto
        StartCoroutine(LoadSceneAndActivate());
    }

    // Corrutina para cargar la escena de manera asíncrona y luego activar el GameObject
    private IEnumerator LoadSceneAndActivate()
    {
        // Carga la escena de manera asíncrona
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);

        // Espera hasta que la escena esté completamente cargada
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        // Una vez que la escena esté cargada, busca y activa el GameObject
        GameObject personaje = GameObject.Find("Solomon");

        if (personaje != null)
        {
            personaje.SetActive(true); // Activa el personaje
        }
    }
}
