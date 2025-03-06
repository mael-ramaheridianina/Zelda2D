using UnityEngine;
using DG.Tweening; // Assurez-vous d'avoir DOTween importé dans votre projet

public class HeartPickup : MonoBehaviour
{
    [Header("Heal Settings")]
    [SerializeField] private int healAmount = 2;
    
    [Header("Animation Settings")]
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpDuration = 0.5f;
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
            Debug.Log("Collision avec le joueur !");
            var heartManager = FindFirstObjectByType<HeartManager>();
            // Vérifie si le joueur n'est pas déjà au maximum de sa vie
            if (heartManager != null && heartManager.currentHealth < heartManager.maxHealth)
            {
                isCollected = true;
                heartManager.Heal(healAmount); // Soigne le joueur
                StartPickupSequence(other.GetComponent<PlayerScript>(), heartManager);
            }
            else
            {
                Debug.Log("Vie déjà au maximum !");
            }
        }
    }

    private void StartPickupSequence(PlayerScript player, HeartManager heartManager)
    {
        // Désactive le collider
        GetComponent<Collider2D>().enabled = false;

        // Séquence d'animation
        transform.DOJump(transform.position, jumpHeight, 1, jumpDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                // Fade out
                spriteRenderer.DOFade(0, fadeOutDuration)
                    .OnComplete(() => {
                        // Détruit l'objet
                        Destroy(gameObject);
                    });
            });
    }
}
