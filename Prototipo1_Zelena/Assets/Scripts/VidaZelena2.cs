using UnityEngine;

public class VidaZelena2 : MonoBehaviour
{
    public int MaxVidaZelena = 100;
    private int CantVidaZelena;
    void Start()
    {
        CantVidaZelena = MaxVidaZelena;
    }
    public void RecibeDano(int Cant){
        CantVidaZelena -= Cant;
        Debug.Log("Vida actual:" + CantVidaZelena);
        if (CantVidaZelena <= 0 ){
            Muerto();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void Muerto() {
        Debug.Log("Rip");
    }
}
