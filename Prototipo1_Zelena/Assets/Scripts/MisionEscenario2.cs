using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MisionEscenario2 : MonoBehaviour
{
    private enum Estado { Menu, Jugando, Pausa, Victoria, Derrota }

    public static MisionEscenario2 Instancia { get; private set; }
    public AudioClip SonidoMuro => sonidoRomper;

    [SerializeField] private AudioClip sonidoFuego = null;
    [SerializeField] private AudioClip sonidoCorte = null;
    [SerializeField] private AudioClip sonidoRomper = null;
    [SerializeField] private AudioClip sonidoDucto = null;

    private const int PersonasNecesarias = 3;
    private Estado estado = Estado.Menu;
    private PlayerController2D jugador;
    private Camera camara;
    private CameraFollow seguimiento;
    private Sprite spriteSuelo;
    private Sprite spritePlataforma;
    private Sprite spriteMuro;
    private Sprite spritePilar;
    private Sprite spriteFachada;
    private Sprite spritePuerta;
    private Sprite spriteCaja;
    private Sprite spriteTanque;
    private Sprite spritePersonaVerde;
    private Sprite spritePersonaAzul;
    private Sprite spritePersonaAmarilla;
    private Sprite spriteHacha;
    private Sprite spriteZelena;
    private Sprite[] spritesMovimiento;
    private Sprite spriteSalto;
    private int personasRescatadas;
    private string mensaje = string.Empty;
    private float mensajeHasta;
    private GUIStyle estiloBoton;
    private GUIStyle estiloTexto;
    private GUIStyle estiloTitulo;
    private GUIStyle estiloPequeno;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        camara = GetComponent<Camera>();
        if (camara == null)
        {
            Debug.LogError("MisionEscenario2 debe estar en la camara principal.");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        jugador = GameObject.Find("Personaje")?.GetComponent<PlayerController2D>();
        if (jugador == null)
        {
            Debug.LogError("No se encontro el objeto Personaje con PlayerController2D en Escenario2.");
            enabled = false;
            return;
        }

        CargarSprites();
        PrepararJugador();
        seguimiento = GetComponent<CameraFollow>();
        if (seguimiento != null)
        {
            seguimiento.target = jugador.transform;
            seguimiento.smoothSpeed = 4f;
            seguimiento.enabled = false;
        }

        camara.orthographic = true;
        camara.orthographicSize = 7.2f;
        camara.clearFlags = CameraClearFlags.SolidColor;
        camara.backgroundColor = new Color(0.055f, 0.075f, 0.105f);
        LimpiarEscenarioAnterior();
        CrearEscenario();
        jugador.gameObject.SetActive(false);
    }

    private void CargarSprites()
    {
        spriteSuelo = CrearSprite("plataforma_larga");
        spritePlataforma = CrearSprite("plataforma_media");
        spriteMuro = CrearSprite("muro_ladrillo");
        spritePilar = CrearSprite("muro_vertical");
        spriteFachada = CrearSprite("fachada");
        spritePuerta = CrearSprite("puerta_salida");
        spriteCaja = CrearSprite("caja_metal");
        spriteTanque = CrearSprite("tanque_fuego");
        spritePersonaVerde = CrearSprite("persona_verde");
        spritePersonaAzul = CrearSprite("persona_azul");
        spritePersonaAmarilla = CrearSprite("persona_amarilla");
        spriteHacha = CrearSprite("hacha_trampa");
        spriteZelena = CrearSprite("zelena_idle");
        spritesMovimiento = new[]
        {
            CrearSprite("zelena_walk_0"),
            CrearSprite("zelena_walk_1")
        };
        spriteSalto = CrearSprite("zelena_jump");
    }

    private static Sprite CrearSprite(string nombre)
    {
        Texture2D textura = Resources.Load<Texture2D>("CityEscape/" + nombre);
        if (textura == null)
        {
            throw new InvalidOperationException(
                "No se pudo cargar Assets/Resources/CityEscape/" + nombre + ".png");
        }

        float pixelsPerUnit = nombre.StartsWith("zelena_", StringComparison.Ordinal) ? 500f : 100f;
        return Sprite.Create(textura, new Rect(0f, 0f, textura.width, textura.height),
            new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }

    private void PrepararJugador()
    {
        jugador.transform.position = new Vector3(-15f, -0.25f, 0f);
        jugador.transform.localScale = Vector3.one;
        jugador.ConfigurarVelocidadBase(8f);
        jugador.UsarHudEscenario(false);
        jugador.jumpForce = 11f;
        jugador.dashSpeed = 22f;
        jugador.groundCheckRadius = 0.18f;
        jugador.ConfigurarSprites(spriteZelena, spritesMovimiento, spriteSalto);

        BoxCollider2D colisionador = jugador.GetComponent<BoxCollider2D>();
        if (colisionador != null)
        {
            colisionador.size = new Vector2(0.82f, 1.35f);
            colisionador.offset = new Vector2(0f, 0f);
        }

        if (jugador.groundCheck != null)
        {
            jugador.groundCheck.localPosition = new Vector3(0f, -0.7f, 0f);
        }

        Rigidbody2D cuerpo = jugador.GetComponent<Rigidbody2D>();
        if (cuerpo != null)
        {
            cuerpo.linearVelocity = Vector2.zero;
            cuerpo.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            cuerpo.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void LimpiarEscenarioAnterior()
    {
        foreach (GameObject objeto in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (objeto == gameObject || objeto == jugador.gameObject ||
                objeto.name == "Global Light 2D")
            {
                continue;
            }

            Destroy(objeto);
        }
    }

    private void CrearEscenario()
    {
        for (float x = -22f; x < 46f; x += 10.2f)
        {
            CrearDecoracion("Fachada de ciudad", new Vector2(x, 1.2f),
                new Vector2(10.2f, 16f), spriteFachada, -10);
        }

        for (float x = -20f; x < 44f; x += 8.2f)
        {
            CrearBloque("Piso de piedra", new Vector2(x + 4.1f, -1.65f),
                new Vector2(8.2f, 1.3f), spriteSuelo, 3, true);
        }

        CrearPlataforma(new Vector2(-8f, 1.4f), new Vector2(7f, 0.8f));
        CrearPlataforma(new Vector2(0f, 3.8f), new Vector2(6f, 0.8f));
        CrearPlataforma(new Vector2(11f, 2.3f), new Vector2(7f, 0.8f));
        CrearPlataforma(new Vector2(23f, 3.2f), new Vector2(8f, 0.8f));
        CrearPlataforma(new Vector2(32f, 1.7f), new Vector2(7f, 0.8f));

        CrearBloque("Limite exterior izquierdo", new Vector2(-22f, 4f),
            new Vector2(1.4f, 12f), spritePilar, 3, true);
        CrearBloque("Limite exterior derecho", new Vector2(43f, 4f),
            new Vector2(1.4f, 12f), spritePilar, 3, true);

        CrearConductoCorto();
        CrearObstaculos();
        CrearPersonas();
        CrearSalida();
    }

    private void CrearConductoCorto()
    {
        CrearPlataforma(new Vector2(4.9f, 0.55f), new Vector2(4f, 0.42f));
        CrearBloque("Muro del conducto", new Vector2(5f, 1.65f),
            new Vector2(1.1f, 1.8f), spritePilar, 3, true);

        GameObject cuchilla = CrearDecoracion("Cuchilla para encogerse",
            new Vector2(2.6f, -0.22f), new Vector2(0.85f, 1.2f), spriteHacha, 2);
        BoxCollider2D cuchillaTrigger = cuchilla.AddComponent<BoxCollider2D>();
        cuchillaTrigger.size = new Vector2(0.75f, 1.25f);
        cuchillaTrigger.isTrigger = true;
        Hacha hacha = cuchilla.AddComponent<Hacha>();
        hacha.escalaPequena = new Vector3(0.5f, 0.5f, 1f);
        hacha.ConfigurarSonido(sonidoCorte);

        GameObject salidaDucto = new GameObject("Salida del conducto");
        salidaDucto.transform.position = new Vector3(7.2f, -0.25f, 0f);
        BoxCollider2D salidaTrigger = salidaDucto.AddComponent<BoxCollider2D>();
        salidaTrigger.size = new Vector2(0.55f, 1.7f);
        salidaTrigger.isTrigger = true;
        Piernas piernas = salidaDucto.AddComponent<Piernas>();
        piernas.escalaOriginal = Vector3.one;
        piernas.ConfigurarSonido(sonidoDucto);
    }

    private void CrearObstaculos()
    {
        CrearBloqueInterior("Muro interior rompible", new Vector2(15.2f, 1.2f),
            new Vector2(1.05f, 4.4f), spritePilar);
        CrearBloqueInterior("Muro interior rompible", new Vector2(27f, 1.5f),
            new Vector2(1.05f, 4.8f), spriteMuro);

        CrearBloque("Caja metalica", new Vector2(10f, -0.45f),
            new Vector2(1.1f, 1.05f), spriteCaja, 3, false);
        CrearBloque("Caja metalica", new Vector2(19.3f, -0.45f),
            new Vector2(1.1f, 1.05f), spriteCaja, 3, false);
        CrearBloque("Caja metalica", new Vector2(31f, -0.45f),
            new Vector2(1.1f, 1.05f), spriteCaja, 3, false);

        GameObject tanque = CrearDecoracion("Tanque de fuego",
            new Vector2(11.8f, -0.25f), new Vector2(0.75f, 1.35f), spriteTanque, 2);
        BoxCollider2D zonaFuego = tanque.AddComponent<BoxCollider2D>();
        zonaFuego.size = new Vector2(1f, 1.4f);
        zonaFuego.isTrigger = true;
        Fuego1 fuego = tanque.AddComponent<Fuego1>();
        fuego.CantDano = 10;
        fuego.ConfigurarEfecto(spriteTanque, sonidoFuego);
    }

    private void CrearPersonas()
    {
        CrearPersona("Persona rescatable 1", new Vector2(12.8f, -0.15f), spritePersonaVerde);
        CrearPersona("Persona rescatable 2", new Vector2(21.8f, -0.15f), spritePersonaAzul);
        CrearPersona("Persona rescatable 3", new Vector2(33f, -0.15f), spritePersonaAmarilla);
    }

    private void CrearPersona(string nombre, Vector2 posicion, Sprite imagen)
    {
        GameObject persona = CrearDecoracion(nombre, posicion, new Vector2(0.72f, 1.15f), imagen, 1);
        persona.tag = "Human";
        BoxCollider2D zona = persona.AddComponent<BoxCollider2D>();
        zona.size = new Vector2(0.8f, 1.2f);
        zona.isTrigger = true;
    }

    private void CrearSalida()
    {
        GameObject salida = CrearDecoracion("Salida final",
            new Vector2(39f, -0.15f), new Vector2(1.35f, 2.1f), spritePuerta, 1);
        BoxCollider2D zona = salida.AddComponent<BoxCollider2D>();
        zona.size = new Vector2(1.2f, 2.1f);
        zona.isTrigger = true;
        salida.AddComponent<SalidaNivel>();
    }

    private void CrearBloqueInterior(string nombre, Vector2 posicion, Vector2 tamano, Sprite imagen)
    {
        GameObject muro = CrearBloque(nombre, posicion, tamano, imagen, 3, true);
        muro.AddComponent<MuroInterior>();
    }

    private void CrearPlataforma(Vector2 posicion, Vector2 tamano)
    {
        CrearBloque("Plataforma elevada", posicion, tamano, spritePlataforma, 3, true);
    }

    private GameObject CrearBloque(
        string nombre, Vector2 posicion, Vector2 tamano, Sprite imagen, int capa, bool solido)
    {
        GameObject bloque = CrearDecoracion(nombre, posicion, tamano, imagen, 0);
        bloque.layer = capa;
        BoxCollider2D colisionador = bloque.AddComponent<BoxCollider2D>();
        colisionador.size = new Vector2(
            tamano.x / bloque.transform.localScale.x,
            tamano.y / bloque.transform.localScale.y);
        if (!solido)
        {
            colisionador.isTrigger = true;
        }
        return bloque;
    }

    private GameObject CrearDecoracion(
        string nombre, Vector2 posicion, Vector2 tamano, Sprite imagen, int orden)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        SpriteRenderer renderer = objeto.AddComponent<SpriteRenderer>();
        renderer.sprite = imagen;
        renderer.sortingOrder = orden;
        Vector2 medidasImagen = imagen.bounds.size;
        objeto.transform.localScale = new Vector3(
            tamano.x / medidasImagen.x,
            tamano.y / medidasImagen.y,
            1f);
        return objeto;
    }

    private void Update()
    {
        if (estado == Estado.Jugando && Input.GetKeyDown(KeyCode.Escape))
        {
            estado = Estado.Pausa;
            Time.timeScale = 0f;
        }

        if (estado == Estado.Jugando && jugador != null && jugador.transform.position.y < -9f)
        {
            jugador.Morir();
        }
    }

    private void LateUpdate()
    {
        if (estado != Estado.Jugando || jugador == null)
        {
            return;
        }

        Vector3 objetivo = new Vector3(
            Mathf.Clamp(jugador.transform.position.x, -14f, 34f),
            Mathf.Clamp(jugador.transform.position.y + 1.2f, 0f, 2.8f),
            -10f);
        camara.transform.position = Vector3.Lerp(
            camara.transform.position, objetivo, Time.deltaTime * 3.5f);
    }

    public void PersonaRescatada()
    {
        if (estado != Estado.Jugando)
        {
            return;
        }

        personasRescatadas = Mathf.Min(PersonasNecesarias, personasRescatadas + 1);
        MostrarMensaje("¡Persona rescatada! " + personasRescatadas + "/" + PersonasNecesarias);
    }

    public void IntentarSalir()
    {
        if (estado != Estado.Jugando)
        {
            return;
        }

        if (personasRescatadas < PersonasNecesarias)
        {
            MostrarMensaje("Rescata a las 3 personas antes de salir.");
            return;
        }

        estado = Estado.Victoria;
        Time.timeScale = 0f;
    }

    public void JugadorDerrotado()
    {
        if (estado == Estado.Jugando)
        {
            estado = Estado.Derrota;
            Time.timeScale = 0f;
        }
    }

    private void MostrarMensaje(string texto)
    {
        mensaje = texto;
        mensajeHasta = Time.unscaledTime + 2.5f;
    }

    private void OnGUI()
    {
        PrepararEstilos();
        if (estado == Estado.Menu)
        {
            DibujarPanel("ZELENA", "La ciudad de los no muertos",
                "Rescata a las 3 personas y alcanza la salida.\n\n" +
                "A/D o flechas: moverte     ESPACIO: saltar\n" +
                "SHIFT: dash     ESC: pausar\n\n" +
                "Toca la cuchilla para encogerte y pasar por el conducto. " +
                "El fuego te da un impulso que rompe muros interiores.",
                "JUGAR", IniciarJuego);
            return;
        }

        if (estado == Estado.Jugando)
        {
            DibujarHud();
            if (GUI.Button(new Rect(Screen.width - 148f, 18f, 126f, 42f),
                "PAUSAR  II", estiloBoton))
            {
                estado = Estado.Pausa;
                Time.timeScale = 0f;
            }

            if (Time.unscaledTime < mensajeHasta)
            {
                GUI.color = new Color(0.08f, 0.11f, 0.16f, 0.92f);
                GUI.DrawTexture(new Rect((Screen.width - 420f) * 0.5f, 90f, 420f, 48f),
                    Texture2D.whiteTexture);
                GUI.color = Color.white;
                GUI.Label(new Rect((Screen.width - 400f) * 0.5f, 97f, 400f, 36f),
                    mensaje, estiloTexto);
            }
            return;
        }

        if (estado == Estado.Pausa)
        {
            DibujarPanel("PAUSA", "Toma aire, Zelena",
                "Las personas rescatadas se quedan contigo.",
                "CONTINUAR", ContinuarJuego, "REINICIAR");
            return;
        }

        if (estado == Estado.Victoria)
        {
            DibujarPanel("¡LO LOGRASTE!", "La ciudad vuelve a tener esperanza",
                "Rescataste a las 3 personas y encontraste la salida.\n¡Buen trabajo, Zelena!",
                "JUGAR OTRA VEZ", ReiniciarNivel);
            return;
        }

        DibujarPanel("¡INTÉNTALO DE NUEVO!", "Esta vez llegarás más lejos",
            "La ciudad aún necesita tu ayuda.\nPuedes volver a intentarlo cuando quieras.",
            "REINTENTAR", ReiniciarNivel);
    }

    private void DibujarHud()
    {
        float vida = jugador != null
            ? Mathf.Clamp01((float)jugador.VidaActual / Mathf.Max(1, jugador.MaxVidaZelena))
            : 0f;
        GUI.color = new Color(0.035f, 0.055f, 0.085f, 0.93f);
        GUI.DrawTexture(new Rect(18f, 18f, 270f, 84f), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(32f, 25f, 240f, 28f),
            $"VIDA   {Mathf.RoundToInt(vida * 100f)}%       PERSONAS   {personasRescatadas}/{PersonasNecesarias}",
            estiloTexto);
        GUI.color = new Color(0.16f, 0.2f, 0.24f, 1f);
        GUI.DrawTexture(new Rect(32f, 63f, 238f, 18f), Texture2D.whiteTexture);
        GUI.color = vida > 0.45f
            ? new Color(0.29f, 0.84f, 0.44f, 1f)
            : new Color(1f, 0.38f, 0.29f, 1f);
        GUI.DrawTexture(new Rect(32f, 63f, 238f * vida, 18f), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void DibujarPanel(
        string titulo, string subtitulo, string descripcion, string textoBoton,
        Action accionBoton, string segundoBoton = null)
    {
        float ancho = Mathf.Min(640f, Screen.width - 36f);
        float alto = Mathf.Min(430f, Screen.height - 36f);
        Rect panel = new Rect((Screen.width - ancho) * 0.5f,
            (Screen.height - alto) * 0.5f, ancho, alto);
        GUI.color = new Color(0.025f, 0.035f, 0.065f, 0.94f);
        GUI.DrawTexture(panel, Texture2D.whiteTexture);
        GUI.color = new Color(0.76f, 0.29f, 0.83f, 1f);
        GUI.DrawTexture(new Rect(panel.x, panel.y, panel.width, 8f), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(panel.x + 28f, panel.y + 30f, panel.width - 56f, 62f),
            titulo, estiloTitulo);
        GUI.Label(new Rect(panel.x + 30f, panel.y + 95f, panel.width - 60f, 38f),
            subtitulo, estiloTexto);
        GUI.Label(new Rect(panel.x + 36f, panel.y + 145f, panel.width - 72f, 150f),
            descripcion, estiloPequeno);

        Rect botonPrincipal = new Rect(panel.x + 54f, panel.y + alto - 94f,
            panel.width - 108f, 48f);
        if (GUI.Button(botonPrincipal, textoBoton, estiloBoton))
        {
            accionBoton();
        }

        if (segundoBoton != null)
        {
            Rect botonReinicio = new Rect(panel.x + 54f, panel.y + alto - 42f,
                panel.width - 108f, 34f);
            if (GUI.Button(botonReinicio, segundoBoton, estiloPequeno))
            {
                ReiniciarNivel();
            }
        }
    }

    private void PrepararEstilos()
    {
        if (estiloTexto != null)
        {
            return;
        }

        estiloTitulo = new GUIStyle(GUI.skin.label)
        {
            fontSize = 34,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = new Color(0.95f, 0.78f, 0.98f) }
        };
        estiloTexto = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };
        estiloPequeno = new GUIStyle(GUI.skin.label)
        {
            fontSize = 15,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            normal = { textColor = new Color(0.83f, 0.87f, 0.94f) }
        };
        estiloBoton = new GUIStyle(GUI.skin.button)
        {
            fontSize = 19,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal =
            {
                textColor = Color.white,
                background = CrearTexturaBoton(new Color(0.34f, 0.68f, 0.38f))
            },
            hover =
            {
                textColor = Color.white,
                background = CrearTexturaBoton(new Color(0.43f, 0.79f, 0.47f))
            }
        };
    }

    private static Texture2D CrearTexturaBoton(Color color)
    {
        Texture2D textura = new Texture2D(1, 1);
        textura.SetPixel(0, 0, color);
        textura.Apply();
        return textura;
    }

    private void IniciarJuego()
    {
        personasRescatadas = 0;
        estado = Estado.Jugando;
        jugador.gameObject.SetActive(true);
    }

    private void ContinuarJuego()
    {
        estado = Estado.Jugando;
        Time.timeScale = 1f;
    }

    private void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
