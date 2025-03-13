using UnityEngine;
using DG.Tweening;

public class EpéeScript : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float bounceHeight = 0.2f;
    [SerializeField] private float bounceSpeed = 2f;
    [SerializeField] private float floatHeight = 1.5f;
    [SerializeField] private float moveUpDuration = 0.5f;
    [SerializeField] private float visibleDuration = 2.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    
    private Vector3 startPosition;
    private float timeOffset;
    private SpriteRenderer spriteRenderer;
    private bool isCollected = false;

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        if (!isCollected)
        {
            float newY = startPosition.y + Mathf.Abs(Mathf.Sin((Time.time + timeOffset) * bounceSpeed)) * bounceHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollected && other.CompareTag("Player"))
        {
            isCollected = true;
            PlayerScript player = other.GetComponent<PlayerScript>();
            if (player != null)
            {
                player.StartCelebration();
                StartPickupSequence(other.transform.position);
            }
        }
    }

    private void StartPickupSequence(Vector3 playerPosition)
    {
        GetComponent<Collider2D>().enabled = false;

        Vector3 targetPosition = new Vector3(
            playerPosition.x,
            playerPosition.y + floatHeight,
            transform.position.z
        );

        transform.DOMove(targetPosition, moveUpDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                DOVirtual.DelayedCall(visibleDuration, () => {
                    spriteRenderer.DOFade(0, fadeOutDuration)
                        .OnComplete(() => {
                            Destroy(gameObject);
                        });
                });
            });
    }
}
