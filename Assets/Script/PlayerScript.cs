using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.2f;
    
    [Header("Invulnerability Settings")]
    [SerializeField] private float invulnerabilityDuration = 1f;
    [SerializeField] private float blinkInterval = 0.1f;
    private float invulnerabilityBlinkTimer = 0f; // Nouveau timer spécifique

    [Header("Low Health Settings")]
    [SerializeField] private float lowHealthThreshold = 2f; // Seuil de vie basse (en points de vie)
    [SerializeField] private Color lowHealthColor = Color.red;
    private Color originalColor;
    private bool isLowHealth = false;

    [Header("Celebration Settings")]
    [SerializeField] private float celebrationDuration = 1f;
    [SerializeField] private Sprite celebrationSprite;
    private bool isCelebrating = false;
    private float celebrationTimer = 0f;

    [Header("Direction Sprites")]
    [SerializeField] private Sprite frontSprite;  // Sprite face avant
    [SerializeField] private Sprite backSprite;   // Sprite de dos
    [SerializeField] private Sprite leftSprite;   // Sprite côté gauche
    [SerializeField] private Sprite rightSprite;  // Sprite côté droit

    [Header("Sword Sprites")]
    [SerializeField] private Sprite frontSwordSprite;
    [SerializeField] private Sprite backSwordSprite;
    [SerializeField] private Sprite leftSwordSprite;
    [SerializeField] private Sprite rightSwordSprite;

    private bool hasSword = false;

    private Rigidbody2D rb;
    private Vector2 movement;
    private HeartManager heartManager;
    private bool isKnockedBack = false;
    private bool isInvulnerable = false;
    private float knockbackTimer = 0f;
    private float invulnerabilityTimer = 0f;
    private SpriteRenderer spriteRenderer;
    private float blinkTimer = 0f;
    private Animator animator;
    private Sprite defaultSprite;
    private Sprite idleSprite;
    private bool isInContactWithEnemy = false;
    private EnemyScript currentEnemy = null;

    void Start()
    {
         rb = GetComponent<Rigidbody2D>();
        // Cherche le SpriteRenderer dans l'enfant
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        heartManager = FindFirstObjectByType<HeartManager>();
        // Cherche l'Animator dans l'enfant
        animator = GetComponentInChildren<Animator>();
        
        if (heartManager == null)
        {
            Debug.LogError("HeartManager non trouvé dans la scène!");
        }

        defaultSprite = frontSprite; // Le sprite par défaut est maintenant le sprite face avant
        originalColor = spriteRenderer.color;
        idleSprite= defaultSprite;
    }

    void Update()
    {        
        UpdateCelebration();

        UpdateKnockback();
        UpdateInvulnerability();
        UpdateLowHealth();
        UpdateMovement();
       
    }

    private void UpdateKnockback()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockedBack = false;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private void UpdateInvulnerability()
    {
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            invulnerabilityBlinkTimer -= Time.deltaTime;

            // Gestion du clignotement
            if (invulnerabilityBlinkTimer <= 0)
            {
                invulnerabilityBlinkTimer = blinkInterval;
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }

            // Fin de l'invulnérabilité UNIQUEMENT quand le sprite est visible
            if (invulnerabilityTimer <= 0 && spriteRenderer.enabled)
            {
                isInvulnerable = false;
                spriteRenderer.enabled = true;
                
                // Vérifie si l'ennemi est toujours en contact
                if (isInContactWithEnemy && currentEnemy != null)
                {
                    // Réapplique les dégâts
                    int damage = currentEnemy.GetDamageAmount();
                    heartManager.TakeDamage(damage);
                    
                    // Recalcule la direction du knockback
                    Vector2 knockbackDirection = (transform.position - currentEnemy.transform.position).normalized;
                    StartKnockback(knockbackDirection);
                    StartInvulnerability();
                }
                
                // Mise à jour de la couleur
                if (isLowHealth)
                {
                    spriteRenderer.color = lowHealthColor;
                }
                else
                {
                    spriteRenderer.color = originalColor;
                }
            }
        }
    }

    private void UpdateLowHealth()
    {
        if (!isInvulnerable) // Ne pas interférer avec l'invulnérabilité
        {
            bool wasLowHealth = isLowHealth;
            isLowHealth = heartManager.currentHealth <= lowHealthThreshold;

            if (isLowHealth)
            {
                blinkTimer -= Time.deltaTime;
                if (blinkTimer <= 0)
                {
                    blinkTimer = blinkInterval;
                    spriteRenderer.color = spriteRenderer.color == originalColor ? lowHealthColor : originalColor;
                }
            }
            else if (wasLowHealth)
            {
                // Restaure la couleur normale quand on sort de l'état de vie basse
                spriteRenderer.color = originalColor;
            }
        }
    }

    private void UpdateMovement()
    {
        if (isKnockedBack || isCelebrating) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movement = new Vector2(moveX, moveY).normalized;

        Debug.Log($"moveX: {moveX}, moveY: {moveY}");
        Debug.Log($"Abs moveY: {Mathf.Abs(moveY)}, Abs moveX: {Mathf.Abs(moveX)}");

        if (movement != Vector2.zero)
        {
            animator.enabled = true;
            if (Mathf.Abs(moveY) > Mathf.Abs(moveX))
            {
                if (moveY > 0)
                {
                    if (hasSword)
                    {
                        animator.Play("WalkUpSword");
                        idleSprite = backSwordSprite;  // Met à jour directement avec le sprite épée
                    }
                    else
                    {
                        animator.Play("WalkUp");
                        idleSprite = backSprite;
                    }
                }
                else if (moveY < 0)
                {
                    animator.Play("WalkDown");
                    idleSprite = hasSword ? frontSwordSprite : frontSprite;
                }
            }
            else
            {
                if (moveX > 0)
                {
                    animator.Play("WalkRight");
                    idleSprite = hasSword ? rightSwordSprite : rightSprite;
                }
                else if (moveX < 0)
                {
                    animator.Play("WalkLeft");
                    idleSprite = hasSword ? leftSwordSprite : leftSprite;
                }
            }
        }
        else
        {
            animator.enabled = false;
            spriteRenderer.sprite = idleSprite; // Utilise directement idleSprite qui est déjà le bon sprite
        }

        rb.linearVelocity = movement * moveSpeed;
    }

    private void UpdateCelebration()
    { 
        if (isCelebrating)
        {
            animator.enabled = false;
            celebrationTimer -= Time.deltaTime;
            if (celebrationTimer <= 0)
            {
                isCelebrating = false;
                // On met à jour le sprite seulement après la fin de la célébration
                if (hasSword)
                {
                    UpdateCurrentSprite(); // Met à jour vers le sprite avec épée
                }
                else
                {
                    spriteRenderer.sprite = defaultSprite;
                    idleSprite = defaultSprite;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!isKnockedBack && !isCelebrating) // Ajout de la vérification de célébration
        {
            rb.linearVelocity = movement * moveSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            isInContactWithEnemy = true;
            currentEnemy = collision.gameObject.GetComponent<EnemyScript>();
            
            if (!isInvulnerable && currentEnemy != null && heartManager != null)
            {
                int damage = currentEnemy.GetDamageAmount();
                Debug.Log($"Dégâts reçus : {damage}");
                heartManager.TakeDamage(damage);

                Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
                StartKnockback(knockbackDirection);
                StartInvulnerability();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            isInContactWithEnemy = false;
            currentEnemy = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coeur de vie"))
        {
            StartCelebration();
        }
        else if (other.CompareTag("Epée"))
        {
            StartCelebration();
            ObtainSword();
        }
    }

    private void ObtainSword()
    {
        hasSword = true;
        defaultSprite = frontSwordSprite;
        // UpdateCurrentSprite sera appelé après la célébration
    }

    private void UpdateCurrentSprite()
    {
        // Met à jour le sprite selon la direction et si on a l'épée
        if (hasSword)
        {
            if (idleSprite == frontSprite) idleSprite = frontSwordSprite;
            else if (idleSprite == backSprite) idleSprite = backSwordSprite;
            else if (idleSprite == leftSprite) idleSprite = leftSwordSprite;
            else if (idleSprite == rightSprite) idleSprite = rightSwordSprite;
        }
        spriteRenderer.sprite = idleSprite;
    }

    public void StartKnockback(Vector2 direction)
    {
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        rb.linearVelocity = direction * knockbackForce;
    }

    public void StartInvulnerability()
    {
        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;
    }

    public void StartCelebration()
    {
        isCelebrating = true;
        celebrationTimer = celebrationDuration;
        
        // Désactive le mouvement pendant la célébration
        rb.linearVelocity = Vector2.zero;
        
        // Tourne le sprite vers la caméra (en 2D, c'est juste le sprite par défaut)
        transform.rotation = Quaternion.identity;
        
        // Change le sprite pour celui avec les bras levés
        // Vous devez avoir un sprite spécial pour cette animation
        spriteRenderer.sprite = celebrationSprite;
    }

    public bool IsCelebrating()
    {
        return isCelebrating;
    }
}
