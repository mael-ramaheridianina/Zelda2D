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

    [Header("Time Control Settings")]
    [SerializeField] private float slowMotionTimeScale = 0.5f; // Vitesse du temps en mode visée
    private float originalTimeScale;

    void Start()
    {
        Debug.Log("Start appelé");
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Pas de caméra principale trouvée!");
            return;
        }

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
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

        // Stocke la valeur originale du timeScale
        originalTimeScale = Time.timeScale;
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

        // Désactiver le collider du player
        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                playerCollider.enabled = false;
                Debug.Log("Player collider disabled");
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

        // Ralentit le temps
        originalTimeScale = Time.timeScale; // Sauvegarde l'échelle actuelle
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Ajuste le fixedDeltaTime pour la physique
        
        Debug.Log($"Entering slow motion: {Time.timeScale}x speed");
    }

    private void HideViseur()
    {
        isVisible = false;
        yPressCount = 0;

        // Restaure le temps normal
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = 0.02f; // Restaure la valeur par défaut
        
        Debug.Log($"Exiting slow motion, restored to {Time.timeScale}x speed");

        foreach (var renderer in spriteRenderers)
        {
            renderer.enabled = false;
        }
        
        if (player != null)
        {
            // Réactiver le collider du player
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                playerCollider.enabled = true;
                Debug.Log("Player collider enabled");
            }
            
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
        
        // Utiliser Time.unscaledDeltaTime pour un mouvement uniforme 
        // indépendamment du ralenti
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0) * moveSpeed * Time.unscaledDeltaTime;
        transform.position += movement;

        // Mouvement de caméra sans tremblements
        if (mainCamera != null)
        {
            // Calculer la position cible
            Vector3 targetPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                mainCamera.transform.position.z
            );
            
            // Utiliser unscaledDeltaTime pour éviter l'influence du ralenti
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                cameraSmoothSpeed * Time.unscaledDeltaTime
            );
        }
    }

    public void IncreaseMaxYPresses()
    {
        maxYPresses++;
        Debug.Log($"Maximum Y presses increased to: {maxYPresses}");
    }
}
