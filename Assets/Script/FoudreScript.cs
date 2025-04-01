using UnityEngine;

public class FoudreScript : MonoBehaviour
{
    [Header("Display Settings")]
    [SerializeField] private float displayDuration = 0.5f;

    [Header("Lightning Levels")]
    [SerializeField] private Sprite[] lightningSprites; // Sprites pour chaque niveau
    [SerializeField] private float[] damageLevels = { 1f, 2f, 3f }; // Dégâts pour chaque niveau
    [SerializeField] private float[] radiusLevels = { 0.5f, 0.75f, 1f }; // Rayon d'effet pour chaque niveau

    private SpriteRenderer spriteRenderer;
    private float displayTimer = 0f;
    private bool isDisplaying = false;
    private int currentLevel = 1;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            if (TryGetComponent<CircleCollider2D>(out CircleCollider2D collider))
            {
                collider.radius = radiusLevels[currentLevel - 1];
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
}
