using UnityEngine;

public class ViseurScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    private Camera mainCamera;
    private bool isVisible = false;
    public bool IsVisible 
    { 
        get { return isVisible; } 
    }
    private SpriteRenderer[] spriteRenderers;
    [SerializeField] private PlayerScript player;
    private int yPressCount = 0;
    private int maxYPresses = 3;  // Default maximum Y presses
    [SerializeField] private FoudreScript foudre;

    [Header("Camera Settings")]
    [SerializeField] private float cameraSmoothSpeed = 5f;
    private Vector3 cameraOffset;

    void Start()
    {
        Debug.Log("Start appelé");
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Pas de caméra principale trouvée!");
            return;
        }

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0)
        {
            Debug.LogError("Aucun SpriteRenderer trouvé!");
            return;
        }

        foreach (var renderer in spriteRenderers)
        {
            renderer.enabled = false;
        }
        isVisible = false;

        if (mainCamera != null)
        {
            cameraOffset = mainCamera.transform.position - transform.position;
        }
    }

    void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        if (isVisible)
        {
            MoveViseur();
            
            if (Input.GetKeyDown(KeyCode.Y))
            {
                yPressCount++;
                if (foudre != null)
                {
                    foudre.ShowFoudre(transform.position);
                }
                
                // Changed to use maxYPresses instead of hardcoded 3
                if (yPressCount >= maxYPresses)
                {
                    HideViseur();
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                HideViseur();
            }
        }
    }

    public void ShowViseur()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Pas de caméra principale trouvée!");
                return;
            }
        }

        isVisible = true;
        foreach (var renderer in spriteRenderers)
        {
            renderer.enabled = true;
        }

        Vector3 centerPosition = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        centerPosition.z = 0;
        transform.position = centerPosition;
        yPressCount = 0;

        if (mainCamera != null)
        {
            cameraOffset = mainCamera.transform.position - centerPosition;
        }
    }

    private void HideViseur()
    {
        isVisible = false;
        yPressCount = 0;
        foreach (var renderer in spriteRenderers)
        {
            renderer.enabled = false;
        }
        if (player != null)
        {
            player.ResetFromViseur();
        }

        if (mainCamera != null && player != null)
        {
            // La caméra retournera au player via le PlayerScript
            mainCamera.transform.position = new Vector3(
                player.transform.position.x,
                player.transform.position.y,
                mainCamera.transform.position.z
            );
        }
    }

    private void MoveViseur()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0) * moveSpeed * Time.deltaTime;
        transform.position += movement;

        // Déplacement fluide de la caméra
        if (mainCamera != null)
        {
            Vector3 desiredPosition = transform.position + cameraOffset;
            Vector3 smoothedPosition = Vector3.Lerp(
                mainCamera.transform.position,
                desiredPosition,
                cameraSmoothSpeed * Time.deltaTime
            );
            mainCamera.transform.position = new Vector3(
                smoothedPosition.x,
                smoothedPosition.y,
                mainCamera.transform.position.z
            );
        }
    }

    public void IncreaseMaxYPresses()
    {
        maxYPresses++;
        Debug.Log($"Maximum Y presses increased to: {maxYPresses}");
    }
}
