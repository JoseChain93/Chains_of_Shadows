using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialogo_Manager_Solomon_Post_Servant : MonoBehaviour
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

    private int indice = 0;
    private bool escribiendo = false;

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
        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space)) && !escribiendo)
            SiguienteLinea();
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
            playerController.moveSpeed = playerOriginalSpeed;  
            movimientoPersonaje.moveSpeed = movimientoOriginalSpeed;  
            playerController.ReanudarMovimiento();  
            movimientoPersonaje.moveSpeed = 2f;

            if (playerController.TryGetComponent(out Animator animator))
                animator.SetBool("Walking", playerController.moveSpeed > 0);
        }

        if (movimientoPersonaje != null)
            ScenePositionEnterCombatData.ultimaPosicion = movimientoPersonaje.transform.position;

        ScenePositionEnterCombatData.ultimaEscena = SceneManager.GetActiveScene().name;

        yield break;
    }
}
