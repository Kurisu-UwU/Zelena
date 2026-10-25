using UnityEngine;

public class Piernas : MonoBehaviour
{
    [Header("Configuración de Restauración")]
    [Tooltip("La escala original a la que regresará el personaje")]
    public Vector3 escalaOriginal = new Vector3(1f, 1f, 1f);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detectamos si es nuestro personaje
        if (collision.name == "Personaje")
        {
            // Le devolvemos su tamaño y hitbox original de forma fija
            collision.transform.localScale = escalaOriginal;

            Debug.Log("¡Piernas recuperadas! El personaje volvió a su tamaño original.");

            // Hacemos que el objeto 'Piernas' desaparezca por completo del juego inmediatamente
            Destroy(gameObject);
        }
    }
}
