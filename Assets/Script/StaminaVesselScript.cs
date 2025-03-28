using UnityEngine;
using DG.Tweening;

public class StaminaVesselScript : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float floatHeight = 1.5f;
    [SerializeField] private float moveUpDuration = 0.5f;
    [SerializeField] private float jumpHeight = 0.3f;
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float visibleDuration = 2.5f;
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
            PlayerScript player = other.GetComponent<PlayerScript>();
            if (player != null)
            {
                isCollected = true;
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
