using UnityEngine;

public class Hacha : MonoBehaviour
{
    [Header("Configuración del Hacha")]
    [Tooltip("El nuevo tamaño (escala) que tendrá el personaje al ser cortado")]
    public Vector3 escalaPequena = new Vector3(1f, 0.5f, 1f);

    // Bandera booleana para asegurar que el corte ocurra una sola vez
    private bool yaCortado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Personaje" && !yaCortado)
        {
            yaCortado = true; // Bloqueo lógico inmediato

            // SOLUCIÓN DEFINITIVA: Desactivamos el collider del hacha para siempre
            Collider2D miCollider = GetComponent<Collider2D>();
            if (miCollider != null)
            {
                miCollider.enabled = false;
                // Al hacer esto falso, la zona verde deja de existir físicamente en el juego.
                // Unity ignora cualquier otra entrada o movimiento posterior.
            }

            // Aplicamos la escala fija que ya vimos que funciona perfecto
            collision.transform.localScale = escalaPequena;

            Debug.Log("¡Hacha desactivada físicamente tras el primer corte!");
        }
    }

}
