using UnityEngine;

public class Epée_Sautille : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float bounceHeight = 0.2f;
    [SerializeField] private float bounceSpeed = 2f;
    
    private Vector3 startPosition;
    private float timeOffset;

    void Start()
    {
        startPosition = transform.position;
        // Offset aléatoire pour que toutes les épées ne sautent pas en même temps
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        // Calcule la position Y avec un effet de rebond sinusoïdal
        float newY = startPosition.y + Mathf.Abs(Mathf.Sin((Time.time + timeOffset) * bounceSpeed)) * bounceHeight;
        
        // Applique la nouvelle position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
