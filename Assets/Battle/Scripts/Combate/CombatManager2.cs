using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager2 : MonoBehaviour
{
    public Player player;
    public Enemy enemy1;
    public Enemy enemy2;

    public LogPanelManager logPanelManager;  // Referencia al LogPanelManager

    public void PlayerTurn()
    {
        Debug.Log("¡Es el turno del jugador!");

        // Llamar a StartTurn del jugador para gestionar efectos como necrosis
        player.StartTurn();
    }

    // Puedes modificar esta función para elegir aleatoriamente el tipo de ataque
    public void EnemyTurn()
    {
        if (enemy1 != null && enemy1.health > 0)
        {
            Debug.Log("¡Es el turno del enemigo 1!");
            enemy1.StartTurn();
            bool useLifeSteal = Random.value > 0.2f; // Ataque chupavidas con un 20% de probabilidad
            enemy1.AttackPlayer(player, useLifeSteal);
        }

        if (enemy2 != null && enemy2.health > 0)
        {
            Debug.Log("¡Es el turno del enemigo 2!");
            enemy2.StartTurn(); // Corregido aquí: debe ser enemy2
            bool useLifeSteal = Random.value < 0.3f; // Ataque chupavidas con un 30% de probabilidad
            enemy2.AttackPlayer(player, useLifeSteal);
        }

        if (PlayerStats.Instance.stats.currentHealth <= 0)
        {
            Debug.Log("Fin del juego. El jugador ha muerto.");
        }

        else if ((enemy1 == null && enemy2 == null) && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            Debug.Log("¡Victoria! Todos los enemigos han sido derrotados.");

            // Comprobar si hay una escena anterior guardada y que no sea una escena de combate
            if (!string.IsNullOrEmpty(ScenePositionEnterCombatData.ultimaEscena) &&
                !ScenePositionEnterCombatData.ultimaEscena.Contains("SceneCombat"))
            {
                // **Restablecer la bandera de transición**
                PlayerController playerController = FindObjectOfType<PlayerController>();
                if (playerController != null)
                {
                    playerController.isInTransition = false; // Permitir futuros combates
                }

                string previousScene = ScenePositionEnterCombatData.ultimaEscena;

                if (previousScene == "Mapa1_Dialogos")
                {
                    // Cargar la escena "Mapa1"
                    SceneManager.LoadScene("Mapa1");

                    // Esperar que la escena "Mapa1" se cargue y luego restaurar la posición
                    StartCoroutine(WaitForSceneToLoadAndRestorePosition("Mapa1"));
                }
                else if (previousScene == "Mapa2_Dialogos")
                {
                    // Cargar la escena "Mapa2_Solomon_Post_Servant"
                    SceneManager.LoadScene("Mapa2_Solomon_Post_Servant");

                    // Esperar que la escena "Mapa2" se cargue y luego restaurar la posición
                    StartCoroutine(WaitForSceneToLoadAndRestorePosition("Mapa2_Solomon_Post_Servant"));
                }
                else if (previousScene == "Mapa2_Solomon_Post_Servant"){
                    // Cargar la escena "Mapa2"
                    SceneManager.LoadScene("Mapa2");
                    StartCoroutine(WaitForSceneToLoadAndRestorePosition("Mapa2"));
                }
                else if (previousScene == "Mapa5"){
                    // Cargar la escena "Mapa2"
                    SceneManager.LoadScene("Mapa6");
                    StartCoroutine(WaitForSceneToLoadAndRestorePosition("Mapa6"));
                }
                else if (previousScene == "Mapa6"){
                    // Cargar la escena "Mapa2"
                    SceneManager.LoadScene("Mapa7");
                    StartCoroutine(WaitForSceneToLoadAndRestorePosition("Mapa7"));
                }
                else
                {
                    // Si no es "Mapa1_Dialogos","Mapa2_Dialogos",etc cargar la última escena guardada
                    SceneManager.LoadScene(previousScene);

                    // Restaurar la posición guardada
                    transform.position = ScenePositionEnterCombatData.ultimaPosicion;
                }
            }
            else
            {
                Debug.LogError("No hay una escena anterior guardada o la escena guardada es un combate.");
            }
        }
    }

    private IEnumerator WaitForSceneToLoadAndRestorePosition(string sceneName)
    {
        // Esperar hasta que la escena se haya cargado
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == sceneName);

        // Asegurarnos de que el personaje no se caiga
        // Aquí verificamos la altura y corregimos si es necesario (puedes ajustar esto si lo necesitas)
        if (transform.position.y < -5f) // Ajusta este valor según tu mapa
        {
            transform.position = new Vector3(transform.position.x, -3f, transform.position.z); // Coloca al personaje sobre el mapa
        }

        // Restaurar la posición después de cargar la escena
        transform.position = ScenePositionEnterCombatData.ultimaPosicion;
        Debug.Log("Posición restaurada en " + sceneName + ": " + transform.position);
    }
}
