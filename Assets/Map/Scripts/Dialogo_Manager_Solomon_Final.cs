using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialogo_Manager_Solomon_Final : MonoBehaviour
{
    [Header("UI y Personaje")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public PlayerController playerController;
    public MovimientoPersonaje movimientoPersonaje;

    [Header("Líneas de diálogo")]
    public string[] lineas;

    [Header("Transición y Música")]
    public AudioClip musicaTransicion;
    public AudioSource audioSource;
    public string nombreEscena;
    public float transitionTime = 1f;

    [Header("Game Over")]
    public GameObject gameOverPanel; // Referencia al panel GameOver
    public string nombreEscenaMenuPrincipal = "MainMenu"; // Nombre de la escena de menú principal
    public CharacterStats stats;

    [Header("Solomon")]
    public GameObject solomon; // Referencia al objeto de Solomon que deseas destruir

    private int indice = 0;
    private bool escribiendo = false;
    private bool gameOverActivo = false; // Bandera para saber si el GameOverPanel está activo

    private float playerOriginalSpeed;
    private float movimientoOriginalSpeed;

    void Awake()
    {
        BuscarYAsignarComponentes();

        if (playerController != null) DontDestroyOnLoad(playerController.gameObject);
        if (movimientoPersonaje != null) DontDestroyOnLoad(movimientoPersonaje.gameObject);
    }

    void Start()
    {
        panelDialogo.SetActive(false);

        if (gameOverPanel != null) gameOverPanel.SetActive(false); // Asegúrate de que GameOver esté desactivado

        StartCoroutine(EsperarYBuscarComponentes());
    }

    void BuscarYAsignarComponentes()
    {
        if (playerController == null || playerController.Equals(null))
            playerController = FindObjectOfType<PlayerController>();

        if (movimientoPersonaje == null || movimientoPersonaje.Equals(null))
            movimientoPersonaje = FindObjectOfType<MovimientoPersonaje>();
    }

    IEnumerator EsperarYBuscarComponentes()
    {
        yield return new WaitForEndOfFrame();
        BuscarYAsignarComponentes();

        if (playerController != null && movimientoPersonaje != null)
            GuardarYDetenerMovimiento();

        ActivarDialogo();
    }

    void GuardarYDetenerMovimiento()
    {
        if (playerController == null || movimientoPersonaje == null)
        {
            Debug.LogWarning("No se pueden detener los movimientos porque los componentes no están asignados.");
            return;
        }

        playerOriginalSpeed = playerController.moveSpeed;
        movimientoOriginalSpeed = movimientoPersonaje.moveSpeed;

        playerController.DetenerMovimiento();
        movimientoPersonaje.moveSpeed = 0f;

        if (playerController.TryGetComponent(out Animator animator))
            animator.SetBool("Walking", false);
    }

    void ActivarDialogo()
    {
        panelDialogo.SetActive(true);

        GuardarYDetenerMovimiento();

        if (lineas.Length > 0)
            StartCoroutine(EscribirTexto(lineas[indice]));
        else
            Debug.LogWarning("No hay líneas de diálogo.");
    }

    void Update()
    {
        // Si el panel GameOver está activo, esperar a la pulsación de Space o Z
        if (gameOverActivo && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z)))
        {
            CargarMenuPrincipal();
        }
        else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z)) && !escribiendo)
        {
            SiguienteLinea();
        }
    }

    IEnumerator EscribirTexto(string linea)
    {
        escribiendo = true;
        textoDialogo.text = "";

        foreach (char letra in linea)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(0.05f);
        }

        escribiendo = false;
    }

    void SiguienteLinea()
    {
        indice++;
        if (indice < lineas.Length)
            StartCoroutine(EscribirTexto(lineas[indice]));
        else
            StartCoroutine(FinalizarDialogo());
    }

    IEnumerator FinalizarDialogo()
    {
        panelDialogo.SetActive(false);

        if (playerController != null && movimientoPersonaje != null)
        {
            playerController.ReanudarMovimiento();
            playerController.moveSpeed = playerOriginalSpeed;
            movimientoPersonaje.moveSpeed = movimientoOriginalSpeed;

            if (playerController.TryGetComponent(out Animator animator))
                animator.SetBool("Walking", playerController.moveSpeed > 0);
        }

        if (movimientoPersonaje != null)
            ScenePositionEnterCombatData.ultimaPosicion = movimientoPersonaje.transform.position;

        ScenePositionEnterCombatData.ultimaEscena = SceneManager.GetActiveScene().name;

        if (musicaTransicion != null)
        {
            audioSource.clip = musicaTransicion;
            audioSource.Play();
        }

        yield return new WaitForSeconds(transitionTime);

        // Activar panel GameOver al finalizar el diálogo
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // Mostrar el panel GameOver
            gameOverActivo = true; // Establecer la bandera a true
        }
        else
        {
            Debug.LogWarning("Panel GameOver no asignado.");
        }

    }

    void CargarMenuPrincipal()
    {
        
        // Destruir a Solomon si está presente
        if (solomon != null)
        {
            Destroy(solomon); // Destruir el objeto de Solomon
        }

        // Reiniciar las estadísticas del jugador antes de cargar el menú
        if (stats != null)
        {
            stats.ResetStats();
        }
        
        // Verifica si se ha asignado el nombre de la escena y la carga
        if (!string.IsNullOrEmpty(nombreEscenaMenuPrincipal))
        {
            SceneManager.LoadScene(nombreEscenaMenuPrincipal);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el nombre de la escena del menú principal.");
        }
    }
}
