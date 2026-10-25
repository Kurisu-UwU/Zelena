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
        // Evaluamos si es el personaje Y si aún NO ha sido cortado
        if (collision.name == "Personaje" && !yaCortado)
        {
            // Cambiamos el estado para bloquear futuras ejecuciones de este golpe
            yaCortado = true;

            collision.transform.localScale = new Vector3(0.5f, 0.5f, 1f);


            Debug.Log("¡El hacha cortó al personaje! Estado: Pequeño.");

            // OPCIONAL: Si quieres que el hacha desaparezca de la escena tras usarse, descomenta la línea de abajo:
            // Destroy(gameObject);
        }
    }
}
