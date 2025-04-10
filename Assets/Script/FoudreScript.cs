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
        float damage = damageLevels[currentLevel - 1];
        
        // Pour OctoRock
        OctoRockScript octoRock = other.GetComponent<OctoRockScript>();
        if (octoRock != null)
        {
            Vector2 hitDirection = (octoRock.transform.position - transform.position).normalized;
            
            // Appliquer TakeHit plusieurs fois selon le niveau de dégâts
            int damageCount = Mathf.RoundToInt(damage);
            for (int i = 0; i < damageCount; i++)
            {
                octoRock.TakeHit(hitDirection, 1f);
            }
            
            Debug.Log($"Foudre niveau {currentLevel} touche OctoRock: {damageCount} fois");
            return;
        }
        
        // Cas 1: Enemy standard
        EnemyScript enemy = other.GetComponent<EnemyScript>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            return;
        }
        
        // Cas 2: Lanmola Head
        Lanmola_HeadScript lanmolaHead = other.GetComponent<Lanmola_HeadScript>();
        if (lanmolaHead != null)
        {
            ApplyDamageToLanmolaHead(lanmolaHead, damage);
            return;
        }
        
        // Cas 3: Lanmola Body
        Lanmola_BodyScript lanmolaBody = other.GetComponent<Lanmola_BodyScript>();
        if (lanmolaBody != null)
        {
            ApplyDamageToLanmolaBody(lanmolaBody, damage);
            return;
        }
        
        // Cas 4: Interface IDamageable (pour les futurs ennemis)
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
    }

    private void ApplyDamageToLanmolaHead(Lanmola_HeadScript lanmola, float damage)
    {
        // Implémentation spéciale pour Lanmola Head
        // Comme cette classe n'a pas de méthode TakeDamage, nous devons l'ajouter
        lanmola.gameObject.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
    }

    private void ApplyDamageToLanmolaBody(Lanmola_BodyScript lanmolaBody, float damage)
    {
        // Trouver la tête parente et lui appliquer les dégâts
        Lanmola_HeadScript head = lanmolaBody.transform.root.GetComponent<Lanmola_HeadScript>();
        if (head != null)
        {
            ApplyDamageToLanmolaHead(head, damage);
        }
    }

    private void ApplyDamageToOctoRock(OctoRockScript octoRock, float damage)
    {
        // OctoRock a déjà une méthode TakeHit mais avec des paramètres différents
        Vector2 hitDirection = (octoRock.transform.position - transform.position).normalized;
        octoRock.TakeHit(hitDirection, damage);
    }
}
