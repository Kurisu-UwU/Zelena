using UnityEngine;

public class DuctoEntrada : MonoBehaviour
{
    [Header("Conexión del Par")]
    [Tooltip("Arrastra aquí el OTRO ducto con el que está conectado este par")]
    public Transform ductoDestino;

    [Header("Condición de Acceso")]
    [Tooltip("El límite máximo de escala X para considerar al personaje 'pequeño'")]
    public float limiteEscalaMax = 0.6f;

    // El bool que pensaste: controla si este ducto está bloqueado temporalmente
    [HideInInspector] // Esto hace que no sature el inspector, pero funciona por detrás
    public bool transportando = false;

    // 1. CUANDO EL JUGADOR ENTRA A LA HITBOX
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Personaje")
        {
            // Verificamos si NO está bloqueado y si cumple la condición del tamaño pequeño
            if (!transportando && collision.transform.localScale.x <= limiteEscalaMax)
            {
                if (ductoDestino != null)
                {
                    // Buscamos el script del ducto destino para avisarle que se bloquee
                    DuctoEntrada scriptDestino = ductoDestino.GetComponent<DuctoEntrada>();
                    if (scriptDestino != null)
                    {
                        scriptDestino.transportando = true; // Bloqueamos el ducto de salida antes de viajar
                    }

                    // Teletransportamos al personaje
                    collision.transform.position = ductoDestino.position;
                    Debug.Log($"Personaje pasó con éxito al ducto: {ductoDestino.name}");
                }
            }
        }
    }

    // 2. TU IDEA: CUANDO EL JUGADOR ABANDONA LA HITBOX, VOLVEMOS EL BOOL A FALSO
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Personaje")
        {
            // El jugador ya salió físicamente del ducto, queda libre para usarse otra vez
            transportando = false;
            Debug.Log($"El ducto {gameObject.name} se ha desbloqueado.");
        }
    }
}
