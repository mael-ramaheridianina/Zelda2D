using UnityEngine;
using DG.Tweening;

public class GrimoireScript : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float floatHeight = 1.5f;
    [SerializeField] private float moveUpDuration = 0.5f;
    [SerializeField] private float jumpHeight = 0.3f;
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float visibleDuration = 2.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;

    [SerializeField] private ViseurScript viseur;
    private SpriteRenderer spriteRenderer;
    private bool isCollected = false;
    private bool isHidden = false;
    private Tween bounceAnimation;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartBouncing();
    }

    private void Update()
    {
        if (viseur != null && !isCollected)
        {
            if (viseur.IsVisible)
            {
                // Cache le grimoire quand on entre en mode visée
                if (!isHidden)
                {
                    isHidden = true;
                    spriteRenderer.enabled = false;
                    GetComponent<Collider2D>().enabled = false;
                }
            }
            else
            {
                // Montre le grimoire quand on sort du mode visée
                if (isHidden)
                {
                    isHidden = false;
                    spriteRenderer.enabled = true;
                    GetComponent<Collider2D>().enabled = true;
                }
            }
        }
    }

    private void StartBouncing()
    {
        bounceAnimation = transform.DOMoveY(transform.position.y + jumpHeight, jumpDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isCollected && other.CompareTag("Player"))
        {
            bounceAnimation.Kill();
            PlayerScript player = other.GetComponent<PlayerScript>();
            if (player != null && viseur != null)
            {
                isCollected = true;
                player.StartCelebration();
                viseur.IncreaseMaxYPresses();  // Changé de IncrementYPressCount à IncreaseMaxYPresses
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
