using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialogo_Manager_Servant : MonoBehaviour
{
    [Header("UI y Personajes")]
    public GameObject solomonDialogo;
    public TextMeshProUGUI DialogoSolomon;

    public GameObject servantDialogo;
    public TextMeshProUGUI DialogoServant;

    [Header("Control de movimiento")]
    public PlayerController playerController;
    public MovimientoPersonaje movimientoPersonaje;

    [Header("Líneas de diálogo")]
    public string[] lineas; // Pares: Solomon | Impares: Servant

    [Header("Transición")]
    public AudioClip musicaTransicion;
    public float transitionTime = 0.5f;
    public string nombreEscena;  // Escena a cargar después del diálogo

    private AudioSource audioSource;
    private int indice = 0;
    private bool escribiendo = false;

    // Guardar velocidades originales
    private float playerOriginalSpeed;
    private float movimientoOriginalSpeed;

    // Guardar animador
    private Animator playerAnimator;

    void Start()
    {
        // Asegurarse de que el personaje y sus componentes están completamente cargados antes de buscar los componentes
        StartCoroutine(EsperarYBuscarComponentes());
        
        // Configurar el AudioSource si no está configurado
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            Debug.Log("AudioSource agregado automáticamente.");
        }

        solomonDialogo.SetActive(false);
        servantDialogo.SetActive(false);
    }

    // Coroutine para esperar a que los componentes estén completamente inicializados
    IEnumerator EsperarYBuscarComponentes()
    {
        // Espera a que el objeto con los componentes esté completamente cargado (esto también abarca la transición entre escenas)
        yield return new WaitForEndOfFrame();

        // Buscar y asignar automáticamente los componentes si no están asignados
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController == null)
                Debug.LogWarning("PlayerController no encontrado en la escena.");
        }

        if (movimientoPersonaje == null)
        {
            movimientoPersonaje = FindObjectOfType<MovimientoPersonaje>();
            if (movimientoPersonaje == null)
                Debug.LogWarning("MovimientoPersonaje no encontrado en la escena.");
        }

        // Si se asignaron correctamente los componentes, detener el movimiento
        if (playerController != null && movimientoPersonaje != null)
        {
            GuardarYDetenerMovimiento();
        }

        // Comienza el diálogo una vez que todo esté configurado
        ActivarDialogo();
    }

    void GuardarYDetenerMovimiento()
    {
        // Guardar las velocidades originales
        playerOriginalSpeed = playerController.moveSpeed;
        movimientoOriginalSpeed = movimientoPersonaje.moveSpeed;

        // Detener el movimiento
        playerController.DetenerMovimiento();
        movimientoPersonaje.moveSpeed = 0f;

        // Desactivar animación de caminar si está activa
        if (playerController.GetComponent<Animator>() != null)
        {
            playerController.GetComponent<Animator>().SetBool("Walking", false);
        }
    }

    void ActivarDialogo()
    {
        indice = 0;
        MostrarDialogo();
    }

    void Update()
    {
        // Avanzar en el diálogo con Z o Espacio si no se está escribiendo
        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space)) && !escribiendo)
        {
            SiguienteLinea();
        }
    }

    void MostrarDialogo()
    {
        solomonDialogo.SetActive(false);
        servantDialogo.SetActive(false);

        // Si se han mostrado todas las líneas, finalizar diálogo
        if (indice >= lineas.Length)
        {
            StartCoroutine(FinalizarDialogo());
            return;
        }

        // Determinar quién habla: pares Solomon, impares Servant
        bool esSolomon = indice % 2 == 0;

        if (esSolomon)
        {
            solomonDialogo.SetActive(true);
            StartCoroutine(EscribirTexto(DialogoSolomon, lineas[indice]));
        }
        else
        {
            servantDialogo.SetActive(true);
            StartCoroutine(EscribirTexto(DialogoServant, lineas[indice]));
        }
    }

    IEnumerator EscribirTexto(TextMeshProUGUI texto, string linea)
    {
        escribiendo = true;
        texto.text = "";

        foreach (char letra in linea)
        {
            texto.text += letra;
            yield return new WaitForSeconds(0.04f); // Velocidad de escritura
        }

        escribiendo = false;
    }

    void SiguienteLinea()
    {
        indice++;
        MostrarDialogo();
    }

    IEnumerator FinalizarDialogo()
    {
        solomonDialogo.SetActive(false);
        servantDialogo.SetActive(false);

        // Restaurar movimiento si los componentes existen
        if (playerController != null && movimientoPersonaje != null)
        {
            playerController.ReanudarMovimiento();
            playerController.moveSpeed = playerOriginalSpeed;
            movimientoPersonaje.moveSpeed = movimientoOriginalSpeed;

            // Restaurar animación de caminar si el jugador comienza a moverse
            if (playerController.GetComponent<Animator>() != null)
            {
                playerController.GetComponent<Animator>().SetBool("Walking", playerController.moveSpeed > 0);
            }
        }

        // Guardar la posición y escena antes de ir al combate
        if (movimientoPersonaje != null)
            ScenePositionEnterCombatData.ultimaPosicion = movimientoPersonaje.transform.position;

        ScenePositionEnterCombatData.ultimaEscena = SceneManager.GetActiveScene().name;

        // Reproducir música de transición si existe
        if (musicaTransicion != null)
        {
            audioSource.clip = musicaTransicion;
            audioSource.Play();
        }

        yield return new WaitForSeconds(transitionTime);

        // Cargar la escena de combate si el nombre está asignado
        if (!string.IsNullOrEmpty(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogWarning("Nombre de escena no asignado.");
        }
    }
}
