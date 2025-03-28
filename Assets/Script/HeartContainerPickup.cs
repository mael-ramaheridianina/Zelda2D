using UnityEngine;
using DG.Tweening;

public class HeartContainerPickup : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float floatHeight = 1.5f;    // Hauteur au-dessus du player
    [SerializeField] private float moveUpDuration = 0.5f;  // Durée du déplacement
    [SerializeField] private float jumpHeight = 0.15f;    // Ajout de la hauteur de rebond
    [SerializeField] private float jumpDuration = 0.5f;   // Ajout de la durée de rebond
    [SerializeField] private float visibleDuration = 2.5f; // Durée de visibilité
    [SerializeField] private float fadeOutDuration = 0.3f;

    private SpriteRenderer spriteRenderer;
    private bool isCollected = false;
    private Tween bounceAnimation;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartBouncing();
    }

    private void StartBouncing()
    {
        // Animation de rebond en boucle
        bounceAnimation = transform.DOMoveY(transform.position.y + jumpHeight, jumpDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollected && other.CompareTag("Player"))
        {
            bounceAnimation.Kill(); // Arrête l'animation de rebond
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
