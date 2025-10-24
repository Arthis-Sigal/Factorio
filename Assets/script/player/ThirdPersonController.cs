using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Entrées clavier (ZQSD)
        float horizontal = Input.GetAxis("Horizontal"); // Q / D
        float vertical = Input.GetAxis("Vertical");     // Z / S

        // direction du mouvement sur le plan XZ
        Vector3 move = new Vector3(horizontal, 0f, vertical).normalized;

        // déplace le joueur dans la direction
        controller.Move(move * moveSpeed * Time.deltaTime);

        // optionnel : oriente le joueur vers sa direction de déplacement
        if (move.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }
    }
}
