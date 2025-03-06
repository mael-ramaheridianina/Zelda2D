using UnityEngine;

public class EnemyScript : MonoBehaviour
{ 
    [Header("Combat Settings")]
    [SerializeField] private int damageAmount = 1;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float patrolRange = 3f;  // Rayon de la zone de patrouille
    [SerializeField] private float returnSpeed = 1.5f; // Vitesse de retour à la zone

    [Header("Patrol Settings")]
    [SerializeField] private float patrolSpeed = 1f;
    [SerializeField] private float patrolDistance = 2f; // Distance de patrouille gauche/droite
    
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isPlayerInRange = false;
    private PlayerScript playerScript;  // Ajout de la référence au PlayerScript
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition; // Position initiale de l'ennemi
    private bool isReturning = false;
    private float patrolTimer = 0f;
    private bool patrollingRight = true;
    private Vector3 leftPatrolPoint;
    private Vector3 rightPatrolPoint;
    private Camera mainCamera;
    private bool isVisible = true;
    private bool isMovingToEdge = true; // true = va vers le bord, false = revient au centre
    private bool isMovingRight = true; // true = droite, false = gauche
    private const float CENTER_THRESHOLD = 0.1f; // Seuil pour détecter l'arrivée au centre

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerScript = player.GetComponent<PlayerScript>();

            if (playerScript == null)
            {
                Debug.LogError("PlayerScript not found on player object!");
            }
            Debug.Log("Player found!");
            Debug.Log("Is player celebrating? " + playerScript.IsCelebrating());
        }
        else
        {
            Debug.LogError("Player not found! Make sure it has the 'Player' tag.");
        }

        startPosition = transform.position; // Sauvegarde la position de départ

        // Calcul des points de patrouille
        leftPatrolPoint = startPosition + Vector3.left * patrolDistance;
        rightPatrolPoint = startPosition + Vector3.right * patrolDistance;

        mainCamera = Camera.main;
    }

    private bool IsVisibleToCamera()
    {
        var viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && 
               viewportPoint.y >= 0 && viewportPoint.y <= 1 && 
               viewportPoint.z > 0;
    }

    void Update()
    {
        bool isVisible = IsVisibleToCamera();
        float distanceToStart = Vector2.Distance(transform.position, startPosition);
        
        // Vérifie si l'ennemi est hors caméra et au centre
        if (!isVisible && distanceToStart < CENTER_THRESHOLD)
        {
            movement = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (playerScript != null && playerScript.IsCelebrating())
        {
            movement = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            if (animator != null)
            {
                animator.enabled = false;
            }
            return;
        }

        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            isPlayerInRange = distanceToPlayer <= detectionRange;

            if (isPlayerInRange)
            {
                // Poursuite du player quand il est détecté
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                movement = direction * moveSpeed;
                isReturning = false;
            }
            else
            {
                // Reste du code de patrouille inchangé
                isReturning = false;
                
                if (isMovingToEdge)
                {
                    // Déplacement vers le bord
                    Vector2 targetPosition = startPosition;
                    targetPosition.x += isMovingRight ? patrolRange : -patrolRange;
                    
                    if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
                    {
                        isMovingToEdge = false; // Commence à revenir au centre
                    }
                    
                    movement = (targetPosition - (Vector2)transform.position).normalized * patrolSpeed;
                }
                else
                {
                    // Retour au centre
                    if (distanceToStart < CENTER_THRESHOLD)
                    {
                        isMovingToEdge = true; // Repart vers un bord
                        isMovingRight = !isMovingRight; // Alterne la direction
                    }
                    
                    movement = ((Vector2)startPosition - (Vector2)transform.position).normalized * patrolSpeed;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (playerScript != null && playerScript.IsCelebrating())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = movement;
    }

    public int GetDamageAmount()
    {
        return damageAmount;
    }

    // Optionnel : Visualisation du range de détection dans l'éditeur
    private void OnDrawGizmosSelected()
    {
        // Zone de détection du player (rouge)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Zone de patrouille (bleue)
        Gizmos.color = Color.blue;
        Vector3 startPos = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawWireSphere(startPos, patrolRange);

        // Visualisation des points de patrouille
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(leftPatrolPoint, 0.2f);
            Gizmos.DrawWireSphere(rightPatrolPoint, 0.2f);
        }
    }
}
