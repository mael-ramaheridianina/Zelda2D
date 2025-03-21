using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private float cameraSpeed = 5f; // Vitesse de déplacement manuel
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    private bool isFollowing = true;
    private Vector3 lastValidPosition;
    private bool hasValidPosition = false;

    void Start()
    {
        if (target != null)
        {
            lastValidPosition = target.position;
            hasValidPosition = true;
        }
    }

    void Update()
    {
        if (!isFollowing)
        {
            // Déplacement manuel avec les flèches
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            
            Vector3 movement = new Vector3(horizontalInput, verticalInput, 0);
            transform.position += movement * cameraSpeed * Time.deltaTime;
        }
        else if (target != null)
        {
            // Suivi normal du player
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            lastValidPosition = transform.position - offset;
            hasValidPosition = true;
        }
        // Sinon on reste à la dernière position valide
        else if (hasValidPosition)
        {
            transform.position = lastValidPosition + offset;
        }
    }

    public void StopFollowing()
    {
        isFollowing = false;
        // Sauvegarde la position actuelle comme dernière position valide
        if (target != null)
        {
            lastValidPosition = target.position;
            hasValidPosition = true;
        }
    }

    public void StartFollowing()
    {
        isFollowing = true;
    }
}
