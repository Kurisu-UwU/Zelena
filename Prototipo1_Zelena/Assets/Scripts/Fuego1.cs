using UnityEngine;

public class Fuego1 : MonoBehaviour
{
    public int CantDano = 10;
    public float IntervaloDeDano = 1.0f;
    private float TiempoDano;
    private void OnTriggerStay2D(Collider2D other){
        if (other.CompareTag("Player")){
            if (Time.time >= TiempoDano) {
                VidaZelena2 vidazelena = other.GetComponent<VidaZelena2>();
                if (vidazelena != null){
                    vidazelena.RecibeDano(CantDano);
                }
                TiempoDano = Time.time + IntervaloDeDano;
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
