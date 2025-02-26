using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogo_Manager_Intro_Solomon : MonoBehaviour
{
    [Header("UI y Personaje")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public PlayerController playerController;       // Referencia al PlayerController
    public MovimientoPersonaje movimientoPersonaje; // Referencia al MovimientoPersonaje

    [Header("Líneas de diálogo")]
    public string[] lineas;

    private int indice = 0;
    private bool escribiendo = false;

    // Guardar velocidades originales para restaurarlas
    private float playerOriginalSpeed;
    private float movimientoOriginalSpeed;

    void Awake()
    {
        // Buscar automáticamente los componentes si no están asignados desde el inspector
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

        // Asegurarse de que no se destruyan entre escenas
        DontDestroyOnLoad(playerController);
        DontDestroyOnLoad(movimientoPersonaje);
    }

    void Start()
    {
        panelDialogo.SetActive(false);     
        ActivarDialogo();    
    }

    void ActivarDialogo()
    {
        panelDialogo.SetActive(true);        

        // Guardar velocidades originales
        playerOriginalSpeed = playerController.moveSpeed;
        movimientoOriginalSpeed = movimientoPersonaje.moveSpeed;

        // Detener movimiento
        playerController.DetenerMovimiento();
        movimientoPersonaje.moveSpeed = 0f;

        // Desactivar animación de caminar si está activa
        if (playerController.GetComponent<Animator>() != null)
        {
            playerController.GetComponent<Animator>().SetBool("Walking", false);
        }

        if (lineas.Length > 0)
            StartCoroutine(EscribirTexto(lineas[indice]));
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space)) && !escribiendo)
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
            yield return new WaitForSeconds(0.05f);  // Velocidad de escritura
        }

        escribiendo = false;
    }

    void SiguienteLinea()
    {
        indice++;

        if (indice < lineas.Length)
        {
            StartCoroutine(EscribirTexto(lineas[indice]));
        }
        else
        {
            FinalizarDialogo();
        }
    }

    void FinalizarDialogo()
    {
        panelDialogo.SetActive(false);         

        // Restaurar velocidades originales
        playerController.ReanudarMovimiento();
        playerController.moveSpeed = playerOriginalSpeed;
        movimientoPersonaje.moveSpeed = movimientoOriginalSpeed;

        // Asegurarse de que la animación de caminar se active si el personaje se mueve
        if (playerController.GetComponent<Animator>() != null)
        {
            playerController.GetComponent<Animator>().SetBool("Walking", playerController.moveSpeed > 0);
        }
    }
}
