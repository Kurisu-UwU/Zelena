using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // El objeto que va a seguir la cámara (tu Personaje)
    public float smoothSpeed = 5f; // Qué tan suave o rápido se mueve la cámara
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Desplazamiento (Z en -10 para que se vea en 2D)

    void LateUpdate()
    {
        if (target == null) return;

        // Posición a la que queremos que vaya la cámara
        Vector3 desiredPosition = target.position + offset;

        // Movimiento suave usando Lerp
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Actualizar la posición de la cámara
        transform.position = smoothedPosition;
    }
}
