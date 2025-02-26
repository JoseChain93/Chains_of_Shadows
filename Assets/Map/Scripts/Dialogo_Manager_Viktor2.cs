using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialogo_Manager_Viktor2 : MonoBehaviour
{
    [Header("UI y Personaje")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public PlayerController playerController;
    public MovimientoPersonaje movimientoPersonaje;

    [Header("Líneas de diálogo")]
    public string[] lineas;

    [Header("Objetos a manipular")]
    public GameObject madre;      // Objeto "Madre"
    public GameObject viktor;     // Objeto "Viktor"
    public GameObject madreBoss;  // Objeto "MadreBoss"
    public GameObject solomon;    // Objeto "Solomon"

    [Header("Transición y Música")]
    public AudioClip musicaTransicion;
    public AudioSource audioSource;
    public string nombreEscena;
    public float transitionTime = 1f;

    [Header("Audio Grito Boss")]
    public AudioClip gritoMadreBoss;  
    
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
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        if (movimientoPersonaje == null)
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
        {
            StartCoroutine(EscribirTexto(lineas[indice]));

            // En la 4ª línea (índice 3): destruye "Madre" y activa "MotherBoss"
            if (indice == 3)
            {
                if (madre != null)
                {
                    Destroy(madre);
                    
                }

                if (madreBoss != null)
                {
                    madreBoss.SetActive(true);
                    
                }
            }
        }
        else
        {
            StartCoroutine(FinalizarDialogo());
        }
    }

    IEnumerator FinalizarDialogo()
    {
        panelDialogo.SetActive(false);

         // Destruye Viktor
        if (viktor != null)
        {
            Destroy(viktor);
           
        }
        madreBoss.transform.rotation = Quaternion.Euler(madreBoss.transform.eulerAngles.x, madreBoss.transform.eulerAngles.y + 180f, madreBoss.transform.eulerAngles.z);

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

       
        if (gritoMadreBoss != null && audioSource != null)
        {
            audioSource.PlayOneShot(gritoMadreBoss);
        }
                    
        GameObject solomon = GameObject.Find("Solomon");
        if (solomon != null) {
        solomon.transform.SetParent(null);  
        Destroy(solomon);  
        }

        // Reproduce música de transición
        if (musicaTransicion != null)
        {
            audioSource.clip = musicaTransicion;
            audioSource.Play();
        }

        yield return new WaitForSeconds(transitionTime);

        if (!string.IsNullOrEmpty(nombreEscena))
            SceneManager.LoadScene(nombreEscena);
        else
            Debug.LogWarning("Nombre de escena no asignado.");
    }
}
