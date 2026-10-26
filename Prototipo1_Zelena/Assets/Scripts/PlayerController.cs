using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController2D : MonoBehaviour
{
    [Header("Stats")]
    public int MaxVidaZelena = 100;
    private int CantVidaZelena;

    [Header("Movimiento Horizontal")]
    public float moveSpeed = 8f;
    private float velocidadOriginal;
    private float horizontalInput;
    private int facingDirection = 1;

    [Header("Salto")]
    public float jumpForce = 12f;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;
    private Rigidbody2D rb;

    [Header("Dash")]
    public float dashSpeed = 22f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.8f;
    private bool canDash = true;
    private bool isDashing;
    private bool isDead;
    private Vector3 escalaOriginal;
    private bool estaEncogida;
    public int VidaActual => CantVidaZelena;

    [Header("Infeccion")]
    public string humanTag = "Human";
    [Header("Quemar")]
    public bool estaQuemandose;
    public string fuegoTag = "Fuego";
    [Header("Portal")]
    public string portalTag = "Portal";

    [Header("Audio")]
    [SerializeField] private AudioClip sonidoPasos = null;
    [SerializeField] private AudioClip sonidoSalto = null;
    [SerializeField] private AudioClip sonidoDash = null;
    [SerializeField] private AudioClip sonidoDano = null;
    [SerializeField] private AudioClip sonidoInfeccion = null;
    private AudioSource audioSource;
    private float tiempoPaso;
    private const float intervaloPaso = 0.34f;

    [Header("Efectos visuales")]
    [SerializeField] private Color colorFuego = new Color(1f, 0.45f, 0.12f);
    [SerializeField] private Color colorDano = new Color(1f, 0.2f, 0.2f);
    private SpriteRenderer spriteRenderer;
    private Color colorOriginal = Color.white;
    private Coroutine rutinaFuego;
    private Coroutine rutinaDestello;
    private Sprite spriteQuieto;
    private Sprite spriteSalto;
    private Sprite[] spritesMovimiento = new Sprite[0];
    private float tiempoAnimacion;
    private int frameMovimiento;
    private bool mostrarHudPropio = true;
    private GUIStyle estiloHud;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        escalaOriginal = transform.localScale;
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        if (spriteRenderer != null)
        {
            colorOriginal = spriteRenderer.color;
        }
    }

    private void Start()
    {
        CantVidaZelena = Mathf.Max(1, MaxVidaZelena);
        velocidadOriginal = moveSpeed;
        if (spriteRenderer != null && spriteQuieto != null)
        {
            spriteRenderer.sprite = spriteQuieto;
        }
    }

    private void Update()
    {
        if (isDead || isDashing)
        {
            return;
        }

        horizontalInput = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput = -1f;
            facingDirection = -1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput = 1f;
            facingDirection = 1;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facingDirection < 0;
        }

        isGrounded = groundCheck != null &&
            Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) ||
             Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            Jump();
        }

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && canDash)
        {
            StartCoroutine(Dash());
        }

        UpdateFootsteps();
        ActualizarAnimacion();
    }

    private void OnGUI()
    {
        if (!mostrarHudPropio)
        {
            return;
        }

        if (estiloHud == null)
        {
            estiloHud = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
        }

        float vida = Mathf.Clamp01((float)CantVidaZelena / Mathf.Max(1, MaxVidaZelena));
        GUI.color = new Color(0.04f, 0.07f, 0.08f, 0.88f);
        GUI.DrawTexture(new Rect(16f, 16f, 236f, 72f), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(28f, 22f, 210f, 24f),
            $"VIDA  {CantVidaZelena}/{Mathf.Max(1, MaxVidaZelena)}", estiloHud);

        GUI.color = new Color(0.16f, 0.2f, 0.2f, 1f);
        GUI.DrawTexture(new Rect(28f, 54f, 210f, 17f), Texture2D.whiteTexture);
        GUI.color = vida > 0.5f
            ? new Color(0.25f, 0.82f, 0.38f, 1f)
            : new Color(1f, 0.58f, 0.16f, 1f);
        GUI.DrawTexture(new Rect(28f, 54f, 210f * vida, 17f), Texture2D.whiteTexture);
        GUI.color = new Color(0.04f, 0.07f, 0.08f, 0.75f);
        GUI.DrawTexture(new Rect(Screen.width - 238f, 16f, 222f, 52f), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(Screen.width - 226f, 22f, 210f, 40f),
            "A/D o flechas: mover   ESPACIO: saltar\nSHIFT: dash", estiloHud);
    }

    private void ActualizarAnimacion()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (estaQuemandose)
        {
            return;
        }

        if (rb != null && Mathf.Abs(rb.linearVelocity.y) > 0.15f && spriteSalto != null)
        {
            spriteRenderer.sprite = spriteSalto;
            return;
        }

        if (Mathf.Abs(horizontalInput) > 0.1f && spritesMovimiento.Length > 0)
        {
            tiempoAnimacion += Time.deltaTime;
            if (tiempoAnimacion >= 0.12f)
            {
                tiempoAnimacion = 0f;
                frameMovimiento = (frameMovimiento + 1) % spritesMovimiento.Length;
            }
            spriteRenderer.sprite = spritesMovimiento[frameMovimiento];
        }
        else if (spriteQuieto != null)
        {
            tiempoAnimacion = 0f;
            frameMovimiento = 0;
            spriteRenderer.sprite = spriteQuieto;
        }
    }

    private void FixedUpdate()
    {
        if (isDead || isDashing || rb == null)
        {
            return;
        }

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void UpdateFootsteps()
    {
        if (isGrounded && Mathf.Abs(horizontalInput) > 0.1f)
        {
            tiempoPaso -= Time.deltaTime;
            if (tiempoPaso <= 0f)
            {
                PlaySfx(sonidoPasos);
                tiempoPaso = intervaloPaso;
            }
        }
        else
        {
            tiempoPaso = 0f;
        }
    }

    private void Jump()
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        PlaySfx(sonidoSalto);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        if (rb != null)
        {
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            float dashDir = horizontalInput != 0f ? horizontalInput : facingDirection;
            rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0f);
            PlaySfx(sonidoDash);

            yield return new WaitForSeconds(dashDuration);

            rb.gravityScale = originalGravity;
        }
        else
        {
            yield return new WaitForSeconds(dashDuration);
        }

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MuroInterior muro = collision.collider.GetComponent<MuroInterior>();
        if (estaQuemandose && muro != null)
        {
            muro.Romper(this);
        }

        CheckAndInfect(collision.gameObject);
        CheckFuego(collision.gameObject);
        CheckPortal(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckAndInfect(collision.gameObject);
        CheckPortal(collision.gameObject);
    }

    private void CheckAndInfect(GameObject target)
    {
        if (target.CompareTag(humanTag))
        {
            PlaySfx(sonidoInfeccion);
            if (MisionEscenario2.Instancia != null)
            {
                MisionEscenario2.Instancia.PersonaRescatada();
            }
            Destroy(target);
        }
    }

    private void CheckFuego(GameObject target)
    {
        if (target.CompareTag(fuegoTag))
        {
            RecibeDano(10);
        }
    }

    private void CheckPortal(GameObject target)
    {
        if (target.CompareTag(portalTag))
        {
            SceneManager.LoadScene("Escenario3");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public void PlaySfx(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void ConfigurarSprites(Sprite quieto, Sprite[] movimiento, Sprite salto)
    {
        spriteQuieto = quieto;
        spritesMovimiento = movimiento ?? new Sprite[0];
        spriteSalto = salto;
        if (spriteRenderer != null && spriteQuieto != null)
        {
            spriteRenderer.sprite = spriteQuieto;
        }
    }

    public void ConfigurarVelocidadBase(float velocidad)
    {
        velocidadOriginal = velocidad;
        moveSpeed = velocidad;
    }

    public void UsarHudEscenario(bool activar)
    {
        mostrarHudPropio = activar;
    }

    public void Encogerse(Vector3 nuevaEscala)
    {
        if (estaEncogida)
        {
            return;
        }

        Collider2D collider = GetComponent<Collider2D>();
        float alturaAntes = collider != null ? collider.bounds.size.y : 0f;
        escalaOriginal = transform.localScale;
        transform.localScale = nuevaEscala;
        float alturaDespues = collider != null ? collider.bounds.size.y : 0f;
        transform.position += Vector3.down * ((alturaAntes - alturaDespues) * 0.5f);
        estaEncogida = true;
    }

    public void RestaurarTamano(Vector3 escala = default)
    {
        if (!estaEncogida)
        {
            return;
        }

        Collider2D collider = GetComponent<Collider2D>();
        float alturaAntes = collider != null ? collider.bounds.size.y : 0f;
        transform.localScale = escala == default ? escalaOriginal : escala;
        float alturaDespues = collider != null ? collider.bounds.size.y : 0f;
        transform.position += Vector3.down * ((alturaAntes - alturaDespues) * 0.5f);
        estaEncogida = false;
    }

    public void RecibeDano(int cantidad)
    {
        if (isDead || cantidad <= 0)
        {
            return;
        }

        CantVidaZelena = Mathf.Max(0, CantVidaZelena - cantidad);
        PlaySfx(sonidoDano);
        if (rutinaDestello != null)
        {
            StopCoroutine(rutinaDestello);
        }
        rutinaDestello = StartCoroutine(DestelloDano());

        if (CantVidaZelena == 0)
        {
            Muerto();
        }
    }

    private IEnumerator DestelloDano()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = colorDano;
        }

        yield return new WaitForSeconds(0.15f);
        ActualizarColor();
        rutinaDestello = null;
    }

    public void ActivarEstadoFuego(float nuevaVelocidad, float tiempo)
    {
        estaQuemandose = true;
        moveSpeed = nuevaVelocidad;
        ActualizarColor();

        if (rutinaFuego != null)
        {
            StopCoroutine(rutinaFuego);
        }
        rutinaFuego = StartCoroutine(TemporizadorFuego(tiempo));
    }

    private IEnumerator TemporizadorFuego(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        estaQuemandose = false;
        moveSpeed = velocidadOriginal;
        ActualizarColor();
        rutinaFuego = null;
    }

    private void ActualizarColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = estaQuemandose ? colorFuego : colorOriginal;
        }
    }

    private void Muerto()
    {
        isDead = true;
        horizontalInput = 0f;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.color = colorDano;
        }
        Debug.Log("Zelena se ha quedado sin vida.");
        if (MisionEscenario2.Instancia != null)
        {
            MisionEscenario2.Instancia.JugadorDerrotado();
        }
    }

    public void Morir()
    {
        if (!isDead)
        {
            CantVidaZelena = 0;
            Muerto();
        }
    }
}
