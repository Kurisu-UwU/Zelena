using UnityEngine;
using UnityEngine.SceneManagement;

public class Esc2a3 : MonoBehaviour
{
    [SerializeField] private string escenaDestino = "Escenario3";
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Colisión con: " + other.name);
        if (other.name == "Personaje"){
            Debug.Log("Cargando: " + escenaDestino);
            SceneManager.LoadScene(escenaDestino);
        }
    }
}
