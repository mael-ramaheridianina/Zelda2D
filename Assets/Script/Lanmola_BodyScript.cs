using UnityEngine;

public class Lanmola_BodyScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float offset;

    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float knockbackForce = 5f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSortingOrder(int order)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }

    public void Initialize(float offset)
    {
        this.offset = offset;
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = -(int)(offset * 10);
        }
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

                // Calculer la direction du knockback
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

                // Appliquer le knockback et l'invulnerabilité
                player.StartKnockback(knockbackDirection);
                player.StartInvulnerability();
            }
        }
    }

    public void UpdateCombatSettings(int newDamageAmount, float newKnockbackForce)
    {
        damageAmount = newDamageAmount;
        knockbackForce = newKnockbackForce;
    }
}
