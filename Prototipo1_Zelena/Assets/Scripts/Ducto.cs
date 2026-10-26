using System.Collections; // Obligatorio para usar Corrutinas (WaitForSeconds)
using UnityEngine;

public class DuctoEntrada : MonoBehaviour
{
    [Header("Conexi�n del Par")]
    [Tooltip("Arrastra aqu� el OTRO ducto con el que est� conectado este par")]
    public Transform ductoDestino;

    [Header("Condici�n de Acceso")]
    [Tooltip("El l�mite m�ximo de escala X para considerar al personaje 'peque�o'")]
    public float limiteEscalaMax = 0.6f;

    [Header("Animaci�n y Tiempos")]
    [Tooltip("Tiempo de espera en segundos antes de aparecer en el otro lado")]
    public float tiempoEspera = 1.0f;
    [SerializeField] private AudioClip sonidoTeletransporte = null;

    [HideInInspector]
    public bool transportando = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Como le quitamos el "2D" al nombre de tu script, ahora buscamos "PlayerController"
        PlayerController2D jugador = collision.GetComponent<PlayerController2D>();

        if (jugador != null)
        {
            // Verificamos si NO est� bloqueado y si cumple la condici�n del tama�o peque�o
            if (!transportando && collision.transform.localScale.x <= limiteEscalaMax)
            {
                // Iniciamos el proceso de viaje con retraso
                StartCoroutine(ProcesoTeletransportacion(collision.gameObject, jugador));
            }
        }
    }

    // Tu idea mejorada: El proceso espera aqu� antes de mover al humano
    private IEnumerator ProcesoTeletransportacion(GameObject personajeGO, PlayerController2D jugadorScript)
    {
        // 1. Bloqueamos este ducto y el de destino para evitar bucles infinitos
        transportando = true;
        if (ductoDestino != null)
        {
            DuctoEntrada scriptDestino = ductoDestino.GetComponent<DuctoEntrada>();
            if (scriptDestino != null)
            {
                scriptDestino.transportando = true;
            }
        }

        // 2. DETENER AL JUGADOR: Apagamos su velocidad para que se quede quieto durante la animaci�n
        float velocidadOriginal = jugadorScript.moveSpeed;
        jugadorScript.moveSpeed = 0f;
        jugadorScript.PlaySfx(sonidoTeletransporte);

        // Opcional: Aqu� es donde tus compa�eros pueden activar el disparador (Trigger) de la animaci�n
        // Animator anim = personajeGO.GetComponent<Animator>();
        // if (anim != null) anim.SetTrigger("EntrarDucto");

        Debug.Log($"Iniciando animaci�n en {gameObject.name}. Esperando {tiempoEspera} segundo...");

        // 3. LA PAUSA: El c�digo se duerme aqu� por 1 segundo completo en tiempo real
        yield return new WaitForSeconds(tiempoEspera);

        // 4. EL VIAJE: Pasado el segundo, movemos f�sicamente al personaje al destino
        if (ductoDestino != null)
        {
            personajeGO.transform.position = ductoDestino.position;
            jugadorScript.PlaySfx(sonidoTeletransporte);
            Debug.Log($"Personaje teletransportado con �xito al ducto: {ductoDestino.name}");
        }

        // 5. DEVOLVER EL MOVIMIENTO: Le regresamos su velocidad original para que pueda caminar al salir
        jugadorScript.moveSpeed = velocidadOriginal;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Buscamos si lo que sali� es el jugador para liberar el seguro del ducto
        if (collision.GetComponent<PlayerController2D>() != null)
        {
            transportando = false;
            Debug.Log($"El ducto {gameObject.name} se ha desbloqueado.");
        }
    }
}
