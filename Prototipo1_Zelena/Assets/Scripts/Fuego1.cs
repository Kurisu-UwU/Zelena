using UnityEngine;

public class Fuego1 : MonoBehaviour
{
    [Header("Configuración del Fuego")]
    public int CantDano = 10;
    // Al volverlo private, Unity borra la casilla del Inspector y obedece al 100% el valor del código
    private float nuevaVelocidadFuego = 24f;

    [Header("Cambio de Aspecto")]
    public Sprite fuegoModificado;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // MENSAJE DE CONTROL: Si esto no sale en consola, el objeto del mapa no tiene este script.
        Debug.Log($"¡Algo tocó el objeto con Fuego1! Nombre del objeto: {other.name}");

        PlayerController2D jugador = other.GetComponent<PlayerController2D>();

        if (jugador != null)
        {
            jugador.ActivarEstadoFuego(nuevaVelocidadFuego, 5f);
            jugador.RecibeDano(CantDano);
        }
    }

}
