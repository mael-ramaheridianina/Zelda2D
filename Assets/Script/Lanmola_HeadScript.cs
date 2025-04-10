using UnityEngine;
using System.Collections.Generic;

public class Lanmola_HeadScript : MonoBehaviour
{
    [Header("Body Settings")]
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private int bodyPartsCount = 3;
    [SerializeField] private float distanceBetweenHeadAndBody = 1f; // Distance tête-corps
    [SerializeField] private float distanceBetweenBodies = 0.5f;    // Distance entre corps
    [SerializeField] private float followSpeed = 5f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationOffset = 90f; // Ajout d'un offset configurable

    [Header("Combat Settings")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float knockbackForce = 5f;

    private Transform playerTransform;
    private List<GameObject> bodyParts = new List<GameObject>();
    private Queue<Vector3> positionHistory = new Queue<Vector3>();
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private bool isVisible = true;
    private int health = 100; // Ajouter une variable pour la santé
    
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        SpawnBodyParts();
    }

    void Update()
    {
        // Vérifie si le Lanmola est visible
        isVisible = IsVisibleOnScreen();

        if (!isVisible)
        {
            return; // Ne fait rien si hors caméra
        }

        StorePosition();
        MoveHead();
        UpdateBodyParts();
    }

    void StorePosition()
    {
        positionHistory.Enqueue(transform.position);
        while (positionHistory.Count > Mathf.Round(distanceBetweenBodies * 50))
        {
            positionHistory.Dequeue();
        }
    }

    void MoveHead()
    {
        if (playerTransform != null)
        {
            // Calcul de la direction
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            
            // Rotation de la tête avec offset
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            // Déplacement
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }
    }

    void SpawnBodyParts()
    {
        Vector3 previousPosition = transform.position;
        
        for (int i = 0; i < bodyPartsCount; i++)
        {
            // Pour le premier corps, utilise la distance tête-corps
            float distance = (i == 0) ? distanceBetweenHeadAndBody : distanceBetweenBodies;
            
            Vector3 spawnPosition = previousPosition + (Vector3.down * distance);
            GameObject bodyPart = Instantiate(bodyPrefab, spawnPosition, Quaternion.identity);
            var bodyScript = bodyPart.GetComponent<Lanmola_BodyScript>();
            bodyScript.Initialize(i * distance);
            bodyScript.UpdateCombatSettings(damageAmount, knockbackForce);
            bodyParts.Add(bodyPart);
            
            previousPosition = spawnPosition;
        }
    }

    void UpdateBodyParts()
    {
        int index = 0;
        var positions = positionHistory.ToArray();
        foreach (var bodyPart in bodyParts)
        {
            // Ajuste l'index en fonction des différentes distances
            float distanceMultiplier = (index == 0) ? distanceBetweenHeadAndBody : distanceBetweenBodies;
            int targetIndex = Mathf.Min(
                Mathf.RoundToInt(index * 10 * (distanceMultiplier / 0.5f)), 
                positions.Length - 1
            );
            
            if (targetIndex >= 0 && bodyPart != null)
            {
                Vector3 targetPos = positions[targetIndex];
                bodyPart.transform.position = Vector3.Lerp(
                    bodyPart.transform.position,
                    targetPos,
                    Time.deltaTime * followSpeed
                );
            }
            index++;
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

    public int DamageAmount
    {
        get => damageAmount;
        set
        {
            damageAmount = value;
            UpdateBodyCombatSettings();
        }
    }

    public float KnockbackForce
    {
        get => knockbackForce;
        set
        {
            knockbackForce = value;
            UpdateBodyCombatSettings();
        }
    }

    private void UpdateBodyCombatSettings()
    {
        foreach (var bodyPart in bodyParts)
        {
            if (bodyPart != null)
            {
                var bodyScript = bodyPart.GetComponent<Lanmola_BodyScript>();
                if (bodyScript != null)
                {
                    bodyScript.UpdateCombatSettings(damageAmount, knockbackForce);
                }
            }
        }
    }

    private bool IsVisibleOnScreen()
    {
        if (mainCamera == null) return false;

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && 
               viewportPoint.y >= 0 && viewportPoint.y <= 1 && 
               viewportPoint.z > 0;
    }

    public void ApplyDamage(float damage)
    {
        // Convertir en int si nécessaire
        int damageInt = Mathf.CeilToInt(damage);
        
        // Réduire la santé
        health -= damageInt;
        
        // Gérer la mort
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
