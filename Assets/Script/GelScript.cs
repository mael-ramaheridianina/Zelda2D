using UnityEngine;

public class GelScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float detectionRange = 5f;
    
    [Header("Combat Settings")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    private Transform playerTransform;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isJumping = false;
    private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
    private PlayerScript playerScript;
    private float savedAnimationTime = 0f;
    private bool wasCelebrating = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerScript = player.GetComponent<PlayerScript>();
        }
    }

    void Update()
    {
        if (playerScript != null && playerScript.IsCelebrating())
        {
            if (!wasCelebrating)
            {
                // Sauvegarde le temps de l'animation actuelle
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Gel_Saute"))
                {
                    savedAnimationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                }
                // Passe à l'animation Idle
                animator.Play("Idle");
                rb.linearVelocity = Vector2.zero;
            }
            wasCelebrating = true;
            return;
        }

        // Si le player vient d'arrêter de célébrer
        if (wasCelebrating)
        {
            wasCelebrating = false;
            if (savedAnimationTime > 0)
            {
                // Reprend l'animation Gel_Saute là où elle s'était arrêtée
                animator.Play("Gel_Saute", -1, savedAnimationTime);
                savedAnimationTime = 0;
            }
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        // Si le joueur sort de la zone de détection
        if (distanceToPlayer > detectionRange)
        {
            ReturnToIdle();
            return;
        }

        // Démarre le saut si on est en Idle et que le joueur est dans la zone
        if (!isJumping && !animator.GetCurrentAnimatorStateInfo(0).IsName("Gel_Saute"))
        {
            StartJump();
        }
    }

    private void StartJump()
    {
        isJumping = true;
        animator.SetBool(IsJumpingHash, true);
    }

    private void ReturnToIdle()
    {
        isJumping = false;
        animator.SetBool(IsJumpingHash, false);
        rb.linearVelocity = Vector2.zero;
        // Force le retour à l'animation Idle
        animator.Play("Gel_Idle");
    }

    // Méthode à appeler via Animation Event pendant l'animation de saut
    public void ApplyJumpMovement()
    {
        if (playerTransform != null)
        {
            // Vérifie si le joueur est toujours dans la zone avant d'appliquer le mouvement
            float currentDistance = Vector2.Distance(transform.position, playerTransform.position);
            if (currentDistance <= detectionRange)
            {
                Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
                rb.linearVelocity = directionToPlayer * jumpForce;
            }
            else
            {
                ReturnToIdle();
            }
        }
    }

    public void OnJumpEnd()
    {
        isJumping = false;
        animator.SetBool(IsJumpingHash, false);
        rb.linearVelocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerScript player = collision.gameObject.GetComponent<PlayerScript>();
            HeartManager heartManager = FindFirstObjectByType<HeartManager>();
            
            if (player != null && heartManager != null)
            {
                heartManager.TakeDamage(damageAmount);
                
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                
                player.StartKnockback(knockbackDirection);
                player.StartInvulnerability();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
