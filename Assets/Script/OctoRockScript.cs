using UnityEngine;
using System.Collections;

public class OctoRockScript : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 5f;
    
    private Transform playerTransform;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isPlayerInRange = false;
    private bool hasEmerged = false;
    private Camera mainCamera;
    
    // Nouveaux paramètres d'animation
    private static readonly int DirectionHash = Animator.StringToHash("Direction");
    private static readonly int IsEmerging = Animator.StringToHash("IsEmerging");
    private static readonly int IsShootingHash = Animator.StringToHash("IsShooting");

    // Énumération pour les directions
    private enum EmergingDirection
    {
        Down = 0,
        Right = 1,
        Up = 2,
        Left = 3
    }

    // Ajoutez ces variables en haut de la classe
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootDelay = 2f;
    private float shootTimer;
    private bool canShoot = false;

    [Header("Combat Settings")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private int health = 3;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private Rigidbody2D rb;

    [SerializeField] private float pauseDuration = 0.5f; // Durée de la pause en seconde
    private bool isPlayingAnimation = false;

    [Header("Animation Settings")]
    [SerializeField] private float timeBeforeHide = 2f;
    private float noShootTimer = 0f;
    private EmergingDirection lastEmergingDirection;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        
        if (animator == null)
        {
            Debug.LogError("Animator not found on OctoRock object!");
        }
        animator.SetBool(IsEmerging, false);
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure it has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockedBack = false;
                rb.linearVelocity = Vector2.zero;
                return;
            }
        }

        if (playerTransform == null) return;

        if (!IsVisibleOnScreen())
        {
            animator.SetBool(IsEmerging, false);
            hasEmerged = false;
            canShoot = false;
            animator.SetBool(IsEmerging, false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        bool wasPlayerInRange = isPlayerInRange;
        isPlayerInRange = distanceToPlayer <= detectionRange;

        if (isPlayerInRange && !wasPlayerInRange && !hasEmerged)
        {
            TriggerEmergeAnimation();
        }

        // Gestion du tir
        if (hasEmerged && canShoot)
        {
            if (!IsPlayerInShootingDirection())
            {
                animator.SetBool(IsShootingHash, false);
                noShootTimer += Time.deltaTime;

                // Vérifie la direction d'émergence et joue l'animation correspondante
                if (noShootTimer >= timeBeforeHide)
                {
                    string animationName = lastEmergingDirection switch
                    {
                        EmergingDirection.Down => "Octorok_RentreDansLaTerre_Bas",
                        EmergingDirection.Up => "Octorok_RentreDansLaTerre_Haut",
                        EmergingDirection.Right => "Octorok_RentreDansLaTerre_Droite",
                        EmergingDirection.Left => "Octorok_RentreDansLaTerre_Gauche",
                        _ => null
                    };

                    if (animationName != null)
                    {
                        animator.Play(animationName);
                        hasEmerged = false;
                        canShoot = false;
                        noShootTimer = 0f;
                    }
                }
                return;
            }

            // Réinitialise le timer quand l'Octorok tire
            noShootTimer = 0f;

            shootTimer += Time.deltaTime;
            if (shootTimer >= shootDelay)
            {
                Shoot();
                shootTimer = 0f;
            }
        }
    }

    private void TriggerEmergeAnimation()
    {
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        EmergingDirection direction = GetEmergingDirection(directionToPlayer);
        
        animator.SetInteger(DirectionHash, (int)direction);
        animator.SetBool(IsEmerging, true);
        hasEmerged = true;
        canShoot = true;
        shootTimer = shootDelay; // Pour tirer immédiatement après l'émergence

        lastEmergingDirection = direction; // Stocke la direction d'émergence

        Debug.Log("OctoRock emerged from the ground!");
        Debug.Log($"Emerging direction: {direction}");
    }

    private EmergingDirection GetEmergingDirection(Vector2 direction)
    {
        // Calcul de l'angle entre la direction et le vecteur (1,0)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Normalisation de l'angle entre 0 et 360
        if (angle < 0) angle += 360;

        // Conversion de l'angle en direction
        if (angle >= 315 || angle < 45) return EmergingDirection.Right;
        if (angle >= 45 && angle < 135) return EmergingDirection.Up;
        if (angle >= 135 && angle < 225) return EmergingDirection.Left;
        return EmergingDirection.Down;
    }

    private bool IsVisibleOnScreen()
    {
        if (spriteRenderer == null || mainCamera == null) return false;
        
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return screenPoint.x > 0 && screenPoint.x < 1 && 
               screenPoint.y > 0 && screenPoint.y < 1 && 
               screenPoint.z > 0;
    }

    private void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is not assigned!");
            return;
        }

        // Déclencher l'animation de tir
        animator.SetBool(IsShootingHash, true);
        
        // Note: Le projectile sera instantié via l'Animation Event sur l'animation de tir
    }

    // Cette méthode sera appelée par l'Animation Event sur l'animation de tir
    public void SpawnProjectile()
    {
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Vector3 spawnPosition = transform.position;
        
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        ProjectileScript projectileScript = projectile.GetComponent<ProjectileScript>();
        
        if (projectileScript != null)
        {
            projectileScript.Initialize(directionToPlayer);
            Debug.Log($"Projectile launched in direction: {directionToPlayer}");
        }

        // Réinitialiser le paramètre d'animation
        animator.SetBool(IsShootingHash, false);
    }

    // Cette méthode sera appelée par l'Animation Event à la fin de l'animation de tir
    public void PauseAfterShot()
    {
        StartCoroutine(PauseAnimation());
    }

    public int GetDamageAmount()
    {
        return damageAmount;
    }

    public void TakeHit(Vector2 hitDirection, float force)
    {
        health--;  // Réduit la santé de 1, indépendamment de la force
        if (health <= 0)
        {
            Die();
            return;
        }

        // Appliquer le knockback
        if (!isKnockedBack)
        {
            isKnockedBack = true;
            knockbackTimer = knockbackDuration;
            rb.linearVelocity = hitDirection * knockbackForce;
        }
    }

    private void Die()
    {
        // Désactiver les comportements
        enabled = false;
        animator.SetBool(IsEmerging, false);
        Destroy(gameObject, 0.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerScript player = collision.gameObject.GetComponent<PlayerScript>();
            HeartManager heartManager = FindFirstObjectByType<HeartManager>();
            
            if (player != null && heartManager != null)
            {
                // Infliger des dégâts au joueur
                heartManager.TakeDamage(damageAmount);
                
                // Calculer la direction du knockback (de l'Octorock vers le joueur)
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                
                // Utiliser la méthode StartKnockback du PlayerScript
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

    // Cette méthode sera appelée par l'Animation Event à la fin de l'animation d'émergence
    public void PauseAtEnd()
    {
        StartCoroutine(PauseAnimation());
    }

    private IEnumerator PauseAnimation()
    {
        animator.speed = 0; // Pause l'animation
        yield return new WaitForSeconds(pauseDuration);
        animator.speed = 1; // Reprend l'animation
    }

    private bool IsPlayerInShootingDirection()
    {
        if (playerTransform == null) return false;

        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        EmergingDirection currentDirection = (EmergingDirection)animator.GetInteger(DirectionHash);

        switch (currentDirection)
        {
            case EmergingDirection.Down:
                return angle >= 225 && angle < 315;
            case EmergingDirection.Up:
                return angle >= 45 && angle < 135;
            case EmergingDirection.Left:
                return angle >= 135 && angle < 225;
            case EmergingDirection.Right:
                // Modification ici pour gérer correctement l'angle à droite
                return (angle >= 315) || (angle < 45);
            default:
                return false;
        }
    }

    public void OnReturnToGroundComplete()
    {
        animator.SetBool(IsEmerging, false);
        hasEmerged = false;
        canShoot = false;
    }
}