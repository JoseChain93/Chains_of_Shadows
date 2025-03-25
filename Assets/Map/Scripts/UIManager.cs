using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // Lista de botones para mantener sus listeners.
    public Button[] botones;

    /*  private void Awake()
     {
         if (instance == null)
         {
             instance = this;
             DontDestroyOnLoad(gameObject); // Asegura que este objeto persista entre escenas.
         }
         else
         {
             Destroy(gameObject); // Si ya existe, destrúyelo para evitar duplicados.
         }
     }
     */


    // Método para registrar los listeners de los botones.
    public void RegistrarBotones()
    {
        foreach (var boton in botones)
        {
            // Asegúrate de agregar los listeners de OnClick en cada botón.
            // Reemplaza los métodos de OnClick por los que necesites.
            boton.onClick.AddListener(() => MiMetodoDeBoton(boton));
        }
    }

    // Ejemplo de un método de OnClick de un botón.
    void MiMetodoDeBoton(Button boton)
    {
        Debug.Log($"Botón {boton.name} presionado.");
    }

    // Este método se puede llamar cuando la escena se ha cargado.
    /*public void RecargarConfiguracionBotones()
    {
        // Aquí podrías hacer un ciclo para reconfigurar los listeners.
        // Llama a `RegistrarBotones` cada vez que cambies de escena.
        RegistrarBotones();
    }
    */
}
 