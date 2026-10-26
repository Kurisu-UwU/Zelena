using UnityEngine;

public class MuroInterior : MonoBehaviour
{
    private bool roto;

    public void Romper(PlayerController2D jugador)
    {
        if (roto || jugador == null || !jugador.estaQuemandose)
        {
            return;
        }

        roto = true;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        jugador.PlaySfx(MisionEscenario2.Instancia != null
            ? MisionEscenario2.Instancia.SonidoMuro
            : null);
        Destroy(gameObject);
    }
}
