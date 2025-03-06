using UnityEngine;

public class BatAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private bool isVisible = false;
    private Rigidbody2D rb;

    [SerializeField] private float animationSpeed = 1f;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        
        // Désactive l'animation au départ
        animator.enabled = false;
    }

    void Update()
    {
        // Vérifie si le sprite est visible par la caméra
        if (IsVisibleToCamera())
        {
            if (!isVisible)
            {
                // Active l'animation quand devient visible
                animator.enabled = true;
                isVisible = true;
                Debug.Log("Bat became visible - Starting animation");
            }

            // Vérifie la direction du mouvement
            if (rb.linearVelocity.x > 0)
            {
                animator.Play("BatFlyRight");
                spriteRenderer.flipX = false;
            }
            else if (rb.linearVelocity.x < 0)
            {
                animator.Play("BatFlyRight");
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            if (isVisible)
            {
                // Désactive l'animation quand sort du champ
                animator.enabled = false;
                isVisible = false;
                Debug.Log("Bat is no longer visible - Stopping animation");
            }
        }
    }

    private bool IsVisibleToCamera()
    {
        if (!spriteRenderer.isVisible)
            return false;

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && 
               viewportPoint.y >= 0 && viewportPoint.y <= 1 && 
               viewportPoint.z > 0;
    }
}
