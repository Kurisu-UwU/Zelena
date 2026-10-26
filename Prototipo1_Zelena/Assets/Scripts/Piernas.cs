using UnityEngine;

public class Piernas : MonoBehaviour
{
    [Header("Configuraci�n de Restauraci�n")]
    [Tooltip("La escala original a la que regresar� el personaje")]
    public Vector3 escalaOriginal = new Vector3(1f, 1f, 1f);
    [SerializeField] private AudioClip sonidoRestaurar = null;

    public void ConfigurarSonido(AudioClip sonido)
    {
        sonidoRestaurar = sonido;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detectamos si es nuestro personaje
        if (collision.name == "Personaje")
        {
            // Le devolvemos su tama�o y hitbox original de forma fija
            PlayerController2D jugador = collision.GetComponent<PlayerController2D>();
            if (jugador != null)
            {
                jugador.RestaurarTamano(escalaOriginal);
                jugador.PlaySfx(sonidoRestaurar);
            }

            Debug.Log("�Piernas recuperadas! El personaje volvi� a su tama�o original.");

            // Hacemos que el objeto 'Piernas' desaparezca por completo del juego inmediatamente
            Destroy(gameObject);
        }
    }
}
