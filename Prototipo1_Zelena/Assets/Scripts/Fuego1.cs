using UnityEngine;

public class Fuego1 : MonoBehaviour
{
    [Header("Configuracion del Fuego")]
    public int CantDano = 10;
    private float nuevaVelocidadFuego = 24f;

    [Header("Cambio de aspecto")]
    public Sprite fuegoModificado;
    [SerializeField] private AudioClip sonidoFuego = null;
    [SerializeField] private Color colorAlTocar = new Color(1f, 0.55f, 0.2f);

    public void ConfigurarEfecto(Sprite sprite, AudioClip sonido)
    {
        fuegoModificado = sprite;
        sonidoFuego = sonido;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController2D jugador = other.GetComponentInParent<PlayerController2D>();

        if (jugador != null)
        {
            jugador.ActivarEstadoFuego(nuevaVelocidadFuego, 5f);
            jugador.RecibeDano(CantDano);

            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.color = colorAlTocar;
                if (fuegoModificado != null)
                {
                    sprite.sprite = fuegoModificado;
                }
            }

            jugador.PlaySfx(sonidoFuego);
        }
    }
}
