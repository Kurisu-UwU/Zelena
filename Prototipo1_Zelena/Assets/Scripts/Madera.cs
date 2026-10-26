using UnityEngine;

public class Madera : MonoBehaviour
{
    [SerializeField] private AudioClip sonidoRomper = null;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController2D jugador = collision.gameObject.GetComponentInParent<PlayerController2D>();

        if (jugador != null && jugador.estaQuemandose)
        {
            jugador.PlaySfx(sonidoRomper);
            Destroy(gameObject, 0.1f);
        }
    }
}
