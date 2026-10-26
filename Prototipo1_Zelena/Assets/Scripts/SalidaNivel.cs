using UnityEngine;

public class SalidaNivel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController2D jugador = other.GetComponentInParent<PlayerController2D>();
        if (jugador != null && MisionEscenario2.Instancia != null)
        {
            MisionEscenario2.Instancia.IntentarSalir();
        }
    }
}
