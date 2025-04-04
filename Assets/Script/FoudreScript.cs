using UnityEngine;

public class FoudreScript : MonoBehaviour
{
    [Header("Display Settings")]
    [SerializeField] private float displayDuration = 0.5f;

    [Header("Lightning Levels")]
    [SerializeField] private Sprite[] lightningSprites; // Sprites pour chaque niveau
    [SerializeField] private float[] damageLevels = { 10f, 20f, 30f }; // Dégâts pour chaque niveau
    [SerializeField] private float[] radiusLevels = { 0.5f, 0.75f, 1f }; // Rayon d'effet pour chaque niveau

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D foudreCollider;
    private float displayTimer = 0f;
    private bool isDisplaying = false;
    private int currentLevel = 1;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        foudreCollider = GetComponent<CircleCollider2D>();

        if (foudreCollider == null)
        {
            foudreCollider = gameObject.AddComponent<CircleCollider2D>();
            foudreCollider.isTrigger = true; // Important !
        }

        if (lightningSprites == null || lightningSprites.Length < 3)
        {
            Debug.LogError("Veuillez assigner 3 sprites pour la foudre!");
        }
        UpdateFoudreSprite();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (isDisplaying)
        {
            displayTimer += Time.deltaTime;
            if (displayTimer >= displayDuration)
            {
                HideFoudre();
            }
        }
    }

    public void ShowFoudre(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
        isDisplaying = true;
        displayTimer = 0f;

        // Activer explicitement le collider
        if (foudreCollider != null)
        {
            foudreCollider.enabled = true;
            Debug.Log("Foudre collider activated");
        }
    }

    private void HideFoudre()
    {
        gameObject.SetActive(false);
        isDisplaying = false;
    }

    public void UpgradeFoudre()
    {
        Debug.Log($"État actuel de la foudre - Niveau: {currentLevel}");
        if (currentLevel < 3)
        {
            currentLevel++;
            UpdateFoudreSprite();
            Debug.Log($"Foudre améliorée - Nouveau niveau: {currentLevel}");
        }
        else
        {
            Debug.Log("Niveau maximum de foudre déjà atteint");
        }
    }

    private void UpdateFoudreSprite()
    {
        if (spriteRenderer != null && lightningSprites != null && currentLevel <= lightningSprites.Length)
        {
            spriteRenderer.sprite = lightningSprites[currentLevel - 1];

            // Mise à jour de la taille du collider si présent
            if (foudreCollider != null)
            {
                foudreCollider.radius = radiusLevels[currentLevel - 1];
                Debug.Log($"Collider radius set to: {foudreCollider.radius}");
            }

            Debug.Log($"Foudre niveau {currentLevel}: " +
                     $"Dégâts = {damageLevels[currentLevel - 1]}, " +
                     $"Rayon = {radiusLevels[currentLevel - 1]}");
        }
    }

    public float GetCurrentDamage()
    {
        return damageLevels[currentLevel - 1];
    }

    public float GetCurrentRadius()
    {
        return radiusLevels[currentLevel - 1];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Foudre collided with {other.gameObject.name}, tag: {other.gameObject.tag}");
        if (other.CompareTag("Enemy"))
        {
            EnemyScript enemy = other.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                float damage = damageLevels[currentLevel - 1];
                enemy.TakeDamage(damage);
                Debug.Log($"Applied {damage} damage to enemy");
            }
        }
    }
}
