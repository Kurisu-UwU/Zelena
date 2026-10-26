using UnityEngine;

public class Hacha : MonoBehaviour
{
    [Header("Configuracion del Hacha")]
    [Tooltip("El nuevo tamano (escala) que tendra el personaje al ser cortado")]
    public Vector3 escalaPequena = new Vector3(1f, 0.5f, 1f);
    [SerializeField] private AudioClip sonidoCorte = null;

    private bool yaCortado = false;

    public void ConfigurarSonido(AudioClip sonido)
    {
        sonidoCorte = sonido;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController2D jugador = collision.GetComponentInParent<PlayerController2D>();
        if (jugador != null && !yaCortado)
        {
            yaCortado = true;

            Collider2D miCollider = GetComponent<Collider2D>();
            if (miCollider != null)
            {
                miCollider.enabled = false;
            }

            jugador.Encogerse(escalaPequena);
            jugador.PlaySfx(sonidoCorte);
            Debug.Log("El hacha se desactivo despues del primer corte.");
        }
    }
}
