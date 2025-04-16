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
    [SerializeField] private FoudreScript foudre;

    [Header("Camera Settings")]
    [SerializeField] private float cameraSmoothSpeed = 5f;
    private Vector3 cameraOffset;

    [Header("Time Control Settings")]
    [SerializeField] private float slowMotionTimeScale = 0.5f; // Vitesse du temps en mode visée
    private float originalTimeScale;

    [Header("Target Mode Duration Settings")]
    [SerializeField] private float[] targetModeDurations = { 1.0f, 2.0f, 3.0f }; // Durées pour niveaux 1, 2, 3
    private float targetModeTimer = 0f;

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
            
            // Gestion du minuteur pour le mode visé
            if (foudre != null)
            {
                int currentLevel = foudre.GetCurrentLevel();
                float maxDuration = GetTargetModeDurationForLevel(currentLevel);
                
                targetModeTimer += Time.unscaledDeltaTime; // Utiliser unscaledDeltaTime car le temps est ralenti
                
                // Afficher le temps restant si on approche de la limite
                float remainingTime = maxDuration - targetModeTimer;
                if (remainingTime <= 3.0f)
                {
                    Debug.Log($"Temps restant en mode visé: {remainingTime:F1} secondes");
                }
                
                // Quitter le mode visé quand le temps est écoulé
                if (targetModeTimer >= maxDuration)
                {
                    Debug.Log("Temps écoulé, sortie du mode visé");
                    HideViseur();
                    return;
                }
            }
            
            // Appuyer sur Y pour déclencher la foudre (sans limite du nombre d'appuis)
            if (Input.GetKeyDown(KeyCode.Y))
            {
                if (foudre != null)
                {
                    foudre.ShowFoudre(transform.position);
                }
            }

            // Toujours permettre de quitter manuellement avec R
            if (Input.GetKeyDown(KeyCode.R))
            {
                HideViseur();
            }
        }
    }

    // Obtenir la durée maximale selon le niveau
    private float GetTargetModeDurationForLevel(int level)
    {
        // S'assurer que l'index est valide
        if (level >= 1 && level <= targetModeDurations.Length)
        {
            return targetModeDurations[level - 1];
        }
        
        // Valeur par défaut
        return 1.0f;
    }

    void LateUpdate()
    {
        if (isVisible && mainCamera != null)
        {
            // Mouvement de caméra sans tremblements
            Vector3 targetPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                mainCamera.transform.position.z
            );
            
            // Damping plus fort pour réduire les oscillations
            float smoothFactor = cameraSmoothSpeed * Time.unscaledDeltaTime;
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                smoothFactor
            );
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

        if (mainCamera != null)
        {
            cameraOffset = mainCamera.transform.position - centerPosition;
        }

        // Ralentit le temps
        originalTimeScale = Time.timeScale; // Sauvegarde l'échelle actuelle
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Ajuste le fixedDeltaTime pour la physique
        
        Debug.Log($"Entering slow motion: {Time.timeScale}x speed");

        // Réinitialiser le minuteur du mode visé
        targetModeTimer = 0f;
    }

    private void HideViseur()
    {
        isVisible = false;

        // Réinitialiser le minuteur
        targetModeTimer = 0f;

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
        
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0) * moveSpeed * Time.unscaledDeltaTime;
        transform.position += movement;
    }

    // Méthode pour éventuellement prolonger la durée
    public void ExtendTargetModeDuration(float additionalTime)
    {
        // Peut être utilisé pour des power-ups qui prolongent le temps
        targetModeTimer -= additionalTime;
        if (targetModeTimer < 0) targetModeTimer = 0;
    }
}
