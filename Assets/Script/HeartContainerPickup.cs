using UnityEngine;
using DG.Tweening;

public class HeartContainerPickup : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float floatHeight = 1.5f;    // Hauteur au-dessus du player
    [SerializeField] private float moveUpDuration = 0.5f;  // Durée du déplacement
    [SerializeField] private float visibleDuration = 2.5f; // Durée de visibilité
    [SerializeField] private float fadeOutDuration = 0.3f;

    private SpriteRenderer spriteRenderer;
    private bool isCollected = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollected && other.CompareTag("Player"))
        {
            var heartManager = FindFirstObjectByType<HeartManager>();
            if (heartManager != null)
            {
                isCollected = true;
                heartManager.IncreaseMaxHearts();
                StartPickupSequence(other.transform.position);
            }
        }
    }

    private void StartPickupSequence(Vector3 playerPosition)
    {
        GetComponent<Collider2D>().enabled = false;

        // Position cible juste au-dessus du player
        Vector3 targetPosition = new Vector3(
            playerPosition.x,
            playerPosition.y + floatHeight,
            transform.position.z
        );

        // Déplacement vers la position au-dessus du player
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
