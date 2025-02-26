using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Necesario para detectar eventos del mouse

public class ButtonColorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Color originalColor;

    void Start()
    {
        button = GetComponent<Button>();
        originalColor = button.image.color; // Guarda el color original
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        button.image.color = Color.white; // Cambia a blanco cuando el mouse entra
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        button.image.color = originalColor; // Restaura el color original al salir
    }
}
