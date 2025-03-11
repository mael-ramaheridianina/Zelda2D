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

    private Transform playerTransform;
    private List<GameObject> bodyParts = new List<GameObject>();
    private Queue<Vector3> positionHistory = new Queue<Vector3>();
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        SpawnBodyParts();
    }

    void Update()
    {
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
            bodyPart.GetComponent<Lanmola_BodyScript>().Initialize(i * distance);
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
}
