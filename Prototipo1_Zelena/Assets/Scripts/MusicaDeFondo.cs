using UnityEngine;

public class MusicaDeFondo : MonoBehaviour
{
    private static MusicaDeFondo instancia;

    private void Awake()
    {
        if (instancia != null)
        {
            Destroy(gameObject);
            return;
        }
        instancia = this;
        DontDestroyOnLoad(gameObject);
    }
}