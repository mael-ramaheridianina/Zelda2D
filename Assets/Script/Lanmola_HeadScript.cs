using UnityEngine;
using System.Collections.Generic;

public class Lanmola_HeadScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float minDistanceFromBody = 0.5f;

    [Header("Body Settings")]
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private int bodyPartsCount = 3;
    
    private Transform playerTransform;
    private List<Transform> bodyParts = new List<Transform>();
    private List<Vector3> positionHistory = new List<Vector3>();
    private float distanceBetweenParts = 0.5f;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        CreateSnakeBody();
    }

    void Update()
    {
        MoveHead();
        UpdateBodyMovement();
    }

    void MoveHead()
    {
        if (playerTransform != null)
        {
            // Calcul de la direction vers le joueur
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            
            // Déplacement de la tête
            transform.Translate(direction * moveSpeed * Time.deltaTime);
            
            // Sauvegarde de la position pour le corps
            positionHistory.Insert(0, transform.position);
            
            // Limite l'historique des positions
            if (positionHistory.Count > bodyPartsCount * 50)
            {
                positionHistory.RemoveAt(positionHistory.Count - 1);
            }
        }
    }

    void UpdateBodyMovement()
    {
        // Met à jour la position de chaque partie du corps
        for (int i = 0; i < bodyParts.Count; i++)
        {
            Vector3 targetPosition = positionHistory[Mathf.Min(i * 10, positionHistory.Count - 1)];
            bodyParts[i].position = Vector3.Lerp(
                bodyParts[i].position,
                targetPosition,
                Time.deltaTime * moveSpeed
            );
        }
    }

    void CreateSnakeBody()
    {
        // Crée les parties du corps
        for (int i = 0; i < bodyPartsCount; i++)
        {
            Vector3 position = transform.position - transform.right * (i + 1) * minDistanceFromBody;
            GameObject body = Instantiate(bodyPrefab, position, Quaternion.identity);
            bodyParts.Add(body.transform);
            positionHistory.Add(position);
        }
    }
}
