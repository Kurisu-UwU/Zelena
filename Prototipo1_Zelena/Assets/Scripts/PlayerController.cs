using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ZombieMordida;
    [SerializeField] private AudioClip ZombieSalta;
    [SerializeField] private AudioClip ZombieSalta2;
    [Header("Stats")]
    public int MaxVidaZelena = 100;
    private int CantVidaZelena;

    [Header("Movimiento Horizontal")]
    public float moveSpeed = 8f;
    private float horizontalInput;
    private int facingDirection = 1; // 1 = Derecha, -1 = Izquierda

    [Header("Salto")]
    public float jumpForce = 12f;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;
    private Rigidbody2D rb;
    private Animator anim; // Referencia al Animator

    [Header("Dash")]
    public float dashSpeed = 22f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.8f;
    private bool canDash = true;
    private bool isDashing;

    [Header("Infección")]
    public string humanTag = "Human";
    [Header("Quemar")]
    public bool estaQuemandose = false;
    public string fuegoTag = "Fuego";
    [Header("Portal")]
    public string portalTag = "Portal";

    [Header("Portal")]
    public string portalTag = "Portal";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        CantVidaZelena = MaxVidaZelena;
    }

    void Update()
    {
        if (isDashing) return;

        // Lectura de entrada con A (izquierda) y D (derecha)
        horizontalInput = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1f;
            facingDirection = -1;
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f;
            facingDirection = 1;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        // Comprobación de si está en el suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        // Salto (Barra Espaciadora)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Dash (Left Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        // Actualizar parámetros del Animator
        bool isWalking = horizontalInput != 0;
        anim.SetBool("IsWalking", isWalking);
        anim.SetBool("IsJumping", !isGrounded);
    }

    void FixedUpdate()
    {
        if (isDashing) return;
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        int i = Random.Range(0, 2);
        if (i==1) {
        audioSource.PlayOneShot(ZombieSalta);
        } else {
            audioSource.PlayOneShot(ZombieSalta2);
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDir = horizontalInput != 0 ? horizontalInput : facingDirection;
        rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // Colisiones
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckAndInfect(collision.gameObject);
        CheckFuego(collision.gameObject);
        CheckPortal(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckAndInfect(collision.gameObject);
        CheckFuego(collision.gameObject);
        CheckPortal(collision.gameObject);
    }

    void CheckAndInfect(GameObject target)
    {
        if (target.CompareTag(humanTag))
        {
            Debug.Log($"¡Objeto humano infectado: {target.name}!");
            audioSource.PlayOneShot(ZombieMordida);
            Destroy(target);
        }
    }

    void CheckFuego(GameObject target)
    {
        if (target.CompareTag(fuegoTag))
        {
            Debug.Log($"¡Te quemas!: {target.name}!");
        }
    }
    void CheckPortal(GameObject target){
        if (target.CompareTag(portalTag)) {
            Debug.Log($"Portal: {target.name}!");
            SceneManager.LoadScene("Escenario3");
        }
    }

    void CheckPortal(GameObject target)
    {
        if (target.CompareTag(portalTag))
        {
            Debug.Log($"Portal: {target.name}!");
            SceneManager.LoadScene("Escenario3");
        }
    }

    // Sistema de Vida y Daño
    public void RecibeDano(int Cant)
    {
        CantVidaZelena -= Cant;
        Debug.Log("Vida actual:" + CantVidaZelena);
        if (CantVidaZelena <= 0)
        {
            Muerto();
        }
    }

    // Función para activar el fuego
    public void ActivarEstadoFuego(float nuevaVelocidad, float tiempo)
    {
        StopAllCoroutines();
        estaQuemandose = true;
        moveSpeed = nuevaVelocidad;

        StartCoroutine(TemporizadorFuego(tiempo));
        Debug.Log($"🔥 ¡Personaje en llamas! Velocidad cambiada a {moveSpeed}");
    }

    private IEnumerator TemporizadorFuego(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        estaQuemandose = false;
        moveSpeed = 8f;
        Debug.Log("❄️ El fuego se apagó y la velocidad volvió a 8f");
    }

    void Muerto()
    {
        Debug.Log("Rip");
    }

    // Gizmos
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
<<<<<<< HEAD
=======
    public void RecibeDano(int Cant){
        CantVidaZelena -= Cant;
        Debug.Log("Vida actual:" + CantVidaZelena);
        if (CantVidaZelena <= 0 ){
            Muerto();
        }
    }

    // Función para que el fuego active la velocidad desde aquí
    public void ActivarEstadoFuego(float nuevaVelocidad, float tiempo)
    {
        StopAllCoroutines(); // Evitamos que se acumulen relojes si toca varios fuegos
        estaQuemandose = true;
        moveSpeed = nuevaVelocidad; // Cambia la velocidad en el frame exacto

        StartCoroutine(TemporizadorFuego(tiempo));
        Debug.Log($"🔥 ¡Personaje en llamas! Velocidad cambiada a {moveSpeed}");
    }

    // El temporizador ahora recibe el tiempo dinámicamente
    private IEnumerator TemporizadorFuego(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);

        estaQuemandose = false;
        moveSpeed = 8f; // Regresa a la velocidad normal de tu diseño

        Debug.Log("❄️ El fuego se apagó y la velocidad volvió a 8f");
    }


    void Muerto() {
        Debug.Log("Rip");
    }

>>>>>>> 67014d2f119c515c53203ebb6a51d4db85ee887b
}