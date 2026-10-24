using System.Collections;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
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

    [Header("Dash")]
    public float dashSpeed = 22f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.8f;
    private bool canDash = true;
    private bool isDashing;

    [Header("Infección")]
    public string humanTag = "Human";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f;
            facingDirection = 1;
        }

        // Comprobación de si está en el suelo (Salto único)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        // Salto (Barra Espaciadora)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Dash (Tecla Left Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        // Aplicar movimiento horizontal manteniendo la velocidad vertical actual
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f; // Anular la gravedad temporalmente durante el dash

        // Determinar dirección del dash según el movimiento actual o la última dirección
        float dashDir = horizontalInput != 0 ? horizontalInput : facingDirection;
        rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // Mecánica de Infección por Colisión
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckAndInfect(collision.gameObject);
    }

    // Mecánica de Infección si el objeto humano es un Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject);
        CheckAndInfect(collision.gameObject);
    }

    void CheckAndInfect(GameObject target)
    {
        if (target.CompareTag(humanTag))
        {
            Debug.Log($"¡Objeto humano infectado: {target.name}!");
            Destroy(target);
        }
    }

    // Visualizar el círculo de detección de suelo en el editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}