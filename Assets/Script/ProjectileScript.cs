using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifetime = 2f;
    private Vector2 direction;
    private SpriteRenderer spriteRenderer;
    private Collider2D projectileCollider;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        projectileCollider = GetComponent<Collider2D>();
    }

    public void Initialize(Vector2 dir, bool isVisible = true)
    {
        direction = dir.normalized;
        SetVisibility(isVisible);
        Destroy(gameObject, lifetime);
    }

    private void SetVisibility(bool visible)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = visible;
        if (projectileCollider != null) projectileCollider.enabled = visible;
    }

    void Update()
    {
        Vector3 movement = (Vector3)(direction * speed * Time.deltaTime);
        transform.position += movement;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerScript player = other.GetComponent<PlayerScript>();
            HeartManager heartManager = FindFirstObjectByType<HeartManager>();
            
            if (player != null && heartManager != null)
            {
                heartManager.TakeDamage(1);
            }
            Destroy(gameObject);
        }
        else if (!other.CompareTag("EnemyProjectile") && !other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
