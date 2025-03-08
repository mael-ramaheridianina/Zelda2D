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

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        isJumping =true;
        if (!isJumping && Vector2.Distance(transform.position, playerTransform.position) <= detectionRange)
        {
            StartJump();
        }
    }

    private void StartJump()
    {
        isJumping = true;
        animator.SetBool(IsJumpingHash, true);
    }

    // Cette méthode sera appelée via Animation Event sur la frame spécifique
    public void ApplyJumpMovement()
    {
        if (playerTransform != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            rb.linearVelocity = directionToPlayer * jumpForce;
        }
    }

    // Cette méthode sera appelée via Animation Event à la fin de l'animation
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
                // Infliger les dégâts
                heartManager.TakeDamage(damageAmount);
                
                // Calculer la direction du knockback (du Gel vers le joueur)
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                
                // Appliquer le knockback et l'invulnérabilité
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
