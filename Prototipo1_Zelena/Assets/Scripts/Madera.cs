using UnityEngine;

public class Madera : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Buscamos el script en el objeto que chocó físicamente
        PlayerController2D jugador = collision.gameObject.GetComponent<PlayerController2D>();

        // Verificamos si existe el jugador Y si tiene la variable 'estaQuemandose' activa
        if (jugador != null && jugador.estaQuemandose)
        {
            Debug.Log("¡La madera se ha quemado por completo y desapareció de la escena!");

            // Borramos el objeto de madera del juego inmediatamente, despejando el camino
            Destroy(gameObject);
        }
        else if (jugador != null)
        {
            Debug.Log("Sólido: Necesitas estar prendido en fuego para destruir esta madera.");
        }
    }
}
