using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public CharacterStats stats;
    
    private Animator animator;
    public float moveSpeed = 2f;  // Velocidad de movimiento
    public float combatChance = 0.05f;  // Probabilidad de entrar en combate (0 a 1)
    public string[] combatScenes;  // Lista de nombres de escenas de combate
    public AudioClip transitionMusic;  // Música de transición
    public float transitionTime = 1f;  // Tiempo de transición para la música

    private AudioSource audioSource;
    public bool isInTransition = false; // Controla si ya está en transición
    private bool canMove = true;  // Variable para controlar si el jugador puede moverse

    public Transform personaje;

    private void Start()
    {
   
        // Obtener el componente Animator
        animator = GetComponent<Animator>();

        // Crear un AudioSource para la música
        audioSource = gameObject.AddComponent<AudioSource>();

        if (FindObjectsOfType<PlayerController>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Mantener el objeto al cambiar de escena
        DontDestroyOnLoad(gameObject);

        // Suscribirse al evento de carga de escenas
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Buscar todos los objetos llamados "Solomon" en la escena
    PlayerController[] players = FindObjectsOfType<PlayerController>(true); // 'true' para incluir objetos desactivados

    foreach (PlayerController player in players)
    {
    // Si el objeto se llama "Solomon" y está desactivado, destrúyelo
    if (player != this && player.gameObject.name == "Solomon" && !player.gameObject.activeSelf)
    {
        Destroy(player.gameObject);  // Destruir solo los Solomon desactivados
        break;  // Solo destruir uno por ejecución
    }
    }

    }

    private void Update()
    {
        if (canMove)
    {
        float horizontalInput = 0f;

        // Detectar las teclas de movimiento (flechas o A/D)
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1f; // Movimiento hacia la izquierda
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f; // Movimiento hacia la derecha
        }

        // Obtener el Rigidbody2D y controlar el movimiento con su velocidad
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Establecer la velocidad en X y mantener la velocidad en Y
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }

        // Cambiar animación según si se está moviendo o no
        if (horizontalInput != 0)
        {
            animator.SetBool("Walking", true);
            Vector3 localScale = transform.localScale;
            localScale.x = horizontalInput > 0 ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
            transform.localScale = localScale;

            //Comprobar combate mientras se mueve
            if (!isInTransition && Random.value < combatChance)
            {
                StartCoroutine(TransitionToCombat());
            }
        }
        else
        {
            animator.SetBool("Walking", false);
        }
    }
    else
    {
        // Si no puede moverse, asegurarse de que la velocidad del Rigidbody sea 0
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Detiene el movimiento
        }
    }

   // Guardado solo si la escena no está restringida
    if (Input.GetKeyDown(KeyCode.G))
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Condiciones para bloquear el guardado
        bool isCombatScene = currentSceneName.Contains("Combat");
        bool isDialogScene = currentSceneName.Contains("Dialogo");
        bool isRestrictedMap = currentSceneName == "Mapa5" || currentSceneName == "Mapa6" || currentSceneName == "Mapa7";

        if (!isCombatScene && !isDialogScene && !isRestrictedMap)
        {
            PlayerStats.Instance.SaveGame();
        }
        else
        {
            Debug.LogWarning("¡No puedes guardar la partida aquí!");
        }
    }

    // Cargar juego (sin restricciones)
    if (Input.GetKeyDown(KeyCode.C))
    {
        PlayerStats.Instance.LoadGame();
    }

    }

   public void DetenerMovimiento()
    {
    // Detener cualquier movimiento aplicado por el Rigidbody2D
    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        rb.velocity = Vector2.zero; // Detiene el movimiento
    }

    // Desactivar el script para que no se ejecute el movimiento
    enabled = false;

    // Desactivar el movimiento con la variable canMove
    canMove = false;  // El jugador no podrá moverse
    }

    public void ReanudarMovimiento()
    {
    // Reactivar el script para que el movimiento funcione de nuevo
    enabled = true;

    // Habilitar el movimiento de nuevo
    canMove = true;  // El jugador podrá moverse
    }
    private void OnDestroy()
    {
        //Desuscribirse del evento al destruir el objeto
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Corutina que maneja la transición con distorsión y música
    private IEnumerator TransitionToCombat()
    {
        isInTransition = true;

        // Obtener el nombre de la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;

        // **Solo guardar la escena y posición si NO es una escena de combate**
        if (!currentSceneName.Contains("SceneCombat"))
        {
            ScenePositionEnterCombatData.ultimaEscena = currentSceneName;
            ScenePositionEnterCombatData.ultimaPosicion = transform.position;
            Debug.Log("Guardando escena y posición: " + currentSceneName + " - " + transform.position);
        }

        // Reproducir música de transición si existe
        if (transitionMusic != null)
        {
            audioSource.clip = transitionMusic;
            audioSource.Play();
        }

        // Esperar el tiempo de transición antes de cambiar de escena
        yield return new WaitForSeconds(transitionTime);

        // Seleccionar una escena aleatoria de combate
        if (combatScenes.Length > 0)
        {
            int randomIndex = Random.Range(0, combatScenes.Length);
            string selectedScene = combatScenes[randomIndex];

            // Cargar la escena de combate
            SceneManager.LoadScene(selectedScene);
        }
        else
        {
            Debug.LogWarning("No hay escenas de combate configuradas.");
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Restaurar la posición si volvemos a la escena anterior al combate
        if (scene.name == ScenePositionEnterCombatData.ultimaEscena)
        {
            transform.position = ScenePositionEnterCombatData.ultimaPosicion;
            Debug.Log("Posición restaurada: " + transform.position);
        }

        // **Restablecer la bandera de transición**
        isInTransition = false;
    }
}