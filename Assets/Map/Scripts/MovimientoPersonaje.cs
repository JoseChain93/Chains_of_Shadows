using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoPersonaje : MonoBehaviour
{
    private bool puedeMover = true;  // Controla si el personaje puede moverse

    public float moveSpeed = 2f;  // Velocidad de movimiento

    void Update()
    {
        if (puedeMover)
        {
            // Movimiento del personaje
            float horizontalInput = 0f;

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                horizontalInput = -1f; // Movimiento hacia la izquierda
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                horizontalInput = 1f; // Movimiento hacia la derecha
            }

            // Mover al personaje
            transform.Translate(Vector3.right * horizontalInput * moveSpeed * Time.deltaTime);
        }
    }

    // Método para bloquear el movimiento
    public void BloquearMovimiento()
    {
    puedeMover = false;  // Desactiva el movimiento
    }

    // Método para reanudar el movimiento
    public void ReanudarMovimiento()
    {
    puedeMover = true;  // Activa el movimiento
    }
}