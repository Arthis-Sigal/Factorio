using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;                 // le joueur
    public Vector3 offset = new Vector3(0, 15, -15); // position isométrique
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // position de la caméra
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // regarde le joueur (vers le bas)
        transform.LookAt(target.position);
    }
}
