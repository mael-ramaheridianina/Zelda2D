using UnityEngine;
using System.Collections.Generic;

public class Lanmola_HeadScript : MonoBehaviour
{
    [Header("Body Settings")]
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private int bodyPartsCount = 3;
    [SerializeField] private float distanceBetweenParts = 0.5f;
    [SerializeField] private float followSpeed = 5f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    
    private Transform playerTransform;
    private List<GameObject> bodyParts = new List<GameObject>();
    private Queue<Vector3> positionHistory = new Queue<Vector3>();
    
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
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
        while (positionHistory.Count > Mathf.Round(distanceBetweenParts * 50))
        {
            positionHistory.Dequeue();
        }
    }

    void MoveHead()
    {
        if (playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }
    }

    void SpawnBodyParts()
    {
        for (int i = 0; i < bodyPartsCount; i++)
        {
            Vector3 spawnPosition = transform.position - (Vector3.right * distanceBetweenParts * (i + 1));
            GameObject bodyPart = Instantiate(bodyPrefab, spawnPosition, Quaternion.identity);
            bodyPart.GetComponent<Lanmola_BodyScript>().Initialize(i * distanceBetweenParts);
            bodyParts.Add(bodyPart);
        }
    }

    void UpdateBodyParts()
    {
        int index = 0;
        var positions = positionHistory.ToArray();
        foreach (var bodyPart in bodyParts)
        {
            int targetIndex = Mathf.Min(index * 10, positions.Length - 1);
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
